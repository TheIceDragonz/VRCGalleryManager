@echo off
setlocal enabledelayedexpansion

title VRCGalleryManager - Universal Build Tool (Windows + Android)
color 0E

set "SCRIPT_DIR=%~dp0"
set "OUTPUT_ROOT=%SCRIPT_DIR%Output"
set "NO_PAUSE=1"
set "NO_EXPLORER=1"

echo ========================================================
echo        VRCGalleryManager - Universal Builder
echo ========================================================
echo.
echo  [1] Building Windows Standalone Executable and Installer...
echo.

call "%SCRIPT_DIR%Build_Windows_EXE.bat"
if %ERRORLEVEL% neq 0 (
    color 0C
    echo.
    echo [ERROR] Windows build failed with error code %ERRORLEVEL%.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================================
echo  [2] Building Android Release APK...
echo ========================================================
echo.

call "%SCRIPT_DIR%Build_Android_APK.bat"
if %ERRORLEVEL% neq 0 (
    color 0C
    echo.
    echo [ERROR] Android build failed with error code %ERRORLEVEL%.
    pause
    exit /b %ERRORLEVEL%
)

color 0A
echo.
echo ========================================================
echo  All Builds Complete! Check the 'Output' folder.
echo ========================================================
echo.
if exist "%OUTPUT_ROOT%" explorer "%OUTPUT_ROOT%"
pause
exit /b 0
