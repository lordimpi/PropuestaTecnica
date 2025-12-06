using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.Common.Enums;

namespace PropuestaTecnica.DataAccess.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        //Categorías
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
                {
                    new() { Name = "Base de Datos" },
                    new() { Name = "Redes" },
                    new() { Name = "Software" },
                    new() { Name = "Infraestructura" },
                    new() { Name = "Hardware" }
                };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Usuario administrador por defecto
        if (!context.Users.Any())
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@prueba.com",
                Email = "admin@prueba.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(defaultUser, "Admin123!");
        }

        // Incidentes de prueba
        if (!context.Incidents.Any())
        {
            var firstUser = await context.Users.FirstAsync();
            var categories = await context.Categories.ToListAsync();

            var incidents = new List<Incident>
                {
                    new() {
                        Title = "Error en base de datos",
                        Description = "La base de datos no responde.",
                        CategoryId = categories[0].Id,
                        UserId = firstUser.Id,
                        Status = IncidentStatus.Open
                    },
                    new() {
                        Title = "Problema de red",
                        Description = "Sin acceso a internet.",
                        CategoryId = categories[1].Id,
                        UserId = firstUser.Id,
                        Status = IncidentStatus.InProgress
                    }
                };

            context.Incidents.AddRange(incidents);
            await context.SaveChangesAsync();
        }
    }
}
