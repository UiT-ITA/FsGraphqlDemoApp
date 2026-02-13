namespace FsGraphqlDemo.GraphQL.Client.Model.EmnerData;

public class EmnerData
{
    public required EmnerV2 EmnerV2 { get; set; }
}

public class EmnerV2
{
    public required Node[] Nodes { get; set; }
    public required int TotalCount { get; set; }
    public required Pageinfo PageInfo { get; set; }
}

public class Pageinfo
{
    public required bool HasPreviousPage { get; set; }
    public required bool HasNextPage { get; set; }
    public required string StartCursor { get; set; }
    public required string EndCursor { get; set; }
}

public class Node
{
    public required string Id { get; set; }
    public required string Kode { get; set; }
    public required string Versjonskode { get; set; }
    public required Navnallesprak NavnAlleSprak { get; set; }
    public required Praksistype Praksistype { get; set; }
    public required Studieansvarligorganisasjonsenhet StudieansvarligOrganisasjonsenhet { get; set; }
    public required Administrativtansvarligorganisasjonsenhet AdministrativtAnsvarligOrganisasjonsenhet { get; set; }
    public required Campuser[] Campuser { get; set; }
}

public class Navnallesprak
{
    public required string Nb { get; set; }
}

public class Praksistype
{
    public required string Id { get; set; }
    public required string Kode { get; set; }
    public required Navn Navn { get; set; }
}

public class Navn
{
    public required string Nb { get; set; }
}

public class Studieansvarligorganisasjonsenhet
{
    public required string Id { get; set; }
    public required Fakultet Fakultet { get; set; }
    public required string Gruppenummer { get; set; }
    public required string Instituttnummer { get; set; }
}

public class Fakultet
{
    public required string Fakultetsnummer { get; set; }
}

public class Administrativtansvarligorganisasjonsenhet
{
    public required string Id { get; set; }
    public required Fakultet1 Fakultet { get; set; }
    public required string Gruppenummer { get; set; }
    public required string Instituttnummer { get; set; }
}

public class Fakultet1
{
    public required string Fakultetsnummer { get; set; }
}

public class Campuser
{
    public required Gyldighetsperiode Gyldighetsperiode { get; set; }
    public required Campus Campus { get; set; }
}

public class Gyldighetsperiode
{
    public string? FraDato { get; set; }
    public string? TilDato { get; set; }
}

public class Campus
{
    public required string Kode { get; set; }
}
