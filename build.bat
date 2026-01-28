@echo off
cd "E-Launchpad"
echo Building E-Launchpad project...
dotnet clean
dotnet build --verbosity detailed
echo Build completed
pause