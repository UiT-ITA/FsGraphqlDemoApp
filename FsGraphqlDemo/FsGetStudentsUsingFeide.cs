using Microsoft.Extensions.DependencyInjection;

namespace FsGraphqlDemo;

public class FsGetStudentsUsingFeide
{
    private readonly IServiceProvider _services;

    public FsGetStudentsUsingFeide(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<int> Run(
        Verbosity verbosity,
        int maximum,
        string? filter,
        string? users,
        string? stedkode,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Verbosity: {verbosity}");

        IFsClient client = _services.GetRequiredService<IFsClient>();

        List<string> userList = [];
        // Split users string into list of users
        users?.Split(',').ToList().ForEach(x => userList.Add(x + "@uit.no"));

        Console.WriteLine($"GetStudentsUsingFeide()");
        userList.ForEach(Console.WriteLine);

        List<IStudentcardinfo> results = [];
        var result1 = await client.QueryStudentsUsingFeide.ExecuteAsync("186", userList, cancellationToken);

        var studentCardList = result1.Data?.StudenterGittFeideBrukere;
        if (studentCardList is not null)
        {
            foreach (var item in studentCardList)
            {
                var studentCardInfo = item as IStudentcardinfo;
                if (studentCardInfo is not null)
                {
                    results.Add(studentCardInfo);
                }
            }
        }

        ShowInfo.ShowUserInfo2(results);
        return 0;
    }
}
