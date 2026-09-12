# VRCGalleryManager - Build System

This folder contains scripts to automatically compile the **Windows (EXE / Setup)** and **Android (APK)** versions of VRCGalleryManager.

---

## Folder Structure

- **`Build_Windows_EXE.bat`**: Compiles and publishes the app for Windows (`net9.0-windows10.0.19041.0`, x64, self-contained, no .NET installation required), automatically creates the **Portable ZIP** package (`VRCGalleryManager_Portable.zip`), and creates the `VRCGalleryManager_Setup.exe` installer if NSIS is installed.
- **`Pack_Portable_ZIP.bat`**: Quickly generates or updates the **Portable ZIP** from the existing `Output/Windows` build without recompiling.
- **`Build_Android_APK.bat`**: Compiles and publishes the Android package (`net9.0-android`, Release) and extracts `VRCGalleryManager.apk`.
- **`Build_All.bat`**: Runs both the Windows and Android builds in sequence.
- **`setup.nsi`**: NSIS configuration script for generating the Windows installer.
- **`Output/`**: Destination directory (generated automatically) containing:
  - `Output/Windows/`: Standalone executable (`VRCGalleryManager.exe`) and dependencies.
  - `Output/VRCGalleryManager_Portable.zip`: Portable ZIP release ready to extract and run anywhere.
  - `Output/VRCGalleryManager_Setup.exe`: Windows installer (when NSIS is installed).
  - `Output/VRCGalleryManager.apk`: Android APK ready to install.
  - `Output/Android/`: Android build intermediate files and signed APK.

---

## Prerequisites

### 1. .NET 9 SDK
Ensure you have the **.NET 9 SDK** installed:
- Download: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

### 2. .NET MAUI Workloads
If needed, install or update the MAUI workloads by opening an elevated terminal (Run as administrator) and executing:
```bash
dotnet workload install maui
# Or individually:
dotnet workload install maui-windows
dotnet workload install maui-android
```

### 3. NSIS (Optional - To create the Setup.exe installer)
If you want to create the Windows installer in addition to the standalone executable:
- Install **NSIS (Nullsoft Scriptable Install System)**: [https://nsis.sourceforge.io/Download](https://nsis.sourceforge.io/Download)

---

## Usage

Simply double-click any of the `.bat` files:
1. Double-click `Build_Windows_EXE.bat` for Windows.
2. Double-click `Build_Android_APK.bat` for Android.
3. Double-click `Build_All.bat` for both.
