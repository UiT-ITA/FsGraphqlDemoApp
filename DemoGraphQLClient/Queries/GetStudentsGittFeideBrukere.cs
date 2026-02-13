using FsGraphqlDemo.GraphQL.Client.Model.StudenterGittFeideBrukeretData;
using GraphQL;
using GraphQL.Client.Http;

namespace FsGraphqlDemo.GraphQL.Client.Queries;

public class GetStudentsGittFeideBrukere(GraphQLHttpClient graphQLClient)
{
    public const string name = "GetStudentsGittFeideBrukere";
    public const string query = """
            query GetStudentsGittFeideBrukere(
                $institusjon: String!
                $feideBrukere: [String!]!
            ) {
                studenterGittFeideBrukere(
                    eierOrganisasjonskode: $institusjon
                    feideBrukere: $feideBrukere
                ) {
                    id
                    lanetakerId
                    studentnummer
                    personProfil
                    {
                        navn {fornavn, etternavn}
                        fodselsnummer
                        feideBruker
                    }
                    bilde {
                        id
                        bilde
                        filtype
                    }
                    studentkort {
                        id
                        studentkortnummer
                        status {
                            id
                            kode
                            aktivtKort
                        }
                        gyldighetsperiode {
                            fraDato
                            tilDato
                        }
                    }
                }
            }
            """;

    public async Task QueryStudentGittFeideBrukere(string institusjon, string[] feideBrukere)
    {
        var QueryGetStudentsGittFeideBrukere = new GraphQLRequest
        {
            Query = query,
            OperationName = name,
            Variables = new
            {
                institusjon,
                feideBrukere,
            }
        };

        try
        {
            var graphQLResponse = await graphQLClient.SendQueryAsync<StudenterGittFeideBrukeretData>(QueryGetStudentsGittFeideBrukere);

            //ArgumentNullException.ThrowIfNull(graphQLResponse, nameof(graphQLResponse));
            if (graphQLResponse.Errors != null)
            {
                foreach (var e in graphQLResponse.Errors)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (graphQLResponse.Data.StudenterGittFeideBrukere is null)
            {
                Console.WriteLine($"Feil ved {name} er <null>");
                return;
            }

            if (graphQLResponse.Data.StudenterGittFeideBrukere.Length == 0)
            {
                Console.WriteLine("Feil ved GetStudentInfo returnerte ingen");
            }

            foreach (var student in graphQLResponse.Data.StudenterGittFeideBrukere)
            {
                if (student is null)
                {
                    Console.WriteLine($"Student not found");
                }
                else
                {
                    Console.WriteLine($"Studentnummer: {student.Studentnummer}, FeideBruker: {student.PersonProfil.FeideBruker}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Feil under GraphQL query");
            Console.WriteLine(e.Message);
            throw;
        }
    }

}
