using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;
using System.Diagnostics;

namespace DemoStrawberryShake;

public class FsGetStudents
{
    private readonly IServiceProvider _services;

    public FsGetStudents(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<int> Run(
        Verbosity verbosity,
        int maximum,
        string? fnr,
        string? username,
        string? studentNumber,
        string? studentId,
        string? stedkode,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Verbosity: {verbosity}");

        IFsClient client = _services.GetRequiredService<IFsClient>();

        var termin = DateTime.UtcNow.Month < 8 ? "VÅR" : "HØST";
        var arstall = DateTime.UtcNow.Year;

        var terminData = await client.QueryTerminer.ExecuteAsync(Settings.FsInstitusjon, [arstall], cancellationToken);
        var terminId = terminData.Data?.Terminer?.Nodes?.Where(x => x?.Betegnelse?.Kode == termin).Select(x => x?.Id).First();
        if (terminId is null)
        {
            Console.WriteLine("TerminId er null");
            return 0;
        }

        // TODO
        // A quick test of getting one emne
        EmneIkkeUtloptITerminTerminbetegnelse terminkodeBetegnelse = termin == "HØST"
            ? EmneIkkeUtloptITerminTerminbetegnelse.Host
            : EmneIkkeUtloptITerminTerminbetegnelse.Var;
        var emnerData = await client.QueryEmner.ExecuteAsync(Settings.FsInstitusjon, 100, null, ["PSY-2900"], 2026, terminkodeBetegnelse, cancellationToken);
        Console.WriteLine(emnerData.Data?.EmnerV2?.TotalCount);
        Console.WriteLine(emnerData.Data?.EmnerV2?.Nodes.Count);

        foreach (var emneFrag in emnerData.Data?.EmnerV2?.Nodes ?? [])
        {
            var emne = emneFrag as IFragmentEmne;
            Console.WriteLine($"Emne: {emne?.Id} - {emne?.Kode} - {emne?.Praksistype?.Navn}");
        }

        Stopwatch stopWatch = new();
        stopWatch.Start();

        List<IStudentinfo> studentsFound = [];
        string? after = null;
        bool hasNextPage;
        int count = 0;
        Console.WriteLine($"QueryStudent for termin {terminId}");
        do
        {
            var result = await client.QueryStudents.ExecuteAsync(Settings.FsInstitusjon, after, cancellationToken);
            result.EnsureNoErrors();

            after = result.Data?.Studenter?.PageInfo?.EndCursor;
            hasNextPage = result.Data?.Studenter?.PageInfo?.HasNextPage == true;
            var studentList = result.Data?.Studenter?.Nodes;

            //hasNextPage = false;
            Console.WriteLine($"GetStudents({count}) - Found = {studentsFound.Count} (total={result.Data?.Studenter?.TotalCount})  hasNextPage={hasNextPage}");

            if (studentList is not null)
            {
                foreach (var student in studentList)
                {
                    var studentInfo = student as IStudentinfo;
                    if (studentInfo is not null)
                    {
                        studentsFound.Add(studentInfo);
                    }
                }
            }
            await Task.Delay(100, cancellationToken);
        } while (hasNextPage && count++ < maximum);

        stopWatch.Stop();
        Console.WriteLine($" - GetStudents: Elapsed time {stopWatch.Elapsed.TotalSeconds:f2}s");
        ShowInfo.ShowUserInfo(studentsFound, fnr, studentId);

        return 0;
    }
}

