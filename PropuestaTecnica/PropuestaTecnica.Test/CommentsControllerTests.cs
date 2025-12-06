using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Comments;
using PropuestaTecnica.DataAccess.UnitOfWork;
using PropuestaTecnica.WebAPI.Controllers;
using PropuestaTecnica.WebAPI.Models;
using System.Security.Claims;

namespace PropuestaTecnica.Test;

public class CommentsControllerTests
{
    private CommentsController CreateController(
        Mock<ICommentService> commentServiceMock,
        Mock<IUnitOfWork> uowMock)
    {
        var controller = new CommentsController(
            commentServiceMock.Object,
            uowMock.Object
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[] { new Claim(ClaimTypes.NameIdentifier, "user-123") },
            "mock"
        ));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    // TEST 1 — Camino feliz: agregar comentario correctamente
    [Fact]
    public async Task AddComment_ShouldReturnOk_WhenCommentIsCreated()
    {
        // Arrange
        var commentServiceMock = new Mock<ICommentService>();
        var uowMock = new Mock<IUnitOfWork>();

        var dto = new CommentCreateDTO
        {
            Message = "Todo está fallando compa"
        };

        var responseDto = new CommentResponseDTO
        {
            Id = 1,
            Message = dto.Message,
            UserId = "user-123",
            UserEmail = "test@example.com"
        };

        commentServiceMock
            .Setup(s => s.AddCommentAsync(10, It.IsAny<CommentCreateDTO>()))
            .ReturnsAsync(responseDto);

        var controller = CreateController(commentServiceMock, uowMock);

        // Act
        var result = await controller.AddComment(10, dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var ok = result as OkObjectResult;
        ok!.Value.Should().BeOfType<ApiResponse<CommentResponseDTO>>();

        var apiResponse = ok.Value as ApiResponse<CommentResponseDTO>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Message.Should().Be(dto.Message);
        apiResponse.Data.UserId.Should().Be("user-123");

        commentServiceMock.Verify(s => s.AddCommentAsync(10, It.IsAny<CommentCreateDTO>()), Times.Once);
    }

    // TEST 2 — Error: incidente no existe → NotFound
    [Fact]
    public async Task AddComment_ShouldReturnNotFound_WhenIncidentDoesNotExist()
    {
        // Arrange
        var commentServiceMock = new Mock<ICommentService>();
        var uowMock = new Mock<IUnitOfWork>();

        // Si el servicio devuelve null significa que incidente no existe
        commentServiceMock
            .Setup(s => s.AddCommentAsync(99, It.IsAny<CommentCreateDTO>()))
            .ReturnsAsync((CommentResponseDTO?)null);

        var controller = CreateController(commentServiceMock, uowMock);

        var dto = new CommentCreateDTO
        {
            Message = "Mensaje"
        };

        // Act
        var result = await controller.AddComment(99, dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var response = result as NotFoundObjectResult;
        var api = response!.Value as ApiResponse<string>;

        api!.Success.Should().BeFalse();
        api.Message.Should().Be("No se pudo agregar el comentario. El incidente no existe.");

        commentServiceMock.Verify(s => s.AddCommentAsync(99, dto), Times.Once);
    }
}