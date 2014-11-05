@echo off
:START
echo コンパイル...
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:library /out:hatomaru.dll /r:C:\Users\bakera\.NET\hatomarudll\regexp\bakerareg.dll; /recurse:*.cs 


IF ERRORLEVEL 1 GOTO ERROR

echo アセンブリバージョンのセット...
C:\Users\bakera\.NET\tool\buildup\buildup.exe C:\Users\bakera\.NET\hatomaru.dll\assemblyinfo.cs

GOTO END

:ERROR
echo コンパイルエラーです。
echo 何かキーを押すと再度コンパイルを試みます。Ctrl+Cで中止します。
pause
goto START

:END
copy hatomaru.dll C:\bakera.jp\home\site\wwwroot\bin\hatomaru.dll
pause
