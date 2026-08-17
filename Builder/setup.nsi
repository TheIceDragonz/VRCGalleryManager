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

; Add version information
VIProductVersion "${AppVersion}.0"
VIAddVersionKey /LANG=0x409 "ProductName" "${AppName}"
VIAddVersionKey /LANG=0x409 "FileDescription" "${AppDescription}"
VIAddVersionKey /LANG=0x409 "CompanyName" "${AppPublisher}"
VIAddVersionKey /LANG=0x409 "FileVersion" "${AppVersion}"
VIAddVersionKey /LANG=0x409 "ProductVersion" "${AppVersion}"
VIAddVersionKey /LANG=0x409 "LegalCopyright" "${AppPublisher}"

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
