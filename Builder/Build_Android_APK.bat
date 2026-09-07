@echo off
setlocal enabledelayedexpansion

title VRCGalleryManager - Android APK Builder
color 0B

echo ========================================================
echo        VRCGalleryManager - Android APK Builder
echo ========================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "ROOT_DIR=%SCRIPT_DIR%.."
set "OUTPUT_ROOT=%SCRIPT_DIR%Output"
set "OUTPUT_DIR=%OUTPUT_ROOT%\Android"
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
echo [*] Checking Android Workload...
dotnet workload list | findstr /i "android" >nul
if %ERRORLEVEL% neq 0 (
    echo [WARNING] Android workload might not be installed.
    echo If the build fails, run: dotnet workload install maui-android
    echo.
)

echo [*] Cleaning previous Android build output...
if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%"
)
mkdir "%OUTPUT_DIR%"

echo.
echo [*] Building and Publishing Android APK (Release)...
echo     Target: net9.0-android
echo     Output: %OUTPUT_DIR%
echo.

dotnet publish "%CSPROJ%" ^
    -f net9.0-android ^
    -c Release ^
    -p:AndroidPackageFormats=apk ^
    -p:EmbedAssembliesIntoApk=true ^
    -p:AndroidKeyStore=false ^
    -p:GeneratePackageOnBuild=false ^
    -o "%OUTPUT_DIR%"

if %ERRORLEVEL% neq 0 (
    color 0C
    echo.
    echo ========================================================
    echo  [BUILD FAILED] Android build encountered errors.
    echo  Ensure that Android SDK and workload 'maui-android' are installed.
    echo ========================================================
    pause
    exit /b 1
)

:: Find the generated APK and create a clean shortcut copy
set "FOUND_APK="
for /r "%OUTPUT_DIR%" %%F in (*-Signed.apk) do (
    set "FOUND_APK=%%F"
)
if not defined FOUND_APK (
    for /r "%OUTPUT_DIR%" %%F in (*.apk) do (
        set "FOUND_APK=%%F"
    )
)
if not defined FOUND_APK (
    for /r "%ROOT_DIR%\bin\Release\net9.0-android" %%F in (*.apk) do (
        set "FOUND_APK=%%F"
    )
)

if defined FOUND_APK (
    copy /y "!FOUND_APK!" "%OUTPUT_ROOT%\VRCGalleryManager.apk" >nul
    copy /y "!FOUND_APK!" "%OUTPUT_DIR%\VRCGalleryManager.apk" >nul
    color 0A
    echo.
    echo ========================================================
    echo  [BUILD SUCCESSFUL] Android APK built successfully!
    echo  APK Location: %OUTPUT_ROOT%\VRCGalleryManager.apk
    echo ========================================================
    echo.
) else (
    color 0E
    echo.
    echo [WARNING] Publish finished, but APK file could not be automatically located in output.
    echo Check folder: %OUTPUT_DIR%
    echo.
)

explorer "%OUTPUT_ROOT%"
pause
exit /b 0
