using DemoStrawberryShake;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets("d7b464be-c667-4934-8d1a-43b8015027b0");

var config = builder.Build();

//
// Check command line arguments early to configure the http client
//
string name = "Test";
for (var i = 0; i < args.Length - 1; i++)
{
    if (args[i] == "--endpoint" && args[i + 1] == "prod")
    {
        name = "Prod";
    }
}
var baseUrl = config[$"FsGraphql{name}:baseUrl"];
var apiKey = config[$"FsGraphql{name}:apiKey"];

if (string.IsNullOrEmpty(apiKey) || apiKey == "ADD-API-KEY")
{
    Console.WriteLine("""Please add your API key to the "appsettings.json" file or your user secrets""");
    return;
}

ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));
ArgumentNullException.ThrowIfNull(apiKey, nameof(apiKey));

Console.WriteLine($"""Use FsGraphQl API {name} endpoint: "{baseUrl}" with api key "{apiKey[..3]}...{apiKey[^3..]}" """);

// Set up the FS client for GraphQl
var serviceCollection = new ServiceCollection();
serviceCollection
    .AddFsClient()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("X-Gravitee-Api-Key", apiKey);
        client.DefaultRequestHeaders.Add("Feature-flags", "beta,experimental");
    });

IServiceProvider services = serviceCollection.BuildServiceProvider();

// Start the program
CommandLineParser commandLineParser = new(services);
await commandLineParser.CommandLineParse(args, CancellationToken.None);
