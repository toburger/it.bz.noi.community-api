# it.bz.noi.community-api

## Getting started

* Clone the repository
* Get .NET 5.0 via get.dot.net
* Run `dotnet tool restore` to restore the lambda CLI
* Run `dotnet restore` to restore the dependencies
* Run `dotnet build` to run a build

## Development

You can develop the app with your coding editor of your choice that supports C#. For example to code with VS Code:

* Start VS Code, e.g. by launching `code .` from the command line.
* Install the `ms-dotnettools.csharp` VS Code extension for C# coding support.

## Local testing

As the AWS documentation states the best way to test the functionality of the app is by writing unit tests then run `dotnet test` (https://aws.amazon.com/de/blogs/developer/net-5-aws-lambda-support-with-container-images/).

I am not convinced that this is a time efficient way to develop and test the functionality so I will try to get it to work via docker-compose and an iterative development approach. Documentation will follow.

## Deploy

To deploy the application the best way is to configure a CI system that execute a deployment on every push to a special `release` tag.

To deploy the image from your own machine make sure to configure an AWS named profile (https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-profiles.html).
Then run the following command:

```
dotnet lambda deploy-serverless it-bz-noi-community--api -sb it-bz-noi-community--api -pl ./src/it.bz.noi.community-api/
```

This should also work as a continuous deployment task.
