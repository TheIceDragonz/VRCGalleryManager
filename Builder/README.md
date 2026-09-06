# VRCGalleryManager - Build System

This folder contains scripts to automatically compile the **Windows (EXE / Setup)** and **Android (APK)** versions of VRCGalleryManager.

---

## 📁 Folder Structure

- **`Build_Windows_EXE.bat`**: Compiles and publishes the app for Windows (`net8.0-windows10.0.19041.0`, x64, self-contained). If NSIS is installed on your computer, it also creates the `VRCGalleryManager_Setup.exe` installer.
- **`Build_Android_APK.bat`**: Compiles and publishes the Android package (`net8.0-android`, Release) and extracts `VRCGalleryManager.apk`.
- **`Build_All.bat`**: Runs both the Windows and Android builds in sequence.
- **`setup.nsi`**: NSIS configuration script for generating the Windows installer.
- **`Output/`**: Destination directory (generated automatically) containing:
  - `Output/Windows/`: Standalone executable (`VRCGalleryManager.exe`) and dependencies.
  - `Output/Android/`: APK file for Android devices (`VRCGalleryManager.apk`).
  - `Output/VRCGalleryManager_Setup.exe`: Windows installer (when NSIS is installed).

---

## ⚙️ Prerequisites

### 1. .NET 8 SDK
Ensure you have the **.NET 8 SDK** installed:
- Download: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

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

## 🚀 Usage

Simply double-click any of the `.bat` files:
1. Double-click `Build_Windows_EXE.bat` for Windows.
2. Double-click `Build_Android_APK.bat` for Android.
3. Double-click `Build_All.bat` for both.
