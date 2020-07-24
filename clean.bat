@echo off

call :clean_project planter
call :clean_project plantor
call :clean_project testbed
call :clean_project utils
call :clean_project svn_exporter

if exist .\bin (
    rmdir /S /Q .\bin
)

if exist cslabs.sln (
    del cslabs.sln
)

exit /b %errorlevel%

:clean_project
    if exist .\%~1\bin (
        rmdir /S /Q .\%~1\bin
    )
    if exist .\%~1\obj (
        rmdir /S /Q .\%~1\obj
    )
    if exist .\%~1\%~1.csproj (
        del .\%~1\%~1.csproj
    )
exit /b 0
