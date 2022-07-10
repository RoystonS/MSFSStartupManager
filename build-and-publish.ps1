if (Test-Path -Path .\MSFSStartupManager\bin\Release) {
	Remove-Item -Path .\MSFSStartupManager\bin\Release -Recurse
}

dotnet publish -p:PublishProfile=FolderProfile -p:Configuration=Release
$path = Resolve-Path "MSFSStartupManager\bin\Release\net6.0-windows\publish\win-x86\MSFSStartupManager.exe"

if (Test-Path -Path Release) {
	Remove-Item -Path Release -Recurse
}

mkdir Release
Copy-Item -Path "$path" -Destination "Release"

$o = [system.diagnostics.fileversioninfo]::GetVersionInfo($path)
Write-Output "Better Bravo Lights $($o.ProductVersion)" | Out-File -Encoding ASCII Release\VERSION.txt
$filename = "MSFSStartupManager$($o.ProductVersion).zip"

Compress-Archive -Path "Release\*" -Force -DestinationPath "$filename"
