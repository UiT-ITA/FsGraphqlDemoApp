using System.CommandLine;

namespace FsGraphqlDemo;

public class CommandLineParser
{
    private readonly IServiceProvider _services;

    public CommandLineParser(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<int> CommandLineParse(string[] args, CancellationToken cancellationToken)
    {
        var verbosityOption = new Option<Verbosity>(
               name: "--verbosity",
               getDefaultValue: () => Verbosity.Normal,
               description: "Verbosity level: Quiet, Normal, Detailed");

        var endpointOption = new Option<string>(
            "--endpoint",
            "Which API endpoint to use, prod or test")
                .FromAmong("prod", "test");

        var maximumOption = new Option<int>(
            name: "--maximum",
            getDefaultValue: () => int.MaxValue,
            description: "Maximum number of rows to process");

        var filterOption = new Option<string>(
            name: "--filter",
            description: "Employee Number / Fnr (11 digit Norwegian fødselsnummer) to search for");

        var userNameOption = new Option<string>(
            name: "--user",
            description: "User Name to search for");

        var usersArg = new Argument<string>(
            name: "Users to search for",
            description: "Username of users to search for. Use comma to separate users");

        var studentNumberOption = new Option<string>(
            name: "--student",
            description: "Student number to search for");

        var stedkodeOption = new Option<string>(
            name: "--stedkode",
            description: "Specific stedkode to search for. Default is to search for all users");

        var rootCommand = new RootCommand("Command line program to run FS GraphQL queries");

        var studentsCommand = new Command("students", "Query all students from FS");
        studentsCommand.AddOption(verbosityOption);
        studentsCommand.AddOption(endpointOption);
        studentsCommand.AddOption(maximumOption);
        studentsCommand.AddOption(filterOption);
        studentsCommand.AddOption(userNameOption);
        studentsCommand.AddOption(stedkodeOption);
        rootCommand.AddCommand(studentsCommand);

        var feideCommand = new Command("feide", "Query for specific students using feideId");
        feideCommand.AddOption(verbosityOption);
        feideCommand.AddOption(endpointOption);
        feideCommand.AddOption(maximumOption);
        feideCommand.AddOption(filterOption);
        feideCommand.AddArgument(usersArg);
        rootCommand.AddCommand(feideCommand);

        var semesterRegCommand = new Command("semreg", "Query semesterRegistreringer for students");
        semesterRegCommand.AddOption(verbosityOption);
        semesterRegCommand.AddOption(endpointOption);
        semesterRegCommand.AddOption(maximumOption);
        semesterRegCommand.AddOption(filterOption);
        semesterRegCommand.AddOption(userNameOption);
        semesterRegCommand.AddOption(studentNumberOption);
        semesterRegCommand.AddOption(stedkodeOption);
        rootCommand.AddCommand(semesterRegCommand);

        var eventsCommand = new Command("events", "Query studentHendelser for students");
        eventsCommand.AddOption(verbosityOption);
        eventsCommand.AddOption(endpointOption);
        eventsCommand.AddOption(maximumOption);
        eventsCommand.AddOption(filterOption);
        eventsCommand.AddOption(userNameOption);
        rootCommand.AddCommand(eventsCommand);

        var checkSemregCommand = new Command("checkSemreg", "Check all Semesterregistreringer against studentHendelser");
        checkSemregCommand.AddOption(verbosityOption);
        checkSemregCommand.AddOption(endpointOption);
        checkSemregCommand.AddOption(maximumOption);
        checkSemregCommand.AddOption(filterOption);
        checkSemregCommand.AddOption(userNameOption);
        rootCommand.AddCommand(checkSemregCommand);

        studentsCommand.SetHandler(async (verbosity, maximum, fnr, username, stedkode) =>
        {
            FsGetStudents getStudents = new(_services);
            await getStudents.Run(verbosity, maximum, fnr, username, stedkode, cancellationToken);
        },
        verbosityOption, maximumOption, filterOption, userNameOption, stedkodeOption);

        feideCommand.SetHandler(async (verbosity, maximum, fnr, users, stedkode) =>
        {
            FsGetStudentsUsingFeide getStudents = new(_services);
            await getStudents.Run(verbosity, maximum, fnr, users, stedkode, cancellationToken);
        },
        verbosityOption, maximumOption, filterOption, usersArg, stedkodeOption);

        semesterRegCommand.SetHandler(async (verbosity, maximum, fnr, username, studentNumber, stedkode) =>
        {
            FsGetSemesterRegistreringer query = new(_services);
            await query.Run(verbosity, maximum, fnr, username, studentNumber, stedkode, cancellationToken);
        },
        verbosityOption, maximumOption, filterOption, userNameOption, studentNumberOption, stedkodeOption);

        eventsCommand.SetHandler(async (verbosity, maximum, fnr, username, studentNumber, stedkode) =>
        {
            FsGetEvents query = new(_services);
            await query.Run(verbosity, maximum, fnr, username, studentNumber, stedkode, cancellationToken);
        },
        verbosityOption, maximumOption, filterOption, userNameOption, studentNumberOption, stedkodeOption);

        checkSemregCommand.SetHandler(async (verbosity, maximum, fnr, username, studentNumber, stedkode) =>
        {
            FsCheckSemesterregistreringerAndEvents query = new(_services);
            await query.Run(verbosity, maximum, fnr, username, studentNumber, stedkode, cancellationToken);
        },
        verbosityOption, maximumOption, filterOption, userNameOption, studentNumberOption, stedkodeOption);

        return await rootCommand.InvokeAsync(args);
    }
}
