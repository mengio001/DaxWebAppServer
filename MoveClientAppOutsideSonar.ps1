# Define the current and new paths for ClientApp
$sourcePath = "C:\WorkspaceQuizTower\QuizTowerPlatform\QuizTowerPlatform.ClientApp"
$destinationPath = "C:\WorkspaceQuizTower\TempQuizTowerPlatformClientApp"

# Check if the source directory exists
if (Test-Path -Path $sourcePath) {
    # Create the destination path if it doesn't exist
    if (!(Test-Path -Path $destinationPath)) {
        New-Item -ItemType Directory -Path $destinationPath | Out-Null
    }

    # Move the ClientApp directory to the new location
    Move-Item -Path $sourcePath -Destination $destinationPath -Force

    Write-Host "ClientApp directory moved to $destinationPath"
} else {
    Write-Host "Source directory does not exist: $sourcePath"
    exit 1
}


# Move the ClientApp directory to the new location
$sourcePath = "C:\WorkspaceQuizTower\QuizTowerPlatform\QuizTowerPlatform.ClientApp"
$destinationPath = "C:\WorkspaceQuizTower\TempQuizTowerPlatformClientApp"

# Move the ClientApp directory back to its original location after the scan
$finalSourcePath = Join-Path -Path $destinationPath -ChildPath "QuizTowerPlatform.ClientApp"
if (Test-Path -Path $finalSourcePath) {
    ## This will create a new folder, this is not necessary!!
    # if (!(Test-Path -Path $sourcePath)) {
    #    New-Item -ItemType Directory -Path $sourcePath | Out-Null
    # }
    Move-Item -Path $finalSourcePath -Destination $sourcePath -Force
    Write-Output "ClientApp directory moved back to $sourcePath"
} else {
    Write-Output "Destination directory does not contain the ClientApp folder: $finalSourcePath"
    exit 1
}