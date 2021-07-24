Set-Location -Path "AutoPick"
dotnet publish -c Release
Set-Location -Path ".."
Copy-Item "AutoPick\bin\Release\net5.0-windows\win-x64\publish\AutoPick.exe" -Destination "."