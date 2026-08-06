@echo off
cd "ELaunch"
echo Building ELaunch project...
dotnet clean
dotnet build --verbosity detailed
echo Build completed
pause