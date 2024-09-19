using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

namespace FsGraphqlDemo;

public class FsGetSemesterRegistreringer
{
    private readonly IServiceProvider _services;

    public FsGetSemesterRegistreringer(IServiceProvider services)
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
        SemesterregistreringsterminInput termin = new()
        {
            Arstall = DateTime.Now.Year,
            Betegnelse = DateTime.Now.Month < 7
                        ? Semesterregistreringsterminbetegnelse.Var
                        : Semesterregistreringsterminbetegnelse.Host
        };

        List<IOperationResult<IQuerySemesterRegistreringerResult>> results = [];
        string? after = null;
        int count = 0;
        int numCalls = 0;
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
        do
        {
            studentNumberList.ForEach(Console.WriteLine);
            Console.WriteLine($"GetSemesterRegistreringer({numCalls++}) - Found = {count}");
            var result = await client.QuerySemesterRegistreringer.ExecuteAsync("186", [termin], studentNumberList, after, cancellationToken);
            result.EnsureNoErrors();
            after = result.Data?.Semesterregistreringer?.PageInfo?.EndCursor;

            results.Add(result);
            count += result.Data?.Semesterregistreringer?.Edges?.Count ?? 0;

        } while (after is not null && count < maximum);

        ShowUserInfo(results, maximum);
        return 0;
    }

    private static void ShowUserInfo(List<IOperationResult<IQuerySemesterRegistreringerResult>> results, int maximum)
    {
        int count = 0;
        foreach (var result in results)
        {
            foreach (var edge in result.Data?.Semesterregistreringer?.Edges ?? [])
            {
                ++count;
                var student = edge?.Node?.Student;
                ArgumentNullException.ThrowIfNull(student);
                ArgumentNullException.ThrowIfNull(student.Navn);
                ArgumentNullException.ThrowIfNull(student.PersonProfil);

                string name = $"{student.Navn.Fornavn} {student.Navn.Etternavn}";
                Console.WriteLine($"[{count,4}] {name,-35}  User={student.FeideBruker}"
                    + $"  Fnr={student.PersonProfil.Fodselsnummer}"
                    + $"  Snr={student.Studentnummer}"
                    + $"  Ltid={student?.LanetakerId}");
                foreach (var studentCard in edge?.Node?.Student.Studentkort ?? [])
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
                if (count >= maximum)
                    break;
            }
        }
    }
}
