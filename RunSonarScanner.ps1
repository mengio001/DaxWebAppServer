 # Move the ClientApp directory to the new location
$sourcePath = "C:\WorkspaceQuizTower\QuizTowerPlatform\QuizTowerPlatform.ClientApp"
$destinationPath = "C:\WorkspaceQuizTower\TempQuizTowerPlatformClientApp"

# Move ClientApp if it exists
if (Test-Path -Path $sourcePath) {
    if (!(Test-Path -Path $destinationPath)) {
        New-Item -ItemType Directory -Path $destinationPath | Out-Null
    }
    Move-Item -Path $sourcePath -Destination $destinationPath -Force
    Write-Output "ClientApp directory moved to $destinationPath"
} else {
    Write-Output "Source directory does not exist: $sourcePath"
    exit 1
}

# Define other SonarQube parameters as variables
$SonarProjectKey = "QuizTowerPlatform-API"
$SonarHostUrl = "http://localhost:9000"
$SonarToken = "sqp_b275e1dde2f99fdf966a312a841269b3bab071cc"
$ProjectBaseDir = "C:\WorkspaceQuizTower\QuizTowerPlatform"
$SonarVerbose = $false # $true  # Set verbosity

## Write-Output $false

cd $ProjectBaseDir
cls

# Set the exclusion filter
$env:SQExclusionFilter = '.*(\\QuizTowerPlatform.ClientApp\\|\\node_modules\\|provisioning|docker-compose.yml|sonar-project.properties).*'
Write-Output "Using exclusion filter: $env:SQExclusionFilter"

# Verbose flag for SonarScanner
$SonarVerboseFlag = if ($SonarVerbose) { "/d:sonar.verbose=true" } else { "" }

# Begin SonarScanner analysis with exclusions and verbosity
dotnet sonarscanner begin `
    /k:"$SonarProjectKey" `
    /d:sonar.projectBaseDir="$ProjectBaseDir" `
    /d:sonar.host.url="$SonarHostUrl" `
    /d:sonar.token="$SonarToken" `
    /d:sonar.sources="$ProjectBaseDir/QuizTowerPlatform.Api" `
    /d:sonar.exclusions="QuizTowerPlatform.ClientApp/**,provisioning/**,**/node_modules/**,**/dist/**,docker-compose.yml,sonar-project.properties" `
    /d:sonar.cs.opencover.reportsPaths="$ProjectBaseDir/QuizTowerPlatform.Api.Test/coverage.opencover.xml" `
    $SonarVerboseFlag

# Build and test
dotnet build "$ProjectBaseDir/QuizTowerPlatform.sln" -c Release
dotnet test "$ProjectBaseDir/QuizTowerPlatform.Api.Test/QuizTowerPlatform.Api.Test.csproj" `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput=./coverage.opencover.xml

# End SonarScanner analysis
dotnet sonarscanner end /d:sonar.token="$SonarToken"