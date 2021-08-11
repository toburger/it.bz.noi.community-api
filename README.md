# it.bz.noi.community-api

* [Getting started](#Getting-started)
    * [Prerequisites](#Prerequisistes)
    * [Source code](#Source-code)
* [Development](#Development)
    * [Local testing](#Local-testing)
* [Deployment](#Deployment)

## Getting started

### Prerequisites

To build the project, the following prerequisites must be met:

* [.NET 5.0](https://get.dot.net)

### Source code

Get a clone of this repository

`git clone https://github.com/noi-techpark/it.bz.noi.community-api`

## Development

The project can be developed purely with a basic editor in combination with the dotnet CLI.

### From the command line

Notable commands are:

* `dotnet tool restore`:
  restores the CLI tools
* `dotnet restore`:
  restores the project dependencies
* `dotnet build`:
  run a build
* `dotnet test`:
  run the tests
* `dotnet watch test -p ./test/it.bz.noi.community-api.Tests/`:
  watch and run tests after a file gets modified

### With an IDE

You can develop the app with your coding editor of your choice that supports C#. For example to code with Visual Studio Code:

* Start VS Code, e.g. by launching `code .` from the command line.
* Install the `ms-dotnettools.csharp` VS Code extension for C# coding support (if not already installed).
* The editor tooling should now work and you can start to code.

### Local testing

As the AWS documentation states the best way to test the functionality of the app is by writing unit tests then run `dotnet test` (https://aws.amazon.com/de/blogs/developer/net-5-aws-lambda-support-with-container-images/).

I am not convinced that this is a time efficient way to develop and test the functionality so I will try to get it to work via docker-compose and an iterative development approach. Documentation will follow.

## Deployment

To deploy the image from your own machine make sure to configure an AWS named profile (https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-profiles.html).

Then run the following commands:

```sh
dotnet tool restore
```

This restores the lambda command line tool needed for deployment.

Then run the lambda serverless deployment tool:

```sh
dotnet lambda deploy-serverless it-bz-noi-community--api -sb it-bz-noi-community--api -pl ./src/it.bz.noi.community-api/
```
