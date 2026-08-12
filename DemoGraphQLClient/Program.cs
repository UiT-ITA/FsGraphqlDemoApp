using DemoGraphQLClient.Model;
using FsGraphqlDemo.GraphQL.Client.Queries;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.Configuration;

// Get secrets id from project

// Load connection string from user secrets
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddUserSecrets("2edf0ef1-9d2a-4b00-8bd1-66eeee979eb0")
    .Build();

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

using var graphQLClient = new GraphQLHttpClient(baseUrl, new SystemTextJsonSerializer());
graphQLClient.HttpClient.DefaultRequestHeaders.Add("X-Gravitee-Api-Key", apiKey);
graphQLClient.HttpClient.DefaultRequestHeaders.Add("Feature-Flags", "beta,experimental");

if (args.Length == 0)
{
    Console.WriteLine("Please provide a query to execute: GetEmner, GetStudentsGittFeideBrukere, or GetStudentHendelser");
    return;
}

CancellationTokenSource cts = new();
CancellationToken cancellationToken = cts.Token;

if (args[0] == "GetEmner")
{
    var queryGetEmner = new GetEmner(graphQLClient);
    await queryGetEmner.QueryEmner(Settings.FsInstitusjon, null, 2026, "VAR", cancellationToken);
}
else if (args[0] == "GetStudentsGittFeideBrukere")
{
    var queryGetStudent = new GetStudentsGittFeideBrukere(graphQLClient);
    string[] feideBrukere = ["tle001@uit.no", "xyz9988@uit.no"];
    await queryGetStudent.QueryStudentGittFeideBrukere(Settings.FsInstitusjon, feideBrukere, cancellationToken);
}
else if (args[0] == "GetStudentHendelser")
{
    var queryGetStudentHendelser = new GetStudenthendelse(graphQLClient);
    await queryGetStudentHendelser.QueryStudentHendelser(Settings.FsInstitusjon, cancellationToken);
}
else
{
    Console.WriteLine("Unknown query. Please provide a valid query to execute: GetEmner, GetStudentsGittFeideBrukere, or GetStudentHendelser");
}