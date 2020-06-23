@echo off

if not exist cslabs.sln (
    dotnet new classlib -o utils
    del .\utils\Class1.cs

    dotnet new console -o testbed
    del .\testbed\Program.cs
    dotnet add .\testbed reference .\utils

    dotnet new sln
    dotnet sln add .\testbed
    dotnet sln add .\utils
)

cl /c /Fo.\tools\tools.obj .\tools\tools.c
link /dll /out:.\bin\tools.dll .\tools\tools.obj

dotnet build -o .\bin cslabs.sln

.\bin\testbed.exe
