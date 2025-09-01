namespace Func.Services;

/// <summary>
/// Интерфейс для работы с файлами настройки приложения
/// </summary>
/// <typeparam name="T">Класс, свойства которого представляют настройки приложения</typeparam>
public interface ISettingsProvider<T> : ICanSave
{
    /// <summary>
    /// Класс свойств настроек
    /// </summary>
    T Value { get; set; }

    /// <summary>
    /// Occurs after settings are loaded or the settings object reference is changed.
    /// </summary>
    event EventHandler? Changed;

    /// <summary>
    /// Occurs when saving settings.
    /// </summary>
    event EventHandler? Saving;

    /// <summary>
    /// Loads settings file if present, or creates a new object with default values.
    /// </summary>
    /// <param name="fileName">The file path to load the serialized settings object from.</param>
    /// <returns></returns>
    Ex<bool> Load(Ex<FileName> fileName);

    /// <summary>
    /// Saves settings into specified path.
    /// </summary>
    /// <param name="path">The file path to save the serialized settings object to.</param>
    /// <returns></returns>
    Ex<bool> Save(Ex<FileName> fileName);

}
