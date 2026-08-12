using FsGraphqlDemo.GraphQL.Client.Model;
using FsGraphqlDemo.GraphQL.Client.Model.StudenthendelserData;
using GraphQL;
using GraphQL.Client.Http;

namespace FsGraphqlDemo.GraphQL.Client.Queries;

internal class GetStudenthendelse(GraphQLHttpClient graphQLClient)
{
    public const string name = "GetStudentHendelser";
    public const string query = """
            query GetStudentHendelser($institusjon: String!, $after: String) {
              studenthendelser(
                filter: {
                  eierOrganisasjonskode: $institusjon
                  hendelsestype: [SEMESTERREGISTRERT]
                }
                first: 100
                after: $after
              ) {
                edges {
                  cursor
                  node {
                    tidspunkt
                    hendelsestype
                    ... on StudentSemesterregistrert {
                      id
                      hendelsestype
                      tidspunkt
                      student {
                        ...studentinfo
                      }
                    }
                  }
                }
                pageInfo {
                  hasPreviousPage
                  startCursor
                  endCursor
                  hasNextPage
                }
                totalCount
              }
            }

            fragment studentinfo on StudentVedLarested {
              id
              lanetakerId
              studentnummer
              personProfil {
                fodselsnummer
                feideBruker
                navn {
                  fornavn
                  etternavn
                }
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
            """;

    public async Task QueryStudentHendelser(string institusjon, CancellationToken cancellationToken)
    {
        var QueryGetStudentsGittFeideBrukere = new GraphQLRequest
        {
            Query = query,
            OperationName = name,
            Variables = new
            {
                institusjon,
            }
        };

        try
        {
            var graphQLResponse = await graphQLClient.SendQueryAsync<StudenthendelserData>(QueryGetStudentsGittFeideBrukere, cancellationToken);

            ArgumentNullException.ThrowIfNull(graphQLResponse, nameof(graphQLResponse));
            if (graphQLResponse.Errors != null)
            {
                foreach (var e in graphQLResponse.Errors)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (graphQLResponse.Data.Studenthendelser is null)
            {
                Console.WriteLine($"Feil ved {name} er <null>");
                return;
            }

            if (graphQLResponse.Data.Studenthendelser.Edges.Length == 0)
            {
                Console.WriteLine("Feil ved GetStudentInfo returnerte ingen");
            }

            Console.WriteLine($"Total count = {graphQLResponse.Data.Studenthendelser.TotalCount}");

            var list = graphQLResponse.Data.Studenthendelser.Edges.
                Select(e => e.Node)
                .OrderByDescending(n => n.Tidspunkt)
                .ToList();

            foreach (var student in list)
            {
                Console.WriteLine($"Studentnummer: {student.Student.Studentnummer}" +
                    $"  FeideBruker: {student.Student.PersonProfil.FeideBruker,16}" +
                    $"  Hendelse: {student.Hendelsestype}" +
                    $"  Tisdpunkt: {student.Tidspunkt}"
                    );
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
