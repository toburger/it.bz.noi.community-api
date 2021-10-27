# it.bz.noi.community-api

This microservice acts as a proxy service over the Dynamics 365 API provided to NOI Technology by Würth Phoenix.

This API uses the OData standard: https://docs.microsoft.com/en-us/odata/

A test installation can be found here --> https://api.community.noi.testingmachine.eu/

## Table of contents

- [Getting started](#Getting-started)
  - [Prerequisites](#Prerequisistes)
  - [Source code](#Source-code)
  - [Environment variables](#Environment-variables)
- [Development](#Development)
  - [From the command line](#From-the-command-line)
  - [With an IDE](#With-an-IDE)
  - [Local testing](#Local-testing)
- [Deployment](#Deployment)

## Getting started

### Prerequisites

To build the project, the following prerequisites must be met:

- [.NET 5.0](https://get.dot.net)

### Source code

Get a clone of this repository

`git clone https://github.com/noi-techpark/it.bz.noi.community-api`

### Environment variables

The following environment variables are needed in ordert to get the lambda function to work:

| Key              | Value                                           |
| -------------    | ----------------------------------------------- |
| CLIENT_ID        | <CLIENT_ID provided by Würth Phoenix>           |
| CLIENT_SECRET    | <CLIENT_SECRET provided by Würth Phoenix>       |
| TENANT_ID        | <TENANT_ID provided by Würth Phoenix>           |
| SERVICE_URL      | https://noitest.crm4.dynamics.com/api/data/v9.2 |
| SERVICE_SCOPE    | https://noitest.crm4.dynamics.com/.default      |
| OPENID_AUTHORITY | <Authentication Server Realm URL>               |

***Question**: The SERVICE_URL and SERVICE_SCOPE (...noitest...) will change in the future?*

## Development

The project can be developed purely with a basic editor in combination with the dotnet CLI.

### From the command line

Notable commands are:

- `dotnet tool restore`:
  restores the CLI tools
  
- `dotnet restore`:
  restores the project dependencies
  
- `dotnet build`:
  run a build
  
- `dotnet run -p ./it.bz.noi.community-api/`
  
  run the microservice
  
- `dotnet watch run -p ./it.bz.noi.community-api/`

  watch and run the microservice, if a change is detected it automatically reloads the executable

- `dotnet test`:
  run the tests
  
- `dotnet watch test -p ./it.bz.noi.community-api.Tests/`:
  watch and run tests after a file gets modified

### With an IDE

You can develop the app with your coding editor of your choice that supports C#. For example to code with Visual Studio Code:

- Start VS Code, e.g. by launching `code .` from the command line.
- Install the `ms-dotnettools.csharp` VS Code extension for C# coding support (if not already installed).
- The editor tooling should now work and you can start to code.

## Deployment

See how the [`odh-api-core` project ](https://github.com/noi-techpark/odh-api-core/) gets deployed. The only notable difference is that the `Dockerfile` and `docker compose.yml` files are located at the root of the directory.

