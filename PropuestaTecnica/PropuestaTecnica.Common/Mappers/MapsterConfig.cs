using Mapster;
using PropuestaTecnica.Common.DTOs.Categories;
using PropuestaTecnica.Common.DTOs.Comments;
using PropuestaTecnica.Common.DTOs.Incidents;
using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.Common.Mappers;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        MapIncidents();
        MapCategories();
        MapComments();
    }

    /// <summary>
    /// Mapeo para incidentes
    /// </summary>
    private static void MapIncidents()
    {
        // Entity -> ResponseDTO
        TypeAdapterConfig<Incident, IncidentResponseDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
            .Map(dest => dest.UserId, src => src.UserId)
        //.Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email : null)
        ;

        // CreateDTO -> Entity
        TypeAdapterConfig<IncidentCreateDTO, Incident>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Status)   // El estado inicial lo define la app
            .Ignore(dest => dest.Category) // Evita cargar navegación

            //.Ignore(dest => dest.User)

            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.UserId, src => src.UserId);

        // UpdateDTO -> Entity
        TypeAdapterConfig<IncidentUpdateDTO, Incident>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CategoryId)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.Category)
            //.Ignore(dest => dest.User)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status);
    }

    /// <summary>
    /// Mapeo para categorías
    /// </summary>
    private static void MapCategories()
    {
        TypeAdapterConfig<Category, CategoryDTO>
            .NewConfig()
            .TwoWays();
    }

    /// <summary>
    /// Mapeo para comentarios
    /// </summary>
    private static void MapComments()
    {
        // Entity -> ResponseDTO
        TypeAdapterConfig<Comment, CommentResponseDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Message, src => src.Message)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UserId, src => src.UserId)
        //.Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email : null)
        ;

        // CreateDTO -> Entity
        TypeAdapterConfig<CommentCreateDTO, Comment>
            .NewConfig()
            .Ignore(dest => dest.Id)
            //.Ignore(dest => dest.User)
            .Ignore(dest => dest.Incident)
            .Map(dest => dest.Message, src => src.Message)
            .Map(dest => dest.UserId, src => src.UserId);
    }
}