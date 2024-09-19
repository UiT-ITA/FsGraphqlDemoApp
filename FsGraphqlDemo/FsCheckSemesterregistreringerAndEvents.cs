using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

namespace FsGraphqlDemo;

public class FsCheckSemesterregistreringerAndEvents
{
    private readonly IServiceProvider _services;

    public FsCheckSemesterregistreringerAndEvents(IServiceProvider services)
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

        var allSemregs = await GetAllSemesterregistreringer(maximum, studentNumber, client, cancellationToken);
        var allEvents = await GetAllEvents(maximum, client, cancellationToken);

        // Check each semesterregistrering if there is a corresponding event
        int count = 0;
        foreach (var semreg in allSemregs)
        {
            foreach (var edge1 in semreg.Data?.Semesterregistreringer?.Edges ?? [])
            {
                var student = edge1?.Node?.Student;
                if (student is not null)
                {
                    if (!FindEventForStudent(allEvents, student))
                        count++;

                    //        var event1 = allEvents.FirstOrDefault(
                    //x => x.Data?.Studenthendelser?.Edges?.Any(y => y?.Node?.S Student?.Studentnummer == student.Studentnummer) ?? false);
                }
            }
        }
        Console.WriteLine($"Total not found {count}");
        return 0;
    }

    private static bool FindEventForStudent(
        List<IOperationResult<IQueryStudentEventsResult>> allEvents,
        IQuerySemesterRegistreringer_Semesterregistreringer_Edges_Node_Student student)
    {
        bool found = false;
        foreach (var event1 in allEvents)
        {
            foreach (var genericNode in event1.Data?.Studenthendelser?.Nodes ?? [])
            {
                if (genericNode?.Hendelsestype == "SEMESTERREGISTRERT")
                {
                    QueryStudentEvents_Studenthendelser_Nodes_StudentSemesterregistrert node
                        = (QueryStudentEvents_Studenthendelser_Nodes_StudentSemesterregistrert)genericNode;

                    if (node.Student?.Studentnummer == student.Studentnummer)
                    {
                        found = true;
                        //Console.WriteLine($"Found event for student {student.Studentnummer}");
                    }
                }
            }
        }
        if (!found)
        {
            Console.WriteLine($"No event found for student Snr {student.Studentnummer} Feide {student?.FeideBruker} - {student?.Navn?.Fornavn} {student?.Navn?.Etternavn}");
        }
        return found;
    }

    private static async Task<List<IOperationResult<IQuerySemesterRegistreringerResult>>> GetAllSemesterregistreringer(int maximum, string? studentNumber, IFsClient client, CancellationToken cancellationToken)
    {
        List<IOperationResult<IQuerySemesterRegistreringerResult>> results = [];

        string? after = null;
        int count = 0;
        List<string> studentNumberList = [];
        if (studentNumber is not null)
        {
            // Split studentnumber string into list of studentnumbers
            var num = studentNumber.Split(',');
            for (int i = 0; i < num.Length; i++)
            {
                studentNumberList.Add(num[i]);
            }
            maximum = studentNumberList.Count;
        }

        SemesterregistreringsterminInput termin = new()
        {
            Arstall = DateTime.Now.Year,
            Betegnelse = DateTime.Now.Month < 7
                ? Semesterregistreringsterminbetegnelse.Var
                : Semesterregistreringsterminbetegnelse.Host
        };

        do
        {
            studentNumberList.ForEach(Console.WriteLine);
            Console.WriteLine($"GetSemesterRegistreringer({count})");
            var result = await client.QuerySemesterRegistreringer.ExecuteAsync("186", [termin], studentNumberList, after, cancellationToken);
            after = result.Data?.Semesterregistreringer?.PageInfo?.EndCursor;
            if (result is not null)
                results.Add(result);

            count += result?.Data?.Semesterregistreringer?.Edges?.Count ?? 0;

        } while (after is not null && count < maximum);

        return results;
    }

    private static async Task<List<IOperationResult<IQueryStudentEventsResult>>> GetAllEvents(int maximum, IFsClient client, CancellationToken cancellationToken)
    {
        List<IOperationResult<IQueryStudentEventsResult>> results = new();
        string? after = null;
        int count = 0;
        do
        {
            Console.WriteLine($"GetStudentEvents({count})");
            var result = await client.QueryStudentEvents.ExecuteAsync("186", after, cancellationToken);
            after = result.Data?.Studenthendelser?.PageInfo?.EndCursor;
            results.Add(result);

            count += result.Data?.Studenthendelser?.Nodes?.Count ?? 0;

        } while (after is not null && count < maximum);
        return results;
    }
}
