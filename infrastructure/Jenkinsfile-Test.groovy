pipeline {
    agent any


    environment {
        AWS_ACCESS_KEY_ID = credentials('AWS_ACCESS_KEY_ID')
        AWS_SECRET_ACCESS_KEY = credentials('AWS_SECRET_ACCESS_KEY')
        AWS_DEFAULT_REGION = "eu-west-1"

        CLIENT_ID = credentials('noi-community-api-test-client-id')
        CLIENT_SECRET = credentials('noi-community-api-test-client-secret')
        TENANT_ID = credentials('noi-community-api-test-tenant-id')
        SERVICE_URL = "https://noitest.crm4.dynamics.com/api/data/v9.2"
        SERVICE_SCOPE = "https://noitest.crm4.dynamics.com/.default"
    }

    stages {
        stage('Restore Dependencies') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget -v ${PWD}/infrastructure/docker/dotnet:/root/.dotnet mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet tool restore'
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget -v ${PWD}/infrastructure/docker/dotnet:/root/.dotnet mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget -v ${PWD}/infrastructure/docker/dotnet:/root/.dotnet mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet build --no-restore'
            }
        }
        stage('Test') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget -v ${PWD}/infrastructure/docker/dotnet:/root/.dotnet mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet test --no-build --verbosity normal'
            }
        }
        stage('Deploy') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget -v ${PWD}/infrastructure/docker/dotnet:/root/.dotnet mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet lambda deploy-serverless it-bz-noi-community-api-test -sb it-bz-noi-community-api-test -pl ./src/it.bz.noi.community-api/'
                sh 'aws lambda update-function-configuration --function-name it-bz-noi-community-api-test --environment "Variables={CLIENT_ID=${CLIENT_ID},CLIENT_SECRET=${CLIENT_SECRET},TENANT_ID=${TENANT_ID},SERVICE_URL=${SERVICE_URL},SERVICE_SCOPE=${SERVICE_SCOPE}}"'
            }
        }
    }
}
