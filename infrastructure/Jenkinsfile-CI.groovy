pipeline {
    agent any

    stages {
        stage('Restore Dependencies') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet tool restore'
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet build --no-restore'
            }
        }
        stage('Test') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code -v ${PWD}/infrastructure/docker/dotnet/.nuget:/root/.nuget mcr.microsoft.com/dotnet/sdk:5.0-alpine dotnet test --no-build --verbosity normal'
            }
        }
    }
}
