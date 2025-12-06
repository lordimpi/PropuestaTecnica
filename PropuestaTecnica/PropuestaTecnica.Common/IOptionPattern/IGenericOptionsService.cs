namespace PropuestaTecnica.Common.IOptionPattern;

/// <summary>
/// Interfaz que proporciona acceso genérico a opciones de configuración.
/// Admite la obtención de opciones utilizando <c>IOptions&lt;T&gt;</c>, <c>IOptionsSnapshot&lt;T&gt;</c> y <c>IOptionsMonitor&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T">
/// El tipo de las opciones de configuración. Debe ser una clase con un constructor sin parámetros.
/// </typeparam>
public interface IGenericOptionsService<T> where T : class, new()
{
    /// <summary>
    /// Obtiene las opciones de configuración estáticas.
    /// Estas opciones se cargan una vez y permanecen inmutables durante el ciclo de vida de la aplicación.
    /// </summary>
    /// <returns>
    /// Las opciones de configuración del tipo <typeparamref name="T"/>.
    /// </returns>
    T GetOptions();

    /// <summary>
    /// Obtiene una instantánea de las opciones de configuración.
    /// Útil para obtener configuraciones actualizadas dentro del contexto de una única solicitud HTTP.
    /// </summary>
    /// <returns>
    /// Las opciones de configuración del tipo <typeparamref name="T"/>.
    /// </returns>
    T GetSnapshotOptions();

    /// <summary>
    /// Obtiene opciones de configuración monitoreadas dinámicamente.
    /// Permite actualizaciones en tiempo real cuando cambia la fuente de configuración.
    /// </summary>
    /// <returns>
    /// Las opciones de configuración del tipo <typeparamref name="T"/>.
    /// </returns>
    T GetMonitorOptions();
}