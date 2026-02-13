using DemoStrawberryShake;
using System.CommandLine;

namespace DemoStrawberryShake;

public class CommandLineParser
{
    private readonly IServiceProvider _services;

    public CommandLineParser(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<int> CommandLineParse(string[] args, CancellationToken cancellationToken)
    {
        Option<Verbosity> verbosityOption = new("--verbosity", "-v")
        {
            Description = "Verbosity level: Quiet, Normal, Detailed",
            DefaultValueFactory = parseResult => Verbosity.Normal,
        };

        var endpointOption = new Option<string>("--endpoint")
        {
            Description = "Which API endpoint to use, prod or test",
        };
        endpointOption.Validators.Add(result =>
        {
            var val = result.GetValue(endpointOption) ?? "";
            if (!val.Equals("prod", StringComparison.OrdinalIgnoreCase) &&
                !val.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                result.AddError("Wrong API endpoint");
            }
        });

        var maximumOption = new Option<int>("--maximum")
        {
            Description = "Maximum number of rows to process",
            DefaultValueFactory = parseResult => int.MaxValue

        };

        var filterOption = new Option<string>("--filter")
        {
            Description = "Employee Number / Fnr (11 digit Norwegian fødselsnummer) to search for",
        };

        var userNameOption = new Option<string>("--user")
        {
            Description = "User Name to search for",
        };

        var usersArg = new Argument<string>("Users to search for")
        {
            Description = "Username of users to search for. Use comma to separate users",

        };

        var studentNumberOption = new Option<string>("--student")
        {
            Description = "Student number to search for",
        };

        var stedkodeOption = new Option<string>("--stedkode")
        {
            Description = "Specific stedkode to search for. Default is to search for all users",
        };

        var rootCommand = new RootCommand("Command line program to run FS GraphQL queries");

        var studentsCommand = new Command("students", "Query all students from FS");
        studentsCommand.Options.Add(verbosityOption);
        studentsCommand.Options.Add(endpointOption);
        studentsCommand.Options.Add(maximumOption);
        studentsCommand.Options.Add(filterOption);
        studentsCommand.Options.Add(userNameOption);
        studentsCommand.Options.Add(stedkodeOption);
        rootCommand.Subcommands.Add(studentsCommand);

        var feideCommand = new Command("feide", "Query for specific students using feideId");
        feideCommand.Options.Add(verbosityOption);
        feideCommand.Options.Add(endpointOption);
        feideCommand.Options.Add(maximumOption);
        feideCommand.Options.Add(filterOption);
        feideCommand.Arguments.Add(usersArg);
        rootCommand.Subcommands.Add(feideCommand);

        var semesterRegCommand = new Command("semreg", "Query semesterRegistreringer for students");
        semesterRegCommand.Options.Add(verbosityOption);
        semesterRegCommand.Options.Add(endpointOption);
        semesterRegCommand.Options.Add(maximumOption);
        semesterRegCommand.Options.Add(filterOption);
        semesterRegCommand.Options.Add(userNameOption);
        semesterRegCommand.Options.Add(studentNumberOption);
        semesterRegCommand.Options.Add(stedkodeOption);
        rootCommand.Subcommands.Add(semesterRegCommand);

        var eventsCommand = new Command("events", "Query studentHendelser for students");
        eventsCommand.Options.Add(verbosityOption);
        eventsCommand.Options.Add(endpointOption);
        eventsCommand.Options.Add(maximumOption);
        eventsCommand.Options.Add(filterOption);
        eventsCommand.Options.Add(userNameOption);
        rootCommand.Subcommands.Add(eventsCommand);

        var checkSemregCommand = new Command("checkSemreg", "Check all Semesterregistreringer against studentHendelser");
        checkSemregCommand.Options.Add(verbosityOption);
        checkSemregCommand.Options.Add(endpointOption);
        checkSemregCommand.Options.Add(maximumOption);
        checkSemregCommand.Options.Add(filterOption);
        checkSemregCommand.Options.Add(userNameOption);
        rootCommand.Subcommands.Add(checkSemregCommand);

        studentsCommand.SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            FsGetStudents getStudents = new(_services);
            return getStudents.Run(
                parseResult.GetValue(verbosityOption),
                parseResult.GetValue(maximumOption),
                parseResult.GetValue(filterOption),
                parseResult.GetValue(userNameOption),
                parseResult.GetValue(stedkodeOption),
                cancellationToken);
        });


        feideCommand.SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            FsGetStudentsUsingFeide getStudents = new(_services);
            return getStudents.Run(
                parseResult.GetValue(verbosityOption),
                parseResult.GetValue(maximumOption),
                parseResult.GetValue(filterOption),
                parseResult.GetValue(usersArg),
                parseResult.GetValue(stedkodeOption),
                cancellationToken);
        });

        semesterRegCommand.SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            FsGetSemesterRegistreringer query = new(_services);
            return query.Run(
                parseResult.GetValue(verbosityOption),
                parseResult.GetValue(maximumOption),
                parseResult.GetValue(filterOption),
                parseResult.GetValue(userNameOption),
                parseResult.GetValue(studentNumberOption),
                parseResult.GetValue(stedkodeOption),
                cancellationToken);
        });

        eventsCommand.SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            FsGetEvents query = new(_services);
            return query.Run(
                parseResult.GetValue(verbosityOption),
                parseResult.GetValue(maximumOption),
                parseResult.GetValue(filterOption),
                parseResult.GetValue(userNameOption),
                parseResult.GetValue(studentNumberOption),
                parseResult.GetValue(stedkodeOption),
                cancellationToken);
        });

        checkSemregCommand.SetAction((ParseResult parseResult, CancellationToken token) =>
        {
            FsCheckSemesterregistreringerAndEvents query = new(_services);
            return query.Run(
                parseResult.GetValue(verbosityOption),
                parseResult.GetValue(maximumOption),
                parseResult.GetValue(filterOption),
                parseResult.GetValue(userNameOption),
                parseResult.GetValue(studentNumberOption),
                parseResult.GetValue(stedkodeOption),
                cancellationToken);
        });

        return await rootCommand.Parse(args).InvokeAsync(cancellationToken: cancellationToken);
    }
}
