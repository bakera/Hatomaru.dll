@echo off
:START
echo �R���p�C��...
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:library /out:hatomaru.dll /r:C:\Users\bakera\.NET\hatomarudll\regexp\bakerareg.dll; /recurse:*.cs 


IF ERRORLEVEL 1 GOTO ERROR

echo �A�Z���u���o�[�W�����̃Z�b�g...
C:\Users\bakera\.NET\tool\buildup\buildup.exe C:\Users\bakera\.NET\hatomaru.dll\assemblyinfo.cs

GOTO END

:ERROR
echo �R���p�C���G���[�ł��B
echo �����L�[�������ƍēx�R���p�C�������݂܂��BCtrl+C�Œ��~���܂��B
pause
goto START

:END
copy hatomaru.dll C:\bakera.jp\home\site\wwwroot\bin\hatomaru.dll
pause
