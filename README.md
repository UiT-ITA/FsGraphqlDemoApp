# FsGraphqlDemoApp

## Simple application to show how to use the FS GraphQl API using the Strawberry Shake library

## Build instructions

Edit "appsettings.json" and add your API key

Build the FS GraphQL client
> dotnet graphql update -x X-Gravitee-Api-Key=<API-KEY>

Build the project
> dotnet build 

## How to run

Help screen
> .\bin\Debug\net8.0\FsGraphqlDemo.exe --help


Search FS for all students (takes a long time)
> .\bin\Debug\net8.0\FsGraphqlDemo.exe students


Search FS for all events of type SEMESTERREGISTRERT
> .\bin\Debug\net8.0\FsGraphqlDemo.exe events


Search FS for students by username (comma separated list when searching for multiple students)
> .\bin\Debug\net8.0\FsGraphqlDemo.exe feide username1,username2


Search FS for semesterRegistreringer this semster
> .\bin\Debug\net8.0\FsGraphqlDemo.exe semreg


Search FS for semesterregistreringer and check them against the student events (experimental)
> .\bin\Debug\net8.0\FsGraphqlDemo.exe checkSemreg
