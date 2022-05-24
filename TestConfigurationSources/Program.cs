// Install-Package Microsoft.Extensions.Configuration
// Install-Package Microsoft.Extensions.Configuration.EnvironmentVariables

using Microsoft.Extensions.Configuration;
using System.Reflection;
using Azure.Identity;


// https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/
// %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
// c:\Users\perh\AppData\Roaming\Microsoft\UserSecrets\4c197b73-0aa6-4ba8-8a5c-6acee93d12f5\secrets.json 

var configurations = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

#if DEBUG

var pgmversion = "1.3";

configurations
    .AddJsonFile("local.settings.json", optional: true) // Never check in (override .gitignore)
    .AddJsonFile($"local.{pgmversion}.json", optional: true) // Never check in (override .gitignore)
    .AddJsonFile(@"\\faelles\faelles\Midlertidig\perh\debugsettings.json", optional: true) // File-share locked with NTFS/AD rights
    .AddUserSecrets(assembly: Assembly.GetExecutingAssembly(), optional: true);

#endif

#if !DEBUG

    configurations.AddEnvironmentVariables();

#endif

var configuration1 = configurations.Build();

#region Azure AppConfiguration Service 

var credential = new VisualStudioCredential();
//new VisualStudioCredential()
//(Azure.Core.TokenCredential) new ManagedIdentityCredential()
//new DefaultAzureCredential()

// Test Azure AppConfiguration Service (possibly keyvault integrated):
// https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-integrate-azure-managed-service-identity
// https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme
// https://blog.novacare.no/azure-app-configuration-using-managed-identity/

var appconfigurationconnection = configuration1["AppConfigurationConnection"];

//configurations.AddAzureAppConfiguration(options => options.Connect(appconfigurationconnection));
configurations.AddAzureAppConfiguration(options =>
                    options.Connect(new Uri("https://perh-appconfig-ac.azconfig.io"), 
                    credential
                    ));

configuration1 = configurations.Build(); // Rebuild


#endregion 

// Show all settings

var x = configuration1.GetDebugView();

configuration1
    .AsEnumerable()
    .ToDictionary(c => c.Key, c => c.Value)
    .ToList()
    .ForEach(e => Console.WriteLine($"{e.Key}={e.Value}"));

Console.WriteLine("OK");