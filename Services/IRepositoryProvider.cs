﻿namespace Func.Services;

public interface IRepositoryProvider<T> where T : class, new()
{
    /// <summary>
    /// Gets or sets the current settings.
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
    T Load();

    /// <summary>
    /// Loads settings file if present, or creates a new object with default values.
    /// </summary>
    /// <param name="path">The file path to load the serialized settings object from.</param>
    T Load(string path);

    /// <summary>
    /// Saves settings into an XML file.
    /// </summary>
    void Save();

    /// <summary>
    /// Saves settings into specified path.
    /// </summary>
    /// <param name="path">The file path to save the serialized settings object to.</param>
    void Save(string path);
}
