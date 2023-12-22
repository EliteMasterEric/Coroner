REM Copy ../Art/icon.png to the current directory
copy /y ..\Art\icon.png .
REM Copy ../Art/manifest.json to the current directory
copy /y ..\Art\manifest.json .
REM Copy ../README.md to the current directory
copy /y ..\README.md .
REM Copy ../CHANGELOG.md to the current directory
copy /y ..\CHANGELOG.md .
REM Copy all files from ../Coroner/build/bin/Debug to the current directory
xcopy /s /y /q ..\Coroner\build\bin\Debug\* .\BepInEx\plugins\
REM Copy Strings_* files from ../Coroner to the current directory, excluding Strings_test.xml
xcopy /s /y /q ..\LanguageData\* .\BepInEx\Lang\Coroner\

REM Create a zip file named Coroner.zip containing all files (except build.bat and Strings_test.xml) in the current directory
"C:\Program Files\7-Zip\7z.exe" a Coroner.zip * -x!build.bat -x!Coroner.zip -x!BepInEx\Lang\Coroner\Strings_test.xml

