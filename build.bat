@echo off

cl /c /Fo.\tools\tools.obj .\tools\tools.c
link /dll /out:.\bin\tools.dll .\tools\tools.obj

dotnet build -o .\bin cslabs.sln
