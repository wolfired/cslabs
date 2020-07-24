@echo off

if not exist cslabs.sln (
    dotnet new sln

    dotnet new classlib -o planter
    del .\planter\Class1.cs
    dotnet sln add .\planter

    dotnet new classlib -o utils
    del .\utils\Class1.cs
    dotnet sln add .\utils

    dotnet new console -o plantor
    del .\plantor\Program.cs
    dotnet add .\plantor reference .\planter
    dotnet add .\plantor package Mono.Options
    dotnet sln add .\plantor

    dotnet new console -o testbed
    del .\testbed\Program.cs
    dotnet add .\testbed reference .\utils
    dotnet sln add .\testbed

    dotnet new console -o svn_exporter
    del .\svn_exporter\Program.cs
    dotnet add .\svn_exporter package Mono.Options
    dotnet sln add .\svn_exporter
)

if not exist .\bin (
    mkdir .\bin
)
