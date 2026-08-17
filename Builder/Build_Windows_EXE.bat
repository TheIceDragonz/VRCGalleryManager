@echo off
setlocal enabledelayedexpansion

title VRCGalleryManager - Windows Build Tool
color 0B

echo ========================================================
echo        VRCGalleryManager - Windows EXE Builder
echo ========================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "ROOT_DIR=%SCRIPT_DIR%.."
set "OUTPUT_DIR=%SCRIPT_DIR%Output\Windows"
set "CSPROJ=%ROOT_DIR%\VRCGalleryManager.csproj"

:: 1. Check .NET SDK
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    color 0C
    echo [ERROR] .NET SDK was not found in PATH!
    echo Please install .NET 8 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [*] Checking .NET version...
dotnet --version

echo.
echo [*] Cleaning previous build output...
if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)
mkdir "%OUTPUT_DIR%"

echo.
echo [*] Publishing VRCGalleryManager for Windows (x64 Release, Self-Contained)...
echo     Target: net8.0-windows10.0.19041.0
echo     Output: %OUTPUT_DIR%
echo.

dotnet publish "%CSPROJ%" ^
    -f net8.0-windows10.0.19041.0 ^
    -c Release ^
    -r win10-x64 ^
    --self-contained true ^
    -p:WindowsPackageType=None ^
    -p:GeneratePackageOnBuild=false ^
    -p:EmbedAssembliesIntoApk=false ^
    -o "%OUTPUT_DIR%"

if %ERRORLEVEL% neq 0 (
    color 0C
    echo.
    echo ========================================================
    echo  [BUILD FAILED] An error occurred during dotnet publish.
    echo ========================================================
    pause
    exit /b 1
)

:: 2. Optional NSIS Installer Compilation
echo.
echo [*] Checking for NSIS (makensis.exe)...
set "MAKENSIS="
if exist "%ProgramFiles(x86)%\NSIS\makensis.exe" set "MAKENSIS=%ProgramFiles(x86)%\NSIS\makensis.exe"
if exist "%ProgramFiles%\NSIS\makensis.exe" set "MAKENSIS=%ProgramFiles%\NSIS\makensis.exe"
if not defined MAKENSIS (
    where makensis >nul 2>nul
    if %ERRORLEVEL% equ 0 set "MAKENSIS=makensis"
)

if defined MAKENSIS (
    echo [*] Extracting version from project file...
    set "APP_VERSION=2.0.0"
    for /f "usebackq delims=" %%V in (`powershell -NoProfile -Command "[xml]$proj = Get-Content '%CSPROJ%'; $ver = $proj.Project.PropertyGroup.FileVersion | Where-Object { $_ } | Select-Object -First 1; if ($ver) { $ver } else { '2.0.0' }"`) do (
        set "APP_VERSION=%%V"
    )
    echo [*] Building Setup Installer with Version: !APP_VERSION!...
    if exist "%SCRIPT_DIR%setup.nsi" (
        "%MAKENSIS%" /DAppVersion="!APP_VERSION!" "%SCRIPT_DIR%setup.nsi"
        if exist "%SCRIPT_DIR%VRCGalleryManager_Setup.exe" (
            if not exist "%SCRIPT_DIR%Output" mkdir "%SCRIPT_DIR%Output"
            move /y "%SCRIPT_DIR%VRCGalleryManager_Setup.exe" "%SCRIPT_DIR%Output\VRCGalleryManager_Setup.exe" >nul
            echo [SUCCESS] Setup Installer created: %SCRIPT_DIR%Output\VRCGalleryManager_Setup.exe
        )
    )
) else (
    echo [INFO] NSIS (makensis) not found. Skipping installer creation.
    echo        The standalone Windows executable is available in Output\Windows.
)

color 0A
echo.
echo ========================================================
echo  [BUILD SUCCESSFUL] Windows build finished!
echo  Executable: %OUTPUT_DIR%\VRCGalleryManager.exe
echo ========================================================
echo.

explorer "%OUTPUT_DIR%"
pause
exit /b 0
