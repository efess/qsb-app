@echo off
echo Registering VS9 path
REM SET PATH ""
mkdir Deploy
CALL "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat"
echo Compiling...
aspnet_compiler -f -u -v / -p QServersBrowser Deploy
echo Deleting unecessary files

rmdir /S /Q Deploy\Debug 
rmdir /S /Q Deploy\Database 
rmdir /S /Q Deploy\Images
pause