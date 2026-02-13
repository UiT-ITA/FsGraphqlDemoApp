# DemoStrawberryShake 
Simple application to show how to use the FS GraphQl API using the Strawberry Shake library.
The application supports searching FS for
- students
- events
- semesterregistreringer

It also has an experimental feature to check semesterregistreringer against student events.

# Build instructions

### API KEY
Add the API key to the application. You can do this in two ways:
1. Edit "appsettings.json" and add your API key.  
   *NB: Make sure you don't commit the API key to github.*
2. Add the API KEY to the user secrets file. 

### Client

Download latest GraphQL schema from SIKT and create the Strawberryshake client  
```
curl -o .\schema.graphql https://api.fellesstudentsystem.no/specs/schema-exp.graphql
```


Build the project
```
dotnet build
```

## Usage

### Help screen
```
dotnet run -- --help

or

.\bin\Debug\net10.0\FsGraphqlDemo.exe --help
```


### Search FS for all students (takes a long time)
```
.\bin\Debug\net10.0\FsGraphqlDemo.exe students  
```


### Search FS for all events of type SEMESTERREGISTRERT
```
.\bin\Debug\net10.0\FsGraphqlDemo.exe events  
```


### Search FS for students by username (comma separated list when searching for multiple students)
```
.\bin\Debug\net10.0\FsGraphqlDemo.exe feide username1,username2  
```


### Search FS for semesterRegistreringer this semster
```
.\bin\Debug\net10.0\FsGraphqlDemo.exe semreg  
```


### Search FS for semesterregistreringer and check them against the student events (experimental)
```
.\bin\Debug\net10.0\FsGraphqlDemo.exe checkSemreg  
```

### Switch FS endpoints

Use the flag --endpoint to switch between TESt and PROD endpoints. TEST is the default.

```
.\bin\Debug\net10.0\FsGraphqlDemo.exe feide tle001 --endpoint prod
```