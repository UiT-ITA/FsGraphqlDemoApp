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

var baseUrl = config["FsGraphQLClient:BaseUrl"];
ArgumentNullException.ThrowIfNull(baseUrl, "FsGraphQLClient configuration not found in user secrets.");


using var graphQLClient = new GraphQLHttpClient(config["FsGraphQLClient:BaseUrl"], new SystemTextJsonSerializer());
graphQLClient.HttpClient.DefaultRequestHeaders.Add("X-Gravitee-Api-Key", config["FsGraphQLClient:ApiKey"]);
graphQLClient.HttpClient.DefaultRequestHeaders.Add("Feature-Flags", config["FsGraphQLClient:Feature-Flags"]);

if (args.Length == 0)
{
    Console.WriteLine("Please provide a query to execute: GetEmner, GetStudentsGittFeideBrukere, or GetStudentHendelser");
    return;
}

if (args[0] == "GetEmner")
{
    var queryGetEmner = new GetEmner(graphQLClient);
    await queryGetEmner.QueryEmner("186", null, 2026, "VAR");
}
else if (args[0] == "GetStudentsGittFeideBrukere")
{
    var queryGetStudent = new GetStudentsGittFeideBrukere(graphQLClient);
    string[] feideBrukere = ["tle001@uit.no", "xyz9988@uit.no"];
    await queryGetStudent.QueryStudentGittFeideBrukere("186", feideBrukere);
}
else if (args[0] == "GetStudentHendelser")
{
    var queryGetStudentHendelser = new GetStudenthendelse(graphQLClient);
    await queryGetStudentHendelser.QueryStudentHendelser("186");
}
else
{
    Console.WriteLine("Unknown query. Please provide a valid query to execute: GetEmner, GetStudentsGittFeideBrukere, or GetStudentHendelser");
}