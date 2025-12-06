using Microsoft.Extensions.Options;

namespace PropuestaTecnica.Common.IOptionPattern;


/// <summary>
/// Servicio genérico para gestionar configuraciones de tipo <typeparamref name="T"/>.
/// Permite acceder a configuraciones estáticas, instantáneas y dinámicas.
/// </summary>
/// <typeparam name="T">Clase que representa el tipo de opciones de configuración.</typeparam>
public class GenericOptionsService<T> : IGenericOptionsService<T> where T : class, new()
{
    private readonly IOptions<T> _options;
    private readonly IOptionsSnapshot<T> _snapshotOptions;
    private readonly IOptionsMonitor<T> _monitorOptions;

    /// <summary>
    /// Inicializa una nueva instancia del servicio <see cref="GenericOptionsService{T}"/>.
    /// </summary>
    /// <param name="options">Instancia de <see cref="IOptions{T}"/> para obtener configuraciones estáticas.</param>
    /// <param name="snapshotOptions">Instancia de <see cref="IOptionsSnapshot{T}"/> para obtener configuraciones actualizadas en cada solicitud.</param>
    /// <param name="monitorOptions">Instancia de <see cref="IOptionsMonitor{T}"/> para observar cambios en las configuraciones dinámicas.</param>
    public GenericOptionsService(
        IOptions<T> options,
        IOptionsSnapshot<T> snapshotOptions,
        IOptionsMonitor<T> monitorOptions)
    {
        _options = options;
        _snapshotOptions = snapshotOptions;
        _monitorOptions = monitorOptions;
    }

    /// <summary>
    /// Obtiene las opciones estáticas configuradas mediante <see cref="IOptions{T}"/>.
    /// Estas opciones permanecen constantes durante el ciclo de vida de la aplicación.
    /// </summary>
    /// <returns>Instancia de <typeparamref name="T"/> con los valores configurados.</returns>
    public T GetOptions()
    {
        return _options.Value;
    }

    /// <summary>
    /// Obtiene una instantánea de las opciones configuradas mediante <see cref="IOptionsSnapshot{T}"/>.
    /// Útil en aplicaciones web donde cada solicitud puede tener configuraciones actualizadas.
    /// </summary>
    /// <returns>Instancia de <typeparamref name="T"/> con los valores configurados en ese momento.</returns>
    public T GetSnapshotOptions()
    {
        return _snapshotOptions.Value;
    }

    /// <summary>
    /// Obtiene las opciones dinámicas configuradas mediante <see cref="IOptionsMonitor{T}"/>.
    /// Estas opciones pueden cambiar en tiempo de ejecución y permiten observar dichos cambios.
    /// </summary>
    /// <returns>Instancia de <typeparamref name="T"/> con los valores actuales configurados.</returns>
    public T GetMonitorOptions()
    {
        return _monitorOptions.CurrentValue;
    }
}
