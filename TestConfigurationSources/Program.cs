// Install-Package Microsoft.Extensions.Configuration
// Install-Package Microsoft.Extensions.Configuration.EnvironmentVariables

using Microsoft.Extensions.Configuration;
using System.Reflection;

// https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/
// %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
// c:\Users\perh\AppData\Roaming\Microsoft\UserSecrets\4c197b73-0aa6-4ba8-8a5c-6acee93d12f5\secrets.json 

var configurationroot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("local.settings.json", optional: true)
                .AddJsonFile("aa.json", optional: true)
                .AddJsonFile(@"q:\Midlertidig\perh\debugsettings.json", optional: true)
                .AddUserSecrets(assembly: Assembly.GetExecutingAssembly(), optional: true)
                //.AddEnvironmentVariables()
                .Build();

var appconfigurationconnection = configurationroot["AppConfigurationConnection"];

var configurationroot2 = new ConfigurationBuilder()
                .AddAzureAppConfiguration(options => options.Connect(appconfigurationconnection))
                .Build();

configurationroot.Bind(configurationroot2);

//var x = configurationroot.GetDebugView();

configurationroot
    .AsEnumerable()
    .ToDictionary(c => c.Key, c => c.Value)
    .ToList()
    .ForEach(e => Console.WriteLine($"{e.Key}={e.Value}"));

Console.WriteLine("OK");