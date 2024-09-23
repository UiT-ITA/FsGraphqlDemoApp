# FsGraphqlDemoApp

## Simple application to show how to use the FS GraphQl API using the Strawberry Shake library

## Subscribe to the API

You need API keys from the API Manager (Gravitee) to access the API.  
The application supports both the TEST and the PROD endpoints of the FS GraphQL API.

1. Go to [Applications](https://api-uit.intark.uh-it.no/applications/mine)
2. Create a new appliction (if you have an application you can reuse it)
3. Name the application using your username as a prefix: eg: *tle001-fsgraphql-test*
4. Go to [API catalog](https://api-uit.intark.uh-it.no/catalog/all). Search for "FS GraphQL API"
5. Use the "Subscribe" button and choose the plan "Test: Read access". Press "Next"
6. Choose you application. Press "Next"
7. Press "Validate the request"

You will find your API keys under your application, submenu "subscriptions"
Click on the API and you will se your API key.


## Build instructions

Edit "appsettings.json" and add your API key. Make sure you don't commit the API key to github

Download GraphQL schema and create the Strawberryshake client  
```
dotnet graphql update -x X-Gravitee-Api-Key=API-KEY
```


Build the project
```
dotnet build
```

## How to run

### Help screen
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe --help
```


### Search FS for all students (takes a long time)
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe students  
```


### Search FS for all events of type SEMESTERREGISTRERT
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe events  
```


### Search FS for students by username (comma separated list when searching for multiple students)
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe feide username1,username2  
```


### Search FS for semesterRegistreringer this semster
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe semreg  
```


### Search FS for semesterregistreringer and check them against the student events (experimental)
```
.\bin\Debug\net8.0\FsGraphqlDemo.exe checkSemreg  
```

### Switch FS endpoints

Use the flag --endpoint to switch between TESt and PROD endpoints. TEST is the default.

```
.\bin\Debug\net8.0\FsGraphqlDemo.exe feide tle001 --endpoint prod
```