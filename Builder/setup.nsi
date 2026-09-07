;--------------------------------
; Configurations and Definitions
;--------------------------------
!define AppName "VRCGalleryManager"
!ifndef AppVersion
  !define AppVersion "2.0.0"
!endif
!define AppPublisher "TheIceDragonz"
!define AppDescription "Simple Tool for your VRChat gallery"

; Icon
!define MUI_ICON "..\icon.ico"
!define MUI_UNICON "..\icon.ico"

;--------------------------------
; Installer Settings
;--------------------------------
Outfile "${AppName}_Setup.exe"
Name "${AppName}"
BrandingText "Nullsoft Install System v3.10+"

!include "MUI2.nsh"   ; Use Modern UI 2
!include "x64.nsh"    ; 64-bit Windows support

; Add version information
VIProductVersion "${AppVersion}.0"
VIAddVersionKey /LANG=0x409 "ProductName" "${AppName}"
VIAddVersionKey /LANG=0x409 "FileDescription" "${AppDescription}"
VIAddVersionKey /LANG=0x409 "CompanyName" "${AppPublisher}"
VIAddVersionKey /LANG=0x409 "FileVersion" "${AppVersion}"
VIAddVersionKey /LANG=0x409 "ProductVersion" "${AppVersion}"
VIAddVersionKey /LANG=0x409 "LegalCopyright" "${AppPublisher}"

;--------------------------------
; 64-bit Architecture Initialization
;--------------------------------
Function .onInit
    ${If} ${RunningX64}
        SetRegView 64
        StrCpy $INSTDIR "$PROGRAMFILES64\${AppName}"
    ${Else}
        MessageBox MB_OK|MB_ICONSTOP "This application requires a 64-bit version of Windows."
        Abort
    ${EndIf}

    ; Check for Microsoft .NET Desktop Runtime 8.0 (x64)
    FindFirst $0 $1 "$PROGRAMFILES64\dotnet\shared\Microsoft.WindowsDesktop.App\8.*"
    FindClose $0
    ${If} $1 == ""
        MessageBox MB_YESNO|MB_ICONEXCLAMATION "${AppName} requires Microsoft .NET Desktop Runtime 8.0 (x64) to run.$\n$\nWould you like to open the official download page now?" IDYES downloadRuntime IDNO continueInstall
downloadRuntime:
        ExecShell "open" "https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe"
continueInstall:
    ${EndIf}
FunctionEnd

Function un.onInit
    ${If} ${RunningX64}
        SetRegView 64
    ${EndIf}
FunctionEnd

;--------------------------------
; Installer Pages
;--------------------------------
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\VRCGalleryManager.exe"
!insertmacro MUI_PAGE_FINISH

;--------------------------------
; Default Installation Directory
;--------------------------------
InstallDir "$PROGRAMFILES64\${AppName}"
InstallDirRegKey HKLM "Software\${AppName}" "Install_Dir"

;--------------------------------
; Installation Section
;--------------------------------
Section "Install"
    SetDetailsView show
    SetOutPath "$INSTDIR"
    
    ; Close any running instances so DLLs are not locked by Windows
    DetailPrint "Closing running instances of ${AppName}..."
    nsExec::Exec 'taskkill /F /IM "${AppName}.exe" /T'
    Sleep 500
    
    ; Clean legacy files and obsolete language folders from previous installations
    ${If} ${FileExists} "$INSTDIR\VRCGalleryManager.exe"
        DetailPrint "Cleaning previous installation files..."
        Delete "$INSTDIR\*.dll"
        Delete "$INSTDIR\*.pdb"
        Delete "$INSTDIR\*.winmd"
        Delete "$INSTDIR\*.json"
        Delete "$INSTDIR\createdump.exe"
    ${EndIf}

    ; Remove all obsolete language folders
    RMDir /r "$INSTDIR\af"
    RMDir /r "$INSTDIR\ar"
    RMDir /r "$INSTDIR\bg"
    RMDir /r "$INSTDIR\ca"
    RMDir /r "$INSTDIR\cs"
    RMDir /r "$INSTDIR\da"
    RMDir /r "$INSTDIR\de"
    RMDir /r "$INSTDIR\el"
    RMDir /r "$INSTDIR\en-GB"
    RMDir /r "$INSTDIR\en-US"
    RMDir /r "$INSTDIR\en-us"
    RMDir /r "$INSTDIR\es"
    RMDir /r "$INSTDIR\es-MX"
    RMDir /r "$INSTDIR\et"
    RMDir /r "$INSTDIR\eu"
    RMDir /r "$INSTDIR\fi"
    RMDir /r "$INSTDIR\fil-Latn"
    RMDir /r "$INSTDIR\fr"
    RMDir /r "$INSTDIR\fr-CA"
    RMDir /r "$INSTDIR\he"
    RMDir /r "$INSTDIR\hi"
    RMDir /r "$INSTDIR\hr"
    RMDir /r "$INSTDIR\hu"
    RMDir /r "$INSTDIR\id"
    RMDir /r "$INSTDIR\it"
    RMDir /r "$INSTDIR\it-IT"
    RMDir /r "$INSTDIR\ja"
    RMDir /r "$INSTDIR\ko"
    RMDir /r "$INSTDIR\lt"
    RMDir /r "$INSTDIR\lv"
    RMDir /r "$INSTDIR\nb"
    RMDir /r "$INSTDIR\nl"
    RMDir /r "$INSTDIR\nn"
    RMDir /r "$INSTDIR\pl"
    RMDir /r "$INSTDIR\pt-BR"
    RMDir /r "$INSTDIR\pt-PT"
    RMDir /r "$INSTDIR\ro"
    RMDir /r "$INSTDIR\ru"
    RMDir /r "$INSTDIR\sk"
    RMDir /r "$INSTDIR\sl"
    RMDir /r "$INSTDIR\sr-Cyrl"
    RMDir /r "$INSTDIR\sr-Latn"
    RMDir /r "$INSTDIR\sv"
    RMDir /r "$INSTDIR\th"
    RMDir /r "$INSTDIR\tr"
    RMDir /r "$INSTDIR\uk"
    RMDir /r "$INSTDIR\vi"
    RMDir /r "$INSTDIR\zh-Hans"
    RMDir /r "$INSTDIR\zh-Hant"
    
    ; Copy application files from Output\Windows
    File /r "Output\Windows\*.*"
    
    ; Copy the Icon.ico file
    File "..\icon.ico"
    
    ; Create shortcuts (desktop and Start menu)
    CreateShortcut "$DESKTOP\${AppName}.lnk" "$INSTDIR\VRCGalleryManager.exe" "" "$INSTDIR\icon.ico"
    CreateDirectory "$SMPROGRAMS\${AppName}"
    CreateShortcut "$SMPROGRAMS\${AppName}\${AppName}.lnk" "$INSTDIR\VRCGalleryManager.exe" "" "$INSTDIR\icon.ico"
    
    ; Register the installation directory in the registry
    WriteRegStr HKLM "Software\${AppName}" "Install_Dir" "$INSTDIR"
    
    ; Write the uninstaller
    WriteUninstaller "$INSTDIR\Uninstall.exe"
    
    ; Add the application to "Programs and Features"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "DisplayName" "${AppName}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "DisplayIcon" "$INSTDIR\VRCGalleryManager.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "Publisher" "${AppPublisher}"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}" "NoRepair" 1
SectionEnd

;--------------------------------
; Uninstallation Section
;--------------------------------
Section "Uninstall"
    ; Close any running instances before uninstalling
    DetailPrint "Closing running instances of ${AppName}..."
    nsExec::Exec 'taskkill /F /IM "${AppName}.exe" /T'
    Sleep 500

    Delete "$INSTDIR\VRCGalleryManager.exe"
    Delete "$INSTDIR\Uninstall.exe"
    Delete "$DESKTOP\${AppName}.lnk"
    Delete "$INSTDIR\icon.ico"
    Delete "$SMPROGRAMS\${AppName}\${AppName}.lnk"
    RMDir "$SMPROGRAMS\${AppName}"
    RMDir /r "$INSTDIR"   ; Remove the entire installation folder recursively
    DeleteRegKey HKLM "Software\${AppName}"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${AppName}"
SectionEnd

!insertmacro MUI_LANGUAGE "English"
