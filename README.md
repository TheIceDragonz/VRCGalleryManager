<div align="center">
  <img src="https://github.com/user-attachments/assets/b3503583-ac41-4202-b8ca-3d397580e296" alt="Logo">

  # VRCGalleryManager

  **Modern, Cross-Platform VRChat Media & Profile Management Suite**  
  *Powered by .NET 9 MAUI & Blazor Hybrid for Windows and Android*

  [![Platform](https://img.shields.io/badge/Platforms-Windows%20%7C%20Android-512BD4?style=flat-square&logo=dotnet)](https://github.com/TheIceDragonz/VRCGalleryManager)
  [![Framework](https://img.shields.io/badge/Framework-.NET%209%20MAUI%20Blazor-blue?style=flat-square&logo=blazor)](https://dotnet.microsoft.com/download/dotnet/9.0)
  [![ImageSharp](https://img.shields.io/badge/ImageSharp-4.1.0-orange?style=flat-square)](https://github.com/SixLabors/ImageSharp)
  [![Release](https://img.shields.io/github/v/release/TheIceDragonz/VRCGalleryManager?style=flat-square&color=success)](https://github.com/TheIceDragonz/VRCGalleryManager/releases/latest)

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
      <td align="center"><img src="https://github.com/user-attachments/assets/2ccd67c3-e4b6-4fd2-a093-b784425d6c37" alt="Settings" width="380"></td>
      <td align="center"><img src="https://github.com/user-attachments/assets/c7112de7-ce0d-46ce-92fb-e245b14b6308" alt="Photos" width="380"></td>
    </tr>
  </table>
</div>

---

## 🌟 What's New in v2.0

**VRCGalleryManager v2.0** represents a complete architectural overhaul. Migrated from the original Windows Forms foundation to a cutting-edge **.NET 9 MAUI Blazor Hybrid** application, VRCGalleryManager is now a high-performance cross-platform desktop and mobile client with a modern hardware-accelerated UI, native OS integrations, enhanced security, and deep VRChat profile customization.

---

## <a id="key-features"></a>✨ Key Features

### 📱 Cross-Platform Architecture (.NET 9 MAUI + Blazor)
- **Windows & Android Support**: Runs seamlessly on **Windows 10/11** desktop and **Android** mobile devices (native APK).
- **Responsive Modern UI**: Sleek dark-mode interface built with CSS design tokens, custom SVG icons, smooth transitions, and custom scrollbars.
- **Android Immersive Experience**: Edge-to-edge layout with full display cutout (notch) support, translucent system bars, and responsive navigation drawers.
- **Cross-Platform Image Engine**: Powered by **SixLabors ImageSharp 4.1.0** for high-performance cross-platform image processing without native GDI+ dependencies.
- **Touch & Desktop Optimized**: Native mouse & keyboard controls on Windows with full touch UX and mobile-friendly gestures on Android.

### 🎨 VRChat Profile Customization & Dynamic Themes
- **VRChat Profile Theme Sync**: Automatically extracts your active VRChat profile theme colors (accent, buttons, icons, subtexts) and applies them dynamically across the app, including custom TitleBar buttons and scrollbars.
- **Custom Windows TitleBar**: Sleek, borderless window header with custom minimize, maximize, and close buttons integrated with your active profile theme.
- **Profile Backgrounds Catalog**: Access and preview official VRChat profile background banners (Grid, Cascade, Bit Mountain, Approach, Planet Fall, Jungle, Light Streams, etc.).
- **Animated Profile Effects ❤️**: Support for VRChat profile cosmetics and animated canvas effects (Aura, Fire, Orbit, Bounce, Rain, Laser, Confetti, and more).
- **Profile Details Card**: View your profile bio, status, pronouns, trust rank, unlimited user badges, icon frames, and avatar pictures directly in-app.

### 🖼️ Comprehensive VRChat Plus Media Management
- **Stickers & Emojis**: Create, upload, organize, and delete VRChat custom stickers and emojis.
- **Photos & Gallery**: Browse, manage, and upload high-resolution screenshots to your VRChat Plus online gallery.
- **Prints**: View, manage, and upload VRChat Prints with custom notes and print details.
- **User Profile Icons**: Crop, adjust, and upload user profile icons.
- **Drag & Drop & Clipboard Paste**: Drag and drop images directly into the application window or paste directly from clipboard (`Ctrl + V`).
- **High-Performance Caching (`MediaCacheService`)**: Smart local memory and disk caching prevents redundant network queries and ensures instant thumbnail loading.

### 🔍 Immersive Image Viewer & Native Saving
- **Full-Screen Viewer**: Immersive photo modal with smooth zooming, panning, and keyboard/touch navigation.
- **Direct Download & Gallery Sync**:
  - **Windows**: Native file save dialog to save photos anywhere on your system.
  - **Android**: Direct save to `Pictures/VRCGalleryManager` with automatic MediaStore scanner notification, making images immediately visible in your device's photo gallery without needing a file manager.
- **Quick Actions**: Download images locally, copy image/link to the system clipboard, or upload directly to VRChat.
- **Integrated Metadata Inspector & QR Scanner**: Inspect rich VRChat/VRCX world information, present player rosters, resolution, aspect ratio, and automatically detect QR codes embedded in photos.
- **Standalone Real-Time Metadata Injection (Windows)**: Pure C# PNG `iTXt` injector that monitors live game sessions and automatically embeds world, instance, and player lists into newly taken screenshots without needing external tools.

### ✂️ Integrated Image & Emoji Editors
- **Quality Scale & Size Preview**: Real-time file size calculation and quality scaling slider before uploading to ensure images never exceed VRChat limits.
- **Refine Edge & Crop**: Built-in canvas tools for cropping and refining edges.
- **GIF to Sprite Sheet Converter**: Convert animated GIFs into VRChat-compatible sprite sheet animations with configurable FPS and timeline settings.

<div align="center">
  <table>
    <tr>
      <td align="center"><img src="https://github.com/user-attachments/assets/ab81521f-9115-4f2f-9127-3eb84d09198a" alt="Image Editor" width="380"></td>
      <td align="center"><img src="https://github.com/user-attachments/assets/3c6ccc5b-b4d9-48f8-a9b6-83133f5ea84b" alt="Emoji Editor" width="380"></td>
    </tr>
  </table>
</div>

### 🌐 Picflow - Global Explorer & Local Database
- **Worldwide Sticker Explorer**: Browse stickers placed across VRChat worlds with lazy-loading.
- **Emoji & Print Discovery**: Discover emojis and prints alongside world stickers.
- **Live Stream Mode (Windows)**: Monitor and inspect new stickers in real time from VRChat logs as players place them.
- **Local Database (`PicflowDatabase`)**: Persistent local storage to cache and quickly search through discovered stickers and prints with one-click cleanup.
- **Info Dialog**: Quick inspection modal with creator information and asset details.

### 🔒 Security & Connectivity
- **Internal High-Performance API Client**: Custom-built, lightweight VRChat API client replacing external SDKs with clean JSON parsing and direct multi-part uploads.
- **Modern Secure Storage**: Uses platform-native OS secure storage (`Microsoft.Maui.Storage.SecureStorage`) instead of plaintext config files.
- **Two-Factor Authentication (2FA)**: Full support for both Authenticator TOTP codes and Email verification (with resend code option).
- **Resilient Networking**: Automated network retry and backoff policies powered by **Polly**.
- **Real-Time Network Monitor**: Dedicated `NetworkStatusService` detecting offline state with auto-reconnect notifications.

### ⚙️ Desktop & Mobile Maintenance Tools
- **System Tray Integration (Windows)**: Minimize to tray (`H.NotifyIcon.Maui`) to keep the app running quietly in the background.
- **Windows Startup**: Optional toggle to launch automatically with Windows (with silent background startup).
- **In-App Auto-Updater**:
  - **Windows**: Built-in update manager with animated download progress bars.
  - **Android**: In-app APK updater that checks releases, downloads the new APK, and triggers the native package installer.
- **VRChat Logs Inspector (Windows)**: View VRChat log file counts and total disk usage, with one-click options to open the logs folder or clean up old logs.
- **VRChat Cache Cleaner**: Monitor VRChat cache size and safely purge temporary files directly from the settings panel.
- **Legal Notice & Compliance**: Built-in Terms of Service compliance and legal notice panel.

---

## <a id="gallery--vrcx-metadata-integration"></a>📷 Screenshot Metadata Engine & VRCX Integration

VRCGalleryManager includes a powerful, zero-dependency **PNG Metadata Engine** that seamlessly bridges VRChat, **[VRCX](https://github.com/vrcx-team/VRCX)**, and your local screenshot library.

### ⚡ Automatic Real-Time Metadata Injection (Windows)
- **Live VRChat Log Monitoring (Windows)**: The built-in `VRCLogWatcherService` tracks your active world, instance ID, world author, and dynamic player joins/leaves in real time.
- **Pure C# `iTXt` Injection**: When a new screenshot is saved to your VRChat Pictures directory, VRCGalleryManager can automatically inject an official/VRCX-compliant `iTXt` `Description` chunk directly into the PNG file.
- **Atomic & Safe**: Recalculates PNG CRC32 checksums on the fly and writes atomically via temporary files, guaranteeing zero image corruption.
- **Standalone Convenience**: No need to keep external helper applications running—your photos are automatically tagged with world and player information right as you take them (can be toggled on/off in Settings).

### 🔍 Lightning-Fast Metadata Extraction
- **Sub-Millisecond Parsing**: High-performance reader (`MetaDataImageReader`) parses PNG `iTXt` text chunks directly from the file stream in milliseconds without buffering or decoding full high-resolution images into RAM.
- **Smart Fallback Scanner**: Employs span-based memory pools (`ArrayPool<byte>`) to detect metadata structures in legacy, modified, or alternative screenshot formats.
- **Cross-Compatible**: Fully interoperable with the **VRCX Screenshot Helper** standard and official VRChat camera metadata.

### 📋 Rich Inspector & In-App Viewer
When viewing photos in the Gallery, through screenshot popups, or in the immersive full-screen Image Viewer:
- **World & Instance Info**: World name, world author, instance type/ID, with quick-copy actions and a direct button to launch or open the world on the VRChat website.
- **Players Roster**: Searchable list of all players present in the instance when the photo was captured, with one-click navigation to their official VRChat web profiles.
- **Image Diagnostics**: File format, exact resolution, aspect ratio badge (16:9, 21:9, etc.), file size, capture date & time, and file name.
- **Integrated QR Code Scanner**: Built-in `QrCodeService` detects QR codes (Discord invites, world links, web URLs) inside screenshots and presents them for instant copying or browser launching.

<div align="center">
  <table>
    <tr>
      <td align="center"><img src="https://github.com/user-attachments/assets/3106c976-8e23-4bb7-a43c-eab9edfaae8e" alt="Gallery" width="380"></td>
      <td align="center"><img src="https://github.com/user-attachments/assets/1376d88c-c543-486a-bc6e-02af608f0bf0" alt="Image Viewer" width="380"></td>
    </tr>
  </table>
</div>

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
- **Standalone / Portable**: Extract the portable ZIP release (`VRCGalleryManager_Portable.zip`) and launch `VRCGalleryManager.exe`.

### Android
- Download the `VRCGalleryManager.apk` file to your Android device.
- Open the file and allow installation from your browser/file manager when prompted.
- Future updates can be checked and installed directly from within the app!

---

## <a id="building-from-source"></a>🛠️ Building from Source

VRCGalleryManager includes an automated build system located in the [`Builder/`](./Builder/) directory for compiling Windows and Android binaries with one click.

### Prerequisites
1. **.NET 9 SDK**: [Download .NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
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
- **`Builder/Pack_Portable_ZIP.bat`**: Quickly packages the compiled Windows build into a portable ZIP package.
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
