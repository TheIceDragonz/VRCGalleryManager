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
set "OUTPUT_ROOT=%SCRIPT_DIR%Output"
set "OUTPUT_DIR=%OUTPUT_ROOT%\Windows"
set "PORTABLE_ZIP=%OUTPUT_ROOT%\VRCGalleryManager_Portable.zip"
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
if not exist "%OUTPUT_ROOT%" mkdir "%OUTPUT_ROOT%"

echo.
echo [*] Publishing VRCGalleryManager for Windows (x64 Release, Framework-Dependent)...
echo     Target: net8.0-windows10.0.19041.0
echo     Output: %OUTPUT_DIR%
echo.

dotnet publish "%CSPROJ%" ^
    -f net8.0-windows10.0.19041.0 ^
    -c Release ^
    -r win-x64 ^
    --self-contained false ^
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

rem 2. Clean up redundant files and ALL language folders
echo.
echo [*] Cleaning unnecessary files and eliminating all language folders...
powershell -NoProfile -Command "if (Test-Path '%OUTPUT_DIR%\Builder') { Remove-Item -Recurse -Force '%OUTPUT_DIR%\Builder' }; Get-ChildItem '%OUTPUT_DIR%' -Filter '*.pdb' | Remove-Item -Force -ErrorAction SilentlyContinue; Get-ChildItem '%OUTPUT_DIR%' -Filter '*.winmd' | Remove-Item -Force -ErrorAction SilentlyContinue; Get-ChildItem '%OUTPUT_DIR%' -Filter 'vrcgm*.png' | Remove-Item -Force -ErrorAction SilentlyContinue; Get-ChildItem '%OUTPUT_DIR%' -Filter 'createdump.exe' | Remove-Item -Force -ErrorAction SilentlyContinue; Get-ChildItem '%OUTPUT_DIR%' -Filter 'mscordbi.dll' | Remove-Item -Force -ErrorAction SilentlyContinue; Get-ChildItem '%OUTPUT_DIR%' -Filter 'mscordaccore*.dll' | Remove-Item -Force -ErrorAction SilentlyContinue; if (Test-Path '%OUTPUT_DIR%\WindowsAppRuntime.png') { Remove-Item -Force '%OUTPUT_DIR%\WindowsAppRuntime.png' }; if (Test-Path '%OUTPUT_DIR%\AboutAssets.txt') { Remove-Item -Force '%OUTPUT_DIR%\AboutAssets.txt' }; if (Test-Path '%OUTPUT_DIR%\global.json') { Remove-Item -Force '%OUTPUT_DIR%\global.json' }; if (Test-Path '%OUTPUT_DIR%\wwwroot') { Get-ChildItem -Recurse '%OUTPUT_DIR%\wwwroot' -Filter '*.psd' | Remove-Item -Force -ErrorAction SilentlyContinue }; $keep = @('wwwroot', 'Microsoft.UI.Xaml', 'runtimes'); Get-ChildItem -Directory '%OUTPUT_DIR%' | Where-Object { $_.Name -notin $keep } | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue"

rem 3. Extract version from project file
echo.
echo [*] Extracting version from project file...
set "APP_VERSION=2.0.0"
for /f "usebackq delims=" %%V in (`powershell -NoProfile -Command "[xml]$proj = Get-Content '%CSPROJ%'; $ver = $proj.Project.PropertyGroup.FileVersion | Where-Object { $_ } | Select-Object -First 1; if ($ver) { $ver } else { '2.0.0' }"`) do (
    set "APP_VERSION=%%V"
)
echo     Version: !APP_VERSION!

rem 4. Create Portable ZIP Package
echo.
echo [*] Packaging Portable ZIP (VRCGalleryManager_Portable.zip)...
if exist "%PORTABLE_ZIP%" del /f /q "%PORTABLE_ZIP%"

set "SEVENZIP="
if exist "%ProgramFiles%\7-Zip\7z.exe" set "SEVENZIP=%ProgramFiles%\7-Zip\7z.exe"
if exist "%ProgramFiles(x86)%\7-Zip\7z.exe" set "SEVENZIP=%ProgramFiles(x86)%\7-Zip\7z.exe"
if not defined SEVENZIP (
    where 7z >nul 2>nul
    if !ERRORLEVEL! equ 0 set "SEVENZIP=7z"
)

if defined SEVENZIP (
    echo     Using 7-Zip for high-speed compression...
    pushd "%OUTPUT_DIR%"
    "%SEVENZIP%" a -tzip -mx=7 "%PORTABLE_ZIP%" * >nul
    popd
) else (
    echo     Using PowerShell for ZIP compression...
    powershell -NoProfile -Command "$src = '%OUTPUT_DIR%'; $dst = '%PORTABLE_ZIP%'; if (Test-Path -LiteralPath $dst) { Remove-Item -LiteralPath $dst -Force }; Add-Type -AssemblyName 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory($src, $dst, [System.IO.Compression.CompressionLevel]::Optimal, $false)"
)

if exist "%PORTABLE_ZIP%" (
    echo [SUCCESS] Portable ZIP created: %PORTABLE_ZIP%
) else (
    echo [WARNING] Failed to create portable ZIP package.
)

rem 4. Optional NSIS Installer Compilation
echo.
echo [*] Checking for NSIS...
set "MAKENSIS="
if exist "C:\Program Files (x86)\NSIS\makensis.exe" set "MAKENSIS=C:\Program Files (x86)\NSIS\makensis.exe"
if exist "C:\Program Files\NSIS\makensis.exe" set "MAKENSIS=C:\Program Files\NSIS\makensis.exe"
if "!MAKENSIS!"=="" (
    for /f "tokens=*" %%I in ('where makensis 2^>nul') do set "MAKENSIS=%%I"
)

if "!MAKENSIS!"=="" (
    echo [INFO] NSIS was not found. Skipping installer creation.
    echo        The standalone Windows executable is available in Output\Windows.
    goto :skip_nsis
)

echo [*] Building Setup Installer with Version: !APP_VERSION!...
if exist "%SCRIPT_DIR%setup.nsi" (
    pushd "%SCRIPT_DIR%"
    "!MAKENSIS!" /DAppVersion="!APP_VERSION!" "setup.nsi"
    if exist "%SCRIPT_DIR%VRCGalleryManager_Setup.exe" (
        move /y "%SCRIPT_DIR%VRCGalleryManager_Setup.exe" "%OUTPUT_ROOT%\VRCGalleryManager_Setup.exe" >nul
        echo [SUCCESS] Setup Installer created: %OUTPUT_ROOT%\VRCGalleryManager_Setup.exe
    )
    popd
)

:skip_nsis

color 0A
echo.
echo ========================================================
echo  [BUILD SUCCESSFUL] Windows build finished!
echo  Folder:     %OUTPUT_DIR%
echo  Executable: %OUTPUT_DIR%\VRCGalleryManager.exe
if exist "%PORTABLE_ZIP%" (
echo  Portable:   %PORTABLE_ZIP%
)
if exist "%OUTPUT_ROOT%\VRCGalleryManager_Setup.exe" (
echo  Installer:  %OUTPUT_ROOT%\VRCGalleryManager_Setup.exe
)
echo ========================================================
echo.

explorer "%OUTPUT_ROOT%"
pause
exit /b 0
