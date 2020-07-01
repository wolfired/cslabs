@echo off

if not exist cslabs.sln (
    dotnet new classlib -o planter
    del .\planter\Class1.cs

    dotnet new classlib -o utils
    del .\utils\Class1.cs

    dotnet new console -o plantor
    del .\plantor\Program.cs
    dotnet add .\plantor reference .\planter
    dotnet add .\plantor package Mono.Options

    dotnet new console -o testbed
    del .\testbed\Program.cs
    dotnet add .\testbed reference .\utils

    dotnet new sln
    dotnet sln add .\planter
    dotnet sln add .\plantor
    dotnet sln add .\testbed
    dotnet sln add .\utils
)

if not exist .\bin (
    mkdir .\bin
)

cl /c /Fo.\tools\tools.obj .\tools\tools.c
link /dll /out:.\bin\tools.dll .\tools\tools.obj

dotnet build -o .\bin cslabs.sln
