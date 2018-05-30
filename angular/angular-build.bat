REM @echo off

REM setlocal and !variable! instead of %variable% to 
REM prevent variable expansion from start
setlocal ENABLEDELAYEDEXPANSION

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

