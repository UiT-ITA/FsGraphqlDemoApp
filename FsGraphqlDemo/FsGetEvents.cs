using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

namespace FsGraphqlDemo;

public class FsGetEvents
{
    private readonly IServiceProvider _services;

    public FsGetEvents(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<int> Run(
        Verbosity verbosity,
        int maximum,
        string? filter,
        string? username,
        string? studentNumber,
        string? stedkode,
        CancellationToken cancellationToken)
    {
        IFsClient client = _services.GetRequiredService<IFsClient>();

        if (username is not null)
        {
            if (username.Length <= 7 || username[^7..] != "@uit.no")
            {
                username += "@uit.no";
            }
        }

        List<IOperationResult<IQueryStudentEventsResult>> results = [];
        string? after = null;
        int count = 0;
        int numCalls = 0;
        do
        {
            Console.WriteLine($"GetStudentEvents({numCalls++}) after={after} - Found {count}");
            var result = await client.QueryStudentEvents.ExecuteAsync("186", after, cancellationToken);
            after = result.Data?.Studenthendelser?.PageInfo?.EndCursor;
            results.Add(result);

            count += result.Data?.Studenthendelser?.Nodes?.Count ?? 0;

        } while (after is not null && count < maximum);

        ShowUserInfo(results, verbosity, maximum, username);
        Console.WriteLine($"Found {count} items");
        return 0;
    }

    private static void ShowUserInfo(
        List<IOperationResult<IQueryStudentEventsResult>> results,
        Verbosity verbosity,
        int maximum,
        string? username)
    {
        int count = 0;
        foreach (var result in results)
        {
            foreach (var item in result.Data?.Studenthendelser?.Nodes ?? [])
            {
                ++count;
                if (item?.Hendelsestype == "SEMESTERREGISTRERT")
                {
                    QueryStudentEvents_Studenthendelser_Nodes_StudentSemesterregistrert node
                        = (QueryStudentEvents_Studenthendelser_Nodes_StudentSemesterregistrert)item;

                    if (node.Student is null)
                        continue;

                    var student = node.Student;
                    ArgumentNullException.ThrowIfNull(student);
                    ArgumentNullException.ThrowIfNull(student.Navn);
                    ArgumentNullException.ThrowIfNull(student.PersonProfil);

                    if (username is not null && student.FeideBruker != username)
                        continue;

                    if (verbosity >= Verbosity.Detailed)
                    {
                        string name = $"{student.Navn.Fornavn} {student.Navn.Etternavn}";
                        Console.WriteLine($"[{count,4}] {name,-35}  Tid={node.Tidspunkt}  User={student.FeideBruker}"
                            + $"  Fnr={student.PersonProfil.Fodselsnummer}"
                            + $"  Snr={student.Studentnummer}"
                            + $"  Ltid={student?.LanetakerId}");
                        //+ $"  Id={student?.Id}");
                        foreach (var studentCard in student?.Studentkort ?? [])
                        {
                            ArgumentNullException.ThrowIfNull(studentCard);

                            Console.Write($"     * KortNr ");
                            var currentColor = Console.ForegroundColor;
                            if (studentCard.Status?.AktivtKort == false)
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            else
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(studentCard.Studentkortnummer);
                            Console.ForegroundColor = currentColor;
                            Console.WriteLine($"  Aktivt {studentCard.Status?.AktivtKort}"
                                + $"  Dato {studentCard.Gyldighetsperiode?.FraDato} - {studentCard.Gyldighetsperiode?.TilDato}");
                        }
                        Console.WriteLine($"");
                    }
                    if (count >= maximum)
                        break;
                }
            }
        }
    }
}
