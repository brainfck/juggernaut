@echo off

REM You don't need to use this file, but, it can be useful.
REM The runnable file
set "exec_location=[EXE location]"

set "exec_path=%exec_location%\juggernaut.exe"

REM The directory to store the images in. Don't use quotes here.
set "image_path=[IMAGE_LOCATION]"

"%exec_path%" "%image_path%"