@echo off

REM Copy ../README.md to the current directory
copy /y ..\README.md .

REM Copy ../CHANGELOG.md to the current directory
copy /y ..\CHANGELOG.md .

REM Copy all files from ../Coroner/build/bin/Debug to the current directory
xcopy /s /y /q ..\Coroner\build\bin\Debug\* .\

REM Copy all files from ../LanguageData to the current directory
xcopy /s /y /q ..\LanguageData\* .\BepInEx\config\EliteMasterEric-Coroner\

REM Create a zip file named Coroner.zip containing all files (except build.bat and Coroner.zip) in the current directory
"C:\Program Files\7-Zip\7z.exe" a -r Coroner.zip * -x!build.bat -x!Coroner.zip

REM Delete everything else in the current working directory except Coroner.zip, build.bat, icon.png, and manifest.json
for %%I in (*) do if not "%%I"=="Coroner.zip" if not "%%I"=="build.bat" if not "%%I"=="icon.png" if not "%%I"=="manifest.json" del /q "%%I"
for /d %%D in (*) do if not "%%D"=="Coroner.zip" if not "%%D"=="build.bat" if not "%%I"=="icon.png" if not "%%I"=="manifest.json" rd /s /q "%%D"