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
            .AddSingleton<ISettingsProvider<AppSettings>, SettingsProvider<AppSettings>>()
            .AddSingleton<ISettingsProvider<SubAppSettings>, SettingsProvider<SubAppSettings>>();

        using var serviceProvider = services.BuildServiceProvider();

        ISettingsProvider<AppSettings> settings = serviceProvider.GetRequiredService<ISettingsProvider<AppSettings>>();
        var env = serviceProvider.GetService<IEnvironmentService>();

        Assert.Equal("test", settings.Value.Name);
        Assert.Equal(123, settings.Value.Value);
        Assert.Equal("C:\\temp\\Func\\FuncTest\\bin\\Release\\net8.0\\", env?.AppDirectory);

        var file = FileName.OpenOrCreate(env?.AppDirectory + "App.set");
        settings.Value.Name = "test_changed";
        settings.Value.Value = 321;

        var res = settings?.Save(file);

        Assert.True(res?.IsSuccess);

        var v = settings!.Load(file);
        Assert.True(v.IsSuccess);
        Assert.Equal("test_changed", settings.Value.Name);
        Assert.Equal(321, settings.Value.Value);

        file.Delete();

        Assert.False(file.Exists());

        ///Если файла не существует должны возвращаться значения по умолчанию
        settings!.Load(file);
        Assert.Equal("test_changed", settings.Value.Name);
        Assert.Equal(321, settings.Value.Value);

        ISettingsProvider<SubAppSettings>? subSetting = serviceProvider.GetRequiredService<ISettingsProvider<SubAppSettings>>();
        Assert.Equal("test", subSetting.Value.Name);
        Assert.Equal(123, subSetting.Value.Value);
        Assert.Equal("City", subSetting.Value.City);



        subSetting.Value.Name = "test_changed";
        subSetting.Value.Value = 3333;
        subSetting.Value.City = "City_change";
        var subRes = subSetting?.Save(file);

        Assert.True(subRes?.IsSuccess);

        var vv = subSetting!.Load(file);
        Assert.True(vv.IsSuccess);
        
        Assert.Equal("test_changed", subSetting.Value.Name);
        Assert.Equal(3333, subSetting.Value.Value);
        Assert.Equal("City_change", subSetting.Value.City);
    }
}


// простой вывод на консоль
public class AppSettings
{
    public string Name { get; set; } = "test";
    public int CurrPerson { get; set; } = 1;
    public int Value { get; set; } = 123;

    public List<string> List { get; set; } = ["List1", "List2", "List3"];
    public List<Person> Persons { get; set; } = [
        new(1,"Вася"),
        new(2,"Петя"),
        new(3,"Кирилл"),
        ];
}
    
public class SubAppSettings() : AppSettings
{
    ICalcDb _calcDb = null!;
    
    public SubAppSettings(ICalcDb calcDb) : this()
    {
        _calcDb = calcDb;
    }


    public string City { get; set; } = "City";
}

public class Person(int idx, string fio)
{
    public int Idx { get; set; } = idx;
    public string FIO { get; set; } = fio;
}

public interface ICalcDb
{
    Ex<string> LoadSettingFromDb(string settingName);

    Ex<bool> SaveSettingToDb(string name, string value, string? pswd = null);

    IEnumerable<Person> GetPersons();

}



[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(SubAppSettings))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}
