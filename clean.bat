@echo off

if exist .\testbed\bin (
    rmdir /S /Q .\testbed\bin
)
if exist .\testbed\obj (
    rmdir /S /Q .\testbed\obj
)
if exist .\testbed\testbed.csproj (
    del .\testbed\testbed.csproj
)

if exist .\utils\bin (
    rmdir /S /Q .\utils\bin
)
if exist .\utils\obj (
    rmdir /S /Q .\utils\obj
)
if exist .\utils\utils.csproj (
    del .\utils\utils.csproj
)

if exist cslabs.sln (
    del cslabs.sln
)