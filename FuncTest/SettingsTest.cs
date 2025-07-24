using Func;
using Func.Impl;
using Func.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FuncTest;

public class SettingTest
{
    [Fact]
    public void IoC_Settings_Test()
    {
        var services = new ServiceCollection()
            .AddTransient<IEnvironmentService, EnvironmentService>()
            .AddTransient<ISerializationService, SerializationService>()
            .AddTransient<IJsonTypeInfoResolver, SourceGenerationContext>()
            .AddTransient<ISettingsProvider<AppSettings>, SettingsProvider<AppSettings>>();

        using var serviceProvider = services.BuildServiceProvider();

        var settings = serviceProvider.GetService<ISettingsProvider<AppSettings>>();
        var env = serviceProvider.GetService<IEnvironmentService>();

        Assert.Equal("test", settings?.Value.Name);
        Assert.Equal(123, settings?.Value.Value);
        Assert.Equal("C:\\temp\\Func\\FuncTest\\bin\\Release\\net8.0\\", env?.AppDirectory);

        var file = FileName.OpenOrCreate(env?.AppDirectory + "App.set");
        var res = settings?.Save(file);

        Assert.True(res?.IsSuccess);

        var v = settings!.Load(file);
        Assert.True(v.IsSuccess);

        file.Delete();

        Assert.False(file.Exists());
    }
}


// простой вывод на консоль
public class AppSettings
{
    public string Name { get; set; } = "test";
    public int Value { get; set; } = 123;
}

[JsonSerializable(typeof(AppSettings))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
