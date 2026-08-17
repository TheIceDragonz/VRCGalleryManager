# VRCGalleryManager - Build System

Questa cartella contiene gli script per compilare automaticamente la versione **Windows (EXE / Setup)** e **Android (APK)** di VRCGalleryManager.

---

## 📁 Struttura della cartella

- **`Build_Windows_EXE.bat`**: Compila e pubblica l'app per Windows (`net8.0-windows10.0.19041.0`, x64, self-contained). Se NSIS è installato sul computer, genera anche il file `VRCGalleryManager_Setup.exe`.
- **`Build_Android_APK.bat`**: Compila e pubblica il pacchetto Android (`net8.0-android`, Release) ed estrae il file `VRCGalleryManager.apk`.
- **`Build_All.bat`**: Esegue in sequenza sia la build Windows che la build Android.
- **`setup.nsi`**: Script di configurazione NSIS per generare l'installer di installazione Windows.
- **`Output/`**: Cartella di destinazione (creata automaticamente) contenente:
  - `Output/Windows/`: File eseguibile standalone (`VRCGalleryManager.exe`) e dipendenze.
  - `Output/Android/`: File APK per dispositivi Android (`VRCGalleryManager.apk`).
  - `Output/VRCGalleryManager_Setup.exe`: Installer Windows (se NSIS è presente).

---

## ⚙️ Prerequisiti

### 1. .NET 8 SDK
Assicurati di avere installato il **.NET 8 SDK**:
- Scaricabile da: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

### 2. Workload .NET MAUI
Se necessario, puoi installare o aggiornare i carichi di lavoro MAUI aprendo il terminale come amministratore ed eseguendo:
```bash
dotnet workload install maui
# Oppure singolarmente:
dotnet workload install maui-windows
dotnet workload install maui-android
```

### 3. NSIS (Opzionale - Per creare l'installer Setup.exe)
Se desideri creare l'installer Windows oltre all'eseguibile standalone:
- Installa **NSIS (Nullsoft Scriptable Install System)**: [https://nsis.sourceforge.io/Download](https://nsis.sourceforge.io/Download)

---

## 🚀 Utilizzo

Fai semplicemente doppio clic su uno dei file `.bat`:
1. Fai doppio clic su `Build_Windows_EXE.bat` per Windows.
2. Fai doppio clic su `Build_Android_APK.bat` per Android.
3. Fai doppio clic su `Build_All.bat` per entrambi.
