REM @echo off

REM setlocal and !variable! instead of %variable% to 
REM prevent variable expansion from start
setlocal ENABLEDELAYEDEXPANSION

REM install npm packages
if not exist .\node_modules (
	npm i
)

REM Move 'dist' into dist-DD-MM-YYYY-HH-MM
echo Checking if .\dist exists

if exist .\dist (
    echo dist folder exists renaming.. 

    set _timepart=%time: =0%
    set _timepart=!_timepart:~0,5!
    set _timepart=!_timepart: =0!
    set _timepart=!_timepart::=-!

    set _datepart=%date:/=-%
    set _newfilename=dist-!_datepart!-!_timepart!

    echo Moving dist to !_newfilename!
    move dist !_newfilename!
) else (
    echo "No Existing dist folder
)

REM call ng build, ng build seems to kill the command process when it exits
REM give it its own proceess to prevent the bat file getting halted
cmd /c "cd %cd% && ng build -prod -aot"

REM remove existing files in deployment directory
pushd ..\aspnet-core\src\AbpCompanyName.AbpProjectName.Web.Host\wwwroot
dir .
echo %cd%

REM delete existing files
del *.chunk.js, *.bundle.js, *.bundle.css, *.woff, *.woff2, *.png, *.jpg, favicon.ico, *.ttf, *.svg, *.eot, *.html, 
rmdir /s /q assets

REM copy the dist files into the deployment folder
popd
xcopy .\dist ..\aspnet-core\src\AbpCompanyName.AbpProjectName.Web.Host\wwwroot /i /s /y

