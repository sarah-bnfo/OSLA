@echo off

if "%PROCESSOR_ARCHITECTURE%"=="x86" goto x86
set fr="%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"
csc /nologo /t:library /out:Bin\Mishel64.dll Source\MishelX.cs
vbc /nologo /t:winexe /out:Bin\Rose.exe /win32icon:Resource\rose.ico Source\Rose.vb /r:Bin\Mishel64.dll
goto common
:x86
set fr="C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\WPF"
:common
csc /nologo /t:library /out:Bin\Mishel.dll Source\Mishel.cs
csc /nologo /t:library /out:Bin\SIPCommon.dll Source\SIPCommon.cs /r:Bin\Mishel.dll
csc /nologo /t:winexe /out:Bin\Lisa.exe /win32icon:Resource\lisa.ico Source\Lisa.cs /r:Bin\Mishel.dll /r:Bin\SIPCommon.dll
vbc /nologo /t:winexe /out:Bin\Elsa.exe /win32icon:Resource\elsa.ico Source\Elsa.vb /r:Bin\Mishel.dll /r:Bin\SIPCommon.dll
csc /nologo /t:library /out:Bin\Evita.dll Source\Evita.cs
csc /nologo /t:winexe /out:Bin\Elisa.exe /win32icon:Resource\elisa.ico Source\Lisa.cs Source\SIPCommon.cs /r:Bin\Evita.dll
csc /nologo /t:library /out:Bin\AcceptMany.dll Source\AcceptMany.cs /r:Bin\Evita.dll
csc /nologo /t:library /out:Bin\DoPresentation.dll Source\DoPresentation.cs /r:Bin\Evita.dll /r:System.Xaml.dll /r:%fr%\WindowsBase.dll,%fr%\PresentationCore.dll,%fr%\PresentationFramework.dll

