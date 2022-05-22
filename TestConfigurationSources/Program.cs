// Install-Package Microsoft.Extensions.Configuration
// Install-Package Microsoft.Extensions.Configuration.EnvironmentVariables

using Microsoft.Extensions.Configuration;
using System.Reflection;

// https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/
// %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
// c:\Users\perh\AppData\Roaming\Microsoft\UserSecrets\4c197b73-0aa6-4ba8-8a5c-6acee93d12f5\secrets.json 

var configurations = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

#if DEBUG

configurations
    .AddJsonFile("local.settings.json", optional: true) // Never check in (override .gitignore)
    .AddJsonFile(@"\\faelles\faelles\Midlertidig\perh\debugsettings.json", optional: true) // File-share locked with NTFS/AD rights
    .AddUserSecrets(assembly: Assembly.GetExecutingAssembly(), optional: true);

#endif

#if !DEBUG

    configurations.AddEnvironmentVariables();

#endif

var configurationroot = configurations.Build();

#region Azure AppConfiguration Service 
// Test Azure AppConfiguration Service (possibly keyvault integrated):

var appconfigurationconnection = configurationroot["AppConfigurationConnection"];

var configurationroot2 = new ConfigurationBuilder()
                .AddAzureAppConfiguration(options => options.Connect(appconfigurationconnection))
                .Build();

configurationroot.Bind(configurationroot2);
#endregion 

// Show all settings

var x = configurationroot.GetDebugView();

configurationroot
    .AsEnumerable()
    .ToDictionary(c => c.Key, c => c.Value)
    .ToList()
    .ForEach(e => Console.WriteLine($"{e.Key}={e.Value}"));

Console.WriteLine("OK");