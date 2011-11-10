RequestExecutionLevel admin

; Include Logic Lib
!include LogicLib.nsh

; Include Modern UI 2
!include "MUI2.nsh"

; Include DotNetChecker
!include "DotNetChecker.nsh"

Name "WinBMA"
Caption "WinBMA ${APP_VERSION} Setup"

OutFile "bin/WinBMA_${APP_VERSION}_Setup.exe"

VIAddVersionKey "ProductName" "WinBMA"
VIAddVersionKey "CompanyName" "WinBMA/Andrew Moore"
VIAddVersionKey "LegalCopyright" "© 2011 WinBMA/Andrew Moore"
VIAddVersionKey "FileDescription" "WinBMA ${APP_VERSION} Installer"
VIAddVersionKey "FileVersion" "${APP_VERSION}"

VIProductVersion "${APP_VERSION}"

; The default installation directory
InstallDir $PROGRAMFILES\WinBMA

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\WinBMA" "InstallPath"

!define MUI_ICON "resources\box_software.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "resources\header_image.bmp"
!define MUI_HEADERIMAGE_RIGHT
!define MUI_WELCOMEFINISHPAGE_BITMAP "resources\welcome.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "resources\welcome.bmp"
!define MUI_WELCOMEPAGE_TITLE "Welcome to the WinBMA ${APP_VERSION} Setup Wizard"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\Resources\License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH
  
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "!WinBMA" SecWinBMA
  SectionIn RO

  SetOutPath "$INSTDIR"
  
  !insertmacro CheckNetFramework 40Client
  
  ;Store installation folder
  WriteRegStr HKLM "Software\WinBMA" "InstallPath" "$INSTDIR"
  
  ;Install Files
  File ..\bin\Release\WinBMA.exe
  File ..\bin\Release\*.dll
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "DisplayName" "WinBMA"

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "DisplayIcon" "$INSTDIR\WinBMA.exe"

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "InstallLocation" "$INSTDIR"

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "Publisher" "WinBMA/Andrew Moore"

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "URLInfoAbout" "http://code.google.com/p/winbma/"
				 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "URLUpdateInfo" "http://code.google.com/p/winbma/"
				 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "DisplayVersion" "${APP_VERSION}"

  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "NoModify" 1

  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "NoRepair" 1

  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""

  Call GetInstalledSize
  Pop $0
  
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA" \
                 "EstimatedSize" $0
SectionEnd

Section "Start Menu Shortcut" SecStartMenuShortcut
	SetShellVarContext All
	CreateDirectory "$SMPROGRAMS\WinBMA"
	CreateShortCut "$SMPROGRAMS\WinBMA\WinBMA.lnk" "$INSTDIR\WinBMA.exe"
SectionEnd

Section /o "Desktop Shortcut" SecDesktopShortcut
	SetShellVarContext All
	CreateShortCut "$DESKTOP\WinBMA.lnk" "$INSTDIR\WinBMA.exe"
SectionEnd

LangString DESC_SecWinBMA ${LANG_ENGLISH} "WinBMA Version ${APP_VERSION}"

;Assign language strings to sections
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecWinBMA} $(DESC_SecWinBMA)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Section "Uninstall"
  Delete "$INSTDIR\Uninstall.exe"

  SetShellVarContext All

  IfFileExists "$SMPROGRAMS\WinBMA\WinBMA.lnk" 0 +3
    Delete "$SMPROGRAMS\WinBMA\WinBMA.lnk"
	RMDir "$SMPROGRAMS\WinBMA"

  IfFileExists "$DESKTOP\WinBMA.lnk" 0 +2
    Delete "$SMPROGRAMS\WinBMA.lnk"

  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\WinBMA.exe"
  RMDir "$INSTDIR"

  DeleteRegValue HKLM "Software\WinBMA" "InstallPath"
  DeleteRegKey /ifempty HKLM "Software\WinBMA"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinBMA"
SectionEnd
 
; Return on top of stack the total size of the selected (installed) sections, formated as DWORD
; Assumes no more than 256 sections are defined
Var GetInstalledSize.total
Function GetInstalledSize
	Push $0
	Push $1
	StrCpy $GetInstalledSize.total 0
	${ForEach} $1 0 256 + 1
		${if} ${SectionIsSelected} $1
			SectionGetSize $1 $0
			IntOp $GetInstalledSize.total $GetInstalledSize.total + $0
		${Endif}
	${Next}
	Pop $1
	Pop $0
	IntFmt $GetInstalledSize.total "0x%08X" $GetInstalledSize.total
	Push $GetInstalledSize.total
FunctionEnd
