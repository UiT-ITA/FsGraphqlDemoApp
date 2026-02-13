using FsGraphqlDemo.GraphQL.Client.Model.StudenData;
namespace FsGraphqlDemo.GraphQL.Client.Model.StudenterGittFeideBrukeretData;

public class StudenterGittFeideBrukeretData
{
    //[JsonPropertyName("studenterGittFeideBrukere")]
    public required Studentergittfeidebrukere[] StudenterGittFeideBrukere { get; set; }
}

public class Studentergittfeidebrukere
{
    public required string Id { get; set; }
    public required string LanetakerId { get; set; }
    public required string Studentnummer { get; set; }
    public required Personprofil PersonProfil { get; set; }
    public required Bilde Bilde { get; set; }
    public required Studentkort[] Studentkort { get; set; }
}
