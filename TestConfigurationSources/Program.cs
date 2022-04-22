// Install-Package Microsoft.Extensions.Configuration
// Install-Package Microsoft.Extensions.Configuration.EnvironmentVariables

using Microsoft.Extensions.Configuration;
using System.Reflection;

// https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/

var configurationroot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("local.settings.json", optional: true)
                .AddJsonFile(@"q:\Midlertidig\perh\debugsettings.json", optional: true)
                .AddUserSecrets(assembly: Assembly.GetExecutingAssembly(), optional: true)
                //.AddEnvironmentVariables()
                .Build();

//foreach (var setting in configurationroot.GetChildren())
//    Console.WriteLine($"{setting.Key}: {setting.Value}");

Console.WriteLine("-".PadRight(20,'-'));

var appconfigurationconnection = configurationroot["AppConfigurationConnection"];

var configurationroot2 = new ConfigurationBuilder()
                .AddAzureAppConfiguration(options => options.Connect(appconfigurationconnection))
                .Build();

configurationroot.Bind(configurationroot2);

//var x = configurationroot.GetDebugView();

foreach (var setting in configurationroot.GetChildren())
    Console.WriteLine($"{setting.Key}: {setting.Value}");

Console.WriteLine("OK");