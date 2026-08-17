@echo off
setlocal enabledelayedexpansion

title VRCGalleryManager - Universal Build Tool (Windows + Android)
color 0E

echo ========================================================
echo        VRCGalleryManager - Universal Builder
echo ========================================================
echo.
echo  [1] Building Windows Standalone Executable...
echo.

call "%~dp0Build_Windows_EXE.bat"

echo.
echo ========================================================
echo  [2] Building Android Release APK...
echo ========================================================
echo.

call "%~dp0Build_Android_APK.bat"

color 0A
echo.
echo ========================================================
echo  All Builds Complete! Check the 'Output' folder.
echo ========================================================
echo.
pause
exit /b 0
