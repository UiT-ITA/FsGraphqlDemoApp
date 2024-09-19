using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace FsGraphqlDemo;

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
        string? filter,
        string? username,
        string? stedkode,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Verbosity: {verbosity}");

        IFsClient client = _services.GetRequiredService<IFsClient>();

        Stopwatch stopWatch = new();
        stopWatch.Start();

        List<IStudentinfo> results = [];
        string? after = null;
        int count = 0;
        do
        {
            Console.WriteLine($"GetStudents({count}) - Found = {results.Count}");
            var result = await client.QueryStudents.ExecuteAsync("186", after, cancellationToken);
            after = result.Data?.Studenter?.PageInfo?.EndCursor;

            var studentList = result.Data?.Studenter?.Nodes;
            if (studentList is not null)
            {
                foreach (var student in studentList)
                {
                    var studentInfo = student as IStudentinfo;
                    if (studentInfo is not null)
                    {
                        results.Add(studentInfo);
                    }
                }
            }
            await Task.Delay(100, cancellationToken);
        } while (after is not null && count++ < maximum);

        stopWatch.Stop();
        Console.WriteLine($" - GetStudents: Elapsed time {stopWatch.Elapsed.TotalSeconds:f2}s");
        ShowInfo.ShowUserInfo(results, filter);

        return 0;
    }
}

