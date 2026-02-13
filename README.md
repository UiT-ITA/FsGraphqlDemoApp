# FsGraphqlDemoApp

Demo application to show how to use the FS GraphQL API using the **Strawberry Shake** and **GraphQL.Client** libraries.

1. StrawberryShake

Automatically generates a *strongly typed* client from a GraphQL *schema*, 
which helps reduce errors and improve developer experience. 
It is developed by ChilliCream, the creators of the popular Hot Chocolate GraphQL server framework for .NET.

2. GraphQL.Client

A highly customizable and versatile client library that supports both System.Text.Json and Newtonsoft.Json serializers. 
It allows developers to construct queries *manually* using the GraphQLQuery class or raw strings 
and send them via GraphQLHttpClient

## Applications

This solution contains two console applications that demonstrates how to use the FS GraphQL API.

1.	DemoStrawberryShake uses the StrawberryShake library.

	*[Strawberry Shake](https://chillicream.com/docs/strawberryshake) 
	is a GraphQL client library for .NET that generates strongly-typed C# code based on your GraphQL schema. 
	It provides a high-level API for making GraphQL queries and mutations, 
	and it handles the serialization and deserialization of data for you.
	Strawberry Shake also supports features like caching, error handling, and real-time updates with subscriptions.*
	
2. DemoGraphQLClient uses the GraphQL.Client library.

   *[GraphQL.Client](https://github.com/graphql-dotnet/graphql-client)
   A GraphQL Client for .NET Standard over HTTP*


## Subscribe to the FS GraphQL API

You need API keys from the API Manager (Gravitee) to access the API.  
The applications supports both the TEST and the PROD endpoints of the FS GraphQL API.

1. Go to [Applications](https://api-uit.intark.uh-it.no/applications/mine)
2. Create a new application or reuse an existing application
3. Name the application, I suggest using your username as a prefix: eg: *tle001-application-name*
4. Go to [API catalog](https://api-uit.intark.uh-it.no/catalog/all). Search for "FS GraphQL API"
5. Use the "Subscribe" button and choose the plan "LES1-TEST Plan" or "LES1-PROD Plan". Press "Next"
6. Choose you application. Press "Next"
7. Press "Validate the request"

You will find your API keys under your application, submenu "subscriptions".
Click on the API and you will se your API key.


