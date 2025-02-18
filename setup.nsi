; Installer name and author
Outfile "VRCGalleryManager.exe"
Name "VRCGalleryManager"
BrandingText "Nullsoft Install System v3.10+"

!include "MUI2.nsh"  ; Use Modern UI 2

; Graphic settings
!define MUI_ICON "E:\- ProgramLabs\VRCGalleryManager\Icon.ico"
!define MUI_UNICON "E:\- ProgramLabs\VRCGalleryManager\Icon.ico"
; We do not define header images if you don't have them
;!define MUI_HEADERIMAGE
;!define MUI_HEADERIMAGE_RIGHT
;!define MUI_HEADERIMAGE_BITMAP "E:\- ProgramLabs\VRCGalleryManager\header.bmp"

; Define installer pages
!insertmacro MUI_PAGE_WELCOME
; If you don't have license.txt, you can comment out the license page:
; !insertmacro MUI_PAGE_LICENSE "E:\- ProgramLabs\VRCGalleryManager\license.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Default installation directory and registry key for installation directory
InstallDir "$PROGRAMFILES\VRCGalleryManager"
InstallDirRegKey HKLM "Software\VRCGalleryManager" "Install_Dir"

; Installation section
Section "Install"
    SetDetailsView show
    SetOutPath "$INSTDIR"
    
    ; Copy application files
    File /r "E:\- ProgramLabs\VRCGalleryManager\bin\Release\net8.0-windows\publish\win-x86\*.*"
    
    ; Copy the Icon.ico file (ensure it exists)
    File "E:\- ProgramLabs\VRCGalleryManager\Icon.ico"
    
    ; Create shortcuts
    CreateShortcut "$DESKTOP\VRCGalleryManager.lnk" "$INSTDIR\VRCGalleryManager.exe" "" "$INSTDIR\Icon.ico"
    CreateDirectory "$SMPROGRAMS\VRCGalleryManager"
    CreateShortcut "$SMPROGRAMS\VRCGalleryManager\VRCGalleryManager.lnk" "$INSTDIR\VRCGalleryManager.exe" "" "$INSTDIR\Icon.ico"
    
    ; Register the installation directory for future reference
    WriteRegStr HKLM "Software\VRCGalleryManager" "Install_Dir" "$INSTDIR"
    
    ; Write the uninstaller file
    WriteUninstaller "$INSTDIR\Uninstall.exe"
    
    ; ******************************
    ; Add the application to the "Programs and Features" list
    ; ******************************
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "DisplayName" "VRCGalleryManager"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "DisplayIcon" "$INSTDIR\VRCGalleryManager.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "Publisher" "TheIceDragonz"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager" "NoRepair" 1
SectionEnd

; Uninstallation section
Section "Uninstall"
    Delete "$INSTDIR\VRCGalleryManager.exe"
    Delete "$INSTDIR\Uninstall.exe"
    Delete "$DESKTOP\VRCGalleryManager.lnk"
    Delete "$INSTDIR\Icon.ico"
    Delete "$SMPROGRAMS\VRCGalleryManager\VRCGalleryManager.lnk"
    RMDir "$SMPROGRAMS\VRCGalleryManager"
    RMDir /r "$INSTDIR"   ; Remove the entire installation folder recursively
    DeleteRegKey HKLM "Software\VRCGalleryManager"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRCGalleryManager"
SectionEnd

!insertmacro MUI_LANGUAGE "English"
