<div align="center">
  <img src="https://github.com/user-attachments/assets/b3503583-ac41-4202-b8ca-3d397580e296" alt="Logo">

  # VRCGalleryManager v2.0

  **Modern, Cross-Platform VRChat Media & Profile Management Suite**  
  *Powered by .NET 8 MAUI & Blazor Hybrid for Windows and Android*

  [![Platform](https://img.shields.io/badge/Platforms-Windows%20%7C%20Android-512BD4?style=flat-square&logo=dotnet)](https://github.com/TheIceDragonz/VRCGalleryManager)
  [![Framework](https://img.shields.io/badge/Framework-.NET%208%20MAUI%20Blazor-blue?style=flat-square&logo=blazor)](https://dotnet.microsoft.com/download/dotnet/8.0)
  [![ImageSharp](https://img.shields.io/badge/ImageSharp-4.1.0-orange?style=flat-square)](https://github.com/SixLabors/ImageSharp)
  [![Release](https://img.shields.io/badge/Release-v2.0.0-success?style=flat-square)](https://github.com/TheIceDragonz/VRCGalleryManager/releases/latest)

  <p align="center">
    <a href="#key-features">Key Features</a> •
    <a href="#gallery--vrcx-metadata-integration">Metadata Integration</a> •
    <a href="#requirements">Requirements</a> •
    <a href="#installation">Installation</a> •
    <a href="#building-from-source">Build System</a> •
    <a href="#disclaimer">Disclaimer</a>
  </p>

  <table>
    <tr>
      <td align="center"><img src="https://github.com/user-attachments/assets/37d60ff7-d03b-44d2-8b03-c1d2a934001d" alt="login" width="380"></td>
      <td align="center"><img src="https://github.com/user-attachments/assets/d0279e66-6370-4156-aeba-c0dfce009650" alt="2fa" width="380"></td>
    </tr>
  </table>
</div>

---

## 🌟 What's New in v2.0

**VRCGalleryManager v2.0** represents a complete architectural overhaul. Migrated from the original Windows Forms foundation to a cutting-edge **.NET 8 MAUI Blazor Hybrid** application, VRCGalleryManager is now a cross-platform desktop and mobile client with a responsive, hardware-accelerated UI, modern security practices, and deep VRChat profile integration.

---

## <a id="key-features"></a>✨ Key Features

### 📱 Cross-Platform Architecture (.NET 8 MAUI + Blazor)
- **Windows & Android Support**: Runs seamlessly on **Windows 10/11** desktop and **Android** mobile devices (APK).
- **Responsive Modern UI**: Modern dark-mode interface built with CSS design tokens, custom SVG icons, smooth transitions, and custom scrollbars.
- **Cross-Platform Image Engine**: Powered by **SixLabors ImageSharp 4.1.0** for high-performance cross-platform image manipulation without native GDI+ dependencies.
- **Touch & Desktop Optimized**: Native mouse & keyboard controls on Windows with full touch UX and responsive navigation drawers on Android.

### 🎨 VRChat Profile Customization & Dynamic Themes
- **VRChat Profile Themes Sync**: Automatically extracts your active VRChat profile theme colors (accent, buttons, icons, subtexts) and applies them dynamically across the app.
- **Profile Backgrounds Catalog**: Access and preview official VRChat profile background banners (Grid, Cascade, Bit Mountain, Approach, Planet Fall, Jungle, Light Streams, etc.).
- **Animated Profile Effects ❤️**: Support for VRChat profile cosmetics and animated canvas effects (Aura, Fire, Orbit, Bounce, Rain, Laser, Confetti, and more).
- **Profile Dialog Card**: View your profile bio, status, pronouns, user badges, icon frames, and avatar pictures directly in-app.

### 🖼️ Comprehensive VRChat Plus Media Management
- **Stickers & Emojis**: Create, upload, organize, and delete VRChat custom stickers and emojis.
- **Photos & Gallery**: Browse, manage, and upload high-resolution screenshots to your VRChat Plus online gallery.
- **Prints**: View, manage, and upload VRChat Prints with custom notes and print details.
- **User Profile Icons**: Crop, adjust, and upload user profile icons.
- **Drag & Drop & Clipboard Paste**: Drag and drop images directly into the application window or paste directly from clipboard (`Ctrl + V`).
- **High-Performance Caching (`MediaCacheService`)**: Smart local memory and disk caching prevents redundant network queries and ensures instant thumbnail loading.

### 🔍 Immersive Image Viewer
- **Full-Screen Viewer**: Immersive photo modal with smooth zooming, panning, and keyboard/touch navigation.
- **Quick Actions**: Download images locally, copy image/link to the system clipboard, or upload directly to VRChat.
- **Integrated Metadata Inspector**: View detailed camera metadata and VRCX session info directly within the viewer.

### ✂️ Integrated Image & Emoji Editors
- **Quality Scale & Size Preview**: Real-time file size calculation and quality scaling slider before uploading to ensure images never exceed VRChat limits.
- **Refine Edge & Crop**: Built-in canvas tools for cropping and refining edges.
- **GIF to Sprite Sheet Converter**: Convert animated GIFs into VRChat-compatible sprite sheet animations with configurable FPS and timeline settings.

### 🌐 Picflow - Global Explorer & Local Database
- **Worldwide Sticker Explorer**: Browse stickers placed across VRChat worlds with lazy-loading.
- **Emoji & Print Discovery**: Now includes emojis and prints alongside world stickers.
- **Live Stream Mode**: Monitor and inspect new stickers in real time as players place them.
- **Local Database (`PicflowDatabase`)**: Persistent local storage to cache and quickly search through discovered stickers and prints with one-click cleanup.

### 🔒 Security & Connectivity
- **Modern Secure Storage**: Uses platform-native OS secure storage (`Microsoft.Maui.Storage.SecureStorage`) instead of legacy encryption files.
- **Two-Factor Authentication (2FA)**: Full support for both Authenticator TOTP codes and Email verification (with resend code option).
- **Resilient Networking**: Automated network retry and backoff policies powered by **Polly**.
- **Real-Time Network Monitor**: Dedicated `NetworkStatusService` detecting offline state with auto-reconnect notifications.

### ⚙️ Desktop Integration & Maintenance Tools
- **System Tray Integration**: Minimize to tray (`H.NotifyIcon.Maui`) to keep the app running quietly in the background.
- **Windows Startup**: Optional toggle to launch automatically with Windows (with silent background startup).
- **VRChat Logs Inspector**: View VRChat log file counts and total disk usage, with one-click options to open the logs folder or clean up old logs.
- **VRChat Cache Cleaner**: Monitor VRChat cache size and safely purge temporary files directly from the settings panel.
- **In-App Auto-Updater**: Built-in update manager with animated download progress bars.

---

## <a id="gallery--vrcx-metadata-integration"></a>📷 Gallery & VRCX Metadata Integration

Integrates with the **[VRCX](https://github.com/vrcx-team/VRCX) Screenshot Helper** metadata. When viewing screenshots in the gallery or in the immersive image viewer, the application automatically extracts:
- **World Information**: World name, world author, and a direct clickable link to the VRChat website.
- **Instance Players**: Full list of players present in the instance at the exact moment the picture was taken.
- **World Thumbnail & Capture Timestamp**: Exact date, time, and world details.

<table>
  <tr>
    <td align="center"><img src="https://github.com/user-attachments/assets/2837570a-64ef-443c-a863-d42ab7ee58e2" alt="gallery_metadata" width="380"></td>
    <td align="center"><img src="https://github.com/user-attachments/assets/04fbbe68-37ae-4409-a0e3-a129e2838bc9" alt="gallery_players" width="380"></td>
  </tr>
</table>

---

## <a id="requirements"></a>📋 Requirements

### VRChat Account
- A **VRChat Plus** subscription is required to manage photo galleries, prints, and custom stickers through the VRChat API.
  - [Get VRChat Plus](https://vrchat.com/home/subscriptions)

### System Requirements
| Platform | Requirement |
| :--- | :--- |
| **Windows** | Windows 10 (version 1809 / build 17763 or newer) or Windows 11 (64-bit) |
| **Android** | Android 7.0 (Nougat, API 24) or newer |
| **Webview** | Microsoft Edge WebView2 Runtime (included in Windows 10/11) / Android System WebView |

---

## <a id="installation"></a>🚀 Installation & Download

Download the latest release for your platform from the **[Releases Page](https://github.com/TheIceDragonz/VRCGalleryManager/releases/latest)**:

### Windows
- **Installer (Recommended)**: Download and run `VRCGalleryManager_Setup.exe` to install the app with desktop and Start Menu shortcuts.
- **Standalone / Portable**: Extract the portable ZIP release and launch `VRCGalleryManager.exe`.

### Android
- Download the `VRCGalleryManager.apk` file to your Android device.
- Open the file and allow installation from your browser/file manager when prompted.

---

## <a id="building-from-source"></a>🛠️ Building from Source

VRCGalleryManager includes an automated build system located in the [`Builder/`](./Builder/) directory for compiling Windows and Android binaries with one click.

### Prerequisites
1. **.NET 8 SDK**: [Download .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **.NET MAUI Workloads**:
   ```bash
   dotnet workload install maui
   # Or install specific target workloads:
   dotnet workload install maui-windows
   dotnet workload install maui-android
   ```
3. **NSIS (Optional)**: If you want to generate the Windows `VRCGalleryManager_Setup.exe` installer, install [NSIS (Nullsoft Scriptable Install System)](https://nsis.sourceforge.io/Download).

### Build Scripts
Simply execute the batch scripts located in the `Builder/` folder:
- **`Builder/Build_Windows_EXE.bat`**: Compiles a self-contained Windows executable and generates the NSIS setup installer.
- **`Builder/Build_Android_APK.bat`**: Compiles and packages the release Android APK (`VRCGalleryManager.apk`).
- **`Builder/Build_All.bat`**: Builds both Windows and Android packages in sequence.

Artifacts will be placed in `Builder/Output/`:
- `Builder/Output/Windows/`
- `Builder/Output/VRCGalleryManager_Portable.zip`
- `Builder/Output/VRCGalleryManager_Setup.exe`
- `Builder/Output/VRCGalleryManager.apk`

---

## <a id="disclaimer"></a>📜 Disclaimer & Terms of Service

VRCGalleryManager is an independent third-party tool that communicates exclusively through VRChat's official API endpoints. It does not modify, hook into, or alter the VRChat client or game files in any manner.

##### VRCGalleryManager is not affiliated with, sponsored by, or endorsed by VRChat Inc. VRChat and related marks are trademarks of VRChat Inc.
