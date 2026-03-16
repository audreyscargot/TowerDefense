@echo off
setlocal enabledelayedexpansion

:: Define the output file name
set "outputFile=All_My_Code.txt"

:: If the output file already exists from a previous run, delete it so we start fresh
if exist "%outputFile%" del "%outputFile%"

echo Collecting all C# files...

:: Loop through every .cs file in this directory and all subdirectories
for /r %%f in (*.cs) do (
    
    :: Print the filename to the console so you can see what it's doing
    echo Adding: %%~nxf
    
    :: Append a header with the filename to the output text file
    echo ======================================================== >> "%outputFile%"
    echo FILE: %%~nxf >> "%outputFile%"
    echo PATH: %%~dpnxf >> "%outputFile%"
    echo ======================================================== >> "%outputFile%"
    echo. >> "%outputFile%"
    
    :: Append the actual code of the file
    type "%%f" >> "%outputFile%"
    
    :: Add some blank space before the next file
    echo. >> "%outputFile%"
    echo. >> "%outputFile%"
    echo. >> "%outputFile%"
)

echo.
echo Done! All code has been merged into %outputFile%
pause
