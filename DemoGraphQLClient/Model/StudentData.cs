using System.Text.Json.Serialization;

namespace FsGraphqlDemo.GraphQL.Client.Model.StudenData;

public class StudentData
{
    public required string Id { get; set; }
    public required string LanetakerId { get; set; }
    public required string Studentnummer { get; set; }
    public required Personprofil PersonProfil { get; set; }
    public required Bilde Bilde { get; set; }
    public required Studentkort[] Studentkort { get; set; }
}

public class Personprofil
{
    public required string FeideBruker { get; set; }
    public required string Fodselsnummer { get; set; }
    public required Navn Navn { get; set; }
}

public class Navn
{
    public required string Fornavn { get; set; }
    public required string Etternavn { get; set; }
}

public class Bilde
{
    public required string Id { get; set; }

    [JsonPropertyName("bilde")]
    public required string BildeValue { get; set; }
    public required string Filtype { get; set; }
}

public class Studentkort
{
    public required string Id { get; set; }
    public required string Studentkortnummer { get; set; }
    public required Status Status { get; set; }
    public required Gyldighetsperiode Gyldighetsperiode { get; set; }
}

public class Status
{
    public required string Id { get; set; }
    public required string Kode { get; set; }
    public required bool AktivtKort { get; set; }
}

public class Gyldighetsperiode
{
    public required string FraDato { get; set; }
    public required string TilDato { get; set; }
}
