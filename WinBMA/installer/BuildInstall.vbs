Dim args

Set args = WScript.Arguments

If args.Count <> 2 Then
	WScript.Echo("You need to specify build configuration and project directory to run this script.")
	WScript.Quit
End If

Dim buildConfiguration, projectDirectory
buildConfiguration = args.Item(0)
projectDirectory = args.Item(1)

If buildConfiguration <> "Release" Then
	WScript.Quit
End If

Dim fso
Set fso = CreateObject("Scripting.FileSystemObject")

Dim assemblyVersion

assemblyVersion = fso.GetFileVersion(projectDirectory & "bin\Release\WinBMA.exe")

Dim nsisMakePath

If fso.FileExists("C:\Program Files\NSIS\makensis.exe") Then
	nsisMakePath = "C:\Program Files\NSIS\makensis.exe"
ElseIf fso.FileExists("C:\Program Files (x86)\NSIS\makensis.exe") Then
	nsisMakePath = "C:\Program Files (x86)\NSIS\makensis.exe"
Else
	WScript.Echo("Could not find NSIS")
End If

Dim shell
Set shell = CreateObject("WScript.Shell")

shell.Run("""" & nsisMakePath & """ /DAPP_VERSION=" & assemblyVersion & " """ & projectDirectory & "installer\Installer.nsi""")
