pipeline {
    agent any

    stages {
        stage('Restore Dependencies') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code mcr.microsoft.com/dotnet/sdk:5.0 dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code mcr.microsoft.com/dotnet/sdk:5.0 dotnet build --no-restore'
            }
        }
        stage('Test') {
            steps {
                sh 'docker run --rm -v ${PWD}:/code -w /code mcr.microsoft.com/dotnet/sdk:5.0 dotnet test --no-build --verbosity normal'
            }
        }
    }
}
