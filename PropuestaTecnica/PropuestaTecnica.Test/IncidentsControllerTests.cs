using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Incidents;
using PropuestaTecnica.DataAccess.UnitOfWork;
using PropuestaTecnica.WebAPI.Controllers;
using PropuestaTecnica.WebAPI.Models;
using System.Security.Claims;

namespace PropuestaTecnica.Test;

public class IncidentsControllerTests
{
    private IncidentsController CreateController(
        Mock<IIncidentService> incidentServiceMock,
        Mock<IUnitOfWork> uowMock,
        Mock<IOutputCacheStore> cacheStoreMock)
    {
        var controller = new IncidentsController(
            incidentServiceMock.Object,
            uowMock.Object,
            cacheStoreMock.Object
        );

        // Simular contexto Http para que tenga UserId
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-123")
            },
            "mock"
        ));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    // TEST 1 - Camino feliz: crear incidente válido
    [Fact]
    public async Task Create_ShouldReturnCreatedAt_WhenIncidentIsValid()
    {
        // Arrange
        var incidentServiceMock = new Mock<IIncidentService>();
        var uowMock = new Mock<IUnitOfWork>();
        var cacheMock = new Mock<IOutputCacheStore>();

        var createDto = new IncidentCreateDTO
        {
            Title = "Falla en servidor",
            Description = "No responde desde las 10am",
            CategoryId = 1
        };

        var responseDto = new IncidentResponseDTO
        {
            Id = 10,
            Title = createDto.Title,
            Description = createDto.Description,
            CategoryId = createDto.CategoryId,
            UserId = "test-user-123"
        };

        incidentServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<IncidentCreateDTO>()))
            .ReturnsAsync(responseDto);

        var controller = CreateController(incidentServiceMock, uowMock, cacheMock);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        var created = result as CreatedAtActionResult;
        created!.Value.Should().BeOfType<ApiResponse<IncidentResponseDTO>>();

        var apiResponse = created.Value as ApiResponse<IncidentResponseDTO>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(10);
        apiResponse.Data.Title.Should().Be(createDto.Title);

        apiResponse.Data.UserId.Should().Be("test-user-123");

        incidentServiceMock.Verify(s => s.CreateAsync(It.IsAny<IncidentCreateDTO>()), Times.Once);
    }

    // TEST 2 - Error: actualizar incidente inexistente
    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenIncidentDoesNotExist()
    {
        // Arrange
        var incidentServiceMock = new Mock<IIncidentService>();
        var uowMock = new Mock<IUnitOfWork>();
        var cacheMock = new Mock<IOutputCacheStore>();

        var updateDto = new IncidentUpdateDTO
        {
            Title = "Nuevo título",
            Description = "Nueva descripción",
            Status = PropuestaTecnica.Common.Enums.IncidentStatus.InProgress
        };

        // El servicio devuelve FALSE lo que significa que incidente no encontrado
        incidentServiceMock
            .Setup(s => s.UpdateAsync(99, updateDto))
            .ReturnsAsync(false);

        var controller = CreateController(incidentServiceMock, uowMock, cacheMock);

        // Act
        var result = await controller.Update(99, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var response = result as NotFoundObjectResult;
        var apiResponse = response!.Value as ApiResponse<string>;

        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("No se pudo actualizar el incidente.");

        incidentServiceMock.Verify(s => s.UpdateAsync(99, updateDto), Times.Once);
    }
}