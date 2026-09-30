@echo off
setlocal
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Upload.ps1"
set "CRAFT_EXIT=%ERRORLEVEL%"
pause
exit /b %CRAFT_EXIT%
