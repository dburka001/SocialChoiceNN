dim fso: set fso = CreateObject("Scripting.FileSystemObject")
dim CurrentDirectory
CurrentDirectory = fso.GetAbsolutePathName(".")
dim Directory
Directory = fso.BuildPath(CurrentDirectory, "NNVotingController\NNVotingController\bin\Debug\NNVotingController.exe")
WScript.CreateObject("WScript.Shell").Run Chr(34) & Directory & Chr(34), 1