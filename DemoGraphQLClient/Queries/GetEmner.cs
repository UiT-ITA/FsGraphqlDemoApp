using FsGraphqlDemo.GraphQL.Client.Model.EmnerData;
using GraphQL;
using GraphQL.Client.Http;

namespace FsGraphqlDemo.GraphQL.Client.Queries;

internal class GetEmner(GraphQLHttpClient graphQLClient)
{
    public const string name = "QueryEmner";
    public const string query = """
        query QueryEmner(
            $institusjon: String!
            $first: Int
            $after: String
            $emnekode: [String!]
            $arstall: Int!
            $terminbetegnelse: EmneIkkeUtloptITerminTerminbetegnelse!
        ) {
            emnerV2(
                filter: {
                    eierOrganisasjonskode: $institusjon
                    emnekoder: $emnekode
                    ikkeUtloptITermin: {
                        arstall: $arstall
                        terminbetegnelse: $terminbetegnelse
                    }
                }
                first: $first
                after: $after
            ) {
                nodes {
                    ...FragmentEmne
                }
                totalCount
                pageInfo {
                    hasPreviousPage
                    hasNextPage
                    startCursor
                    endCursor
                }
            }
        }
        fragment FragmentEmne on Emne {
            id
            kode
            versjonskode
            navnAlleSprak {
                nb
            }
            praksistype {
                id
                kode
                navn {
                    nb
                }
            }
            studieansvarligOrganisasjonsenhet {
                id
                fakultet {fakultetsnummer}
                gruppenummer
                instituttnummer
            }
            administrativtAnsvarligOrganisasjonsenhet {
                id
                fakultet {fakultetsnummer}
                gruppenummer
                instituttnummer
            }
            campuser {
                gyldighetsperiode {
                    tilTermin {
                        arstall
                        betegnelse {
                            kode
                        }
                    }
                }
                campus {
                    kode
                }
            }
        }
        """;

    public int CountEmnerMedPraksis { get; set; }
    public string? EndCursor { get; set; }
    public bool HasNextPage { get; private set; }
    public int QueryCount { get; set; } = 1000;

    public async Task QueryEmner(string institusjon, string[]? emnekode, int arstall, string terminbetegnelse, CancellationToken cancellationToken)
    {
        do
        {
            bool status = await DoQuery(institusjon, QueryCount, EndCursor, emnekode, arstall, terminbetegnelse, cancellationToken);
        } while (HasNextPage);
        Console.WriteLine($"Antall emner med praksis: {CountEmnerMedPraksis}");
    }

    private async Task<bool> DoQuery(string institusjon, int count, string? endCursor, string[]? emnekode, int arstall, string terminbetegnelse, CancellationToken cancellationToken)
    {
        var QueryGetEmner = new GraphQLRequest
        {
            Query = query,
            OperationName = name,
            Variables = new
            {
                institusjon,
                first = QueryCount,
                after = endCursor,
                emnekode,
                arstall,
                terminbetegnelse
            }
        };

        try
        {
            var graphQLResponse = await graphQLClient.SendQueryAsync<EmnerData>(QueryGetEmner, cancellationToken);

            ArgumentNullException.ThrowIfNull(graphQLResponse, nameof(graphQLResponse));
            if (graphQLResponse.Errors != null)
            {
                foreach (var e in graphQLResponse.Errors)
                {
                    Console.WriteLine(e.Message);
                }
                return false;
            }

            if (graphQLResponse.Data.EmnerV2 is null)
            {
                Console.WriteLine($"Feil ved {name} er <null>");
                return false;
            }

            Console.WriteLine($"Count = {graphQLResponse.Data.EmnerV2.Nodes.Length}" +
                $" Total count = {graphQLResponse.Data.EmnerV2.TotalCount}" +
                $" HasNextPage: {graphQLResponse.Data.EmnerV2.PageInfo.HasNextPage}" +
                $" Start: {graphQLResponse.Data.EmnerV2.PageInfo.StartCursor}" +
                $" End: {graphQLResponse.Data.EmnerV2.PageInfo.EndCursor}");
            EndCursor = graphQLResponse.Data.EmnerV2.PageInfo.EndCursor;
            HasNextPage = graphQLResponse.Data.EmnerV2.PageInfo.HasNextPage;

            if (graphQLResponse.Data.EmnerV2.Nodes.Length == 0)
            {
                Console.WriteLine("Feil ved GetEmner returnerte ingen");
            }

            // Select the relevant nodes
            var list = graphQLResponse.Data.EmnerV2.Nodes
                .Where(e => e.Praksistype is not null)
                .ToList();

            foreach (var emne in list)
            {
                Console.WriteLine($"Emne: {emne.Kode}  PraksisType: {emne.Praksistype.Kode,12}  Navn: {emne.NavnAlleSprak.Nb}");
                ++CountEmnerMedPraksis;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Feil under GraphQL query");
            Console.WriteLine(e.Message);
            throw;
        }

        return true;
    }
}
