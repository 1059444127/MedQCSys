@echo off
@echo ***************************************************
@echo ����Ϊ����װ�����ʿز���ϵͳ�����Ժ�...
@echo �汾�ţ�{medqc_version}
@echo �벻Ҫ�ֶ��رձ�����

rem �ȴ�������
ping -n 3 127.0.0.1

@echo ���ڿ����汾�ļ�... 
xcopy "{app_path}\Upgrade" "{app_path}" /e /q /h /r /y
if errorlevel 5 goto err
if errorlevel 4 goto err
if errorlevel 3 goto err
if errorlevel 2 goto err
if errorlevel 1 goto err

@echo ���ڿ��������ļ�... 

xcopy "{app_path}\MedQCSys_Bak.xml" "{app_path}\MedQCSys.xml" /q /y


@echo �������������ʿ�ϵͳ�����Ժ�...
start "���Ӳ���ϵͳ" "{app_path}\mrqc.exe" "{app_args}"

:err
if exist "{app_path}\Upgrade" (rd "{app_path}\Upgrade" /s /q)
if exist "{app_path}\{medqc_version}.zip" (del "{app_path}\{medqc_version}.zip" /q /f)
if exist "{app_path}\MedQCSys_Bak.xml" (del "{app_path}\MedQCSys_Bak.xml" /q /f)
if exist "{app_path}\Upgrade.bat" (del "{app_path}\Upgrade.bat" /q /f)
if exist "{app_path}\AutoUpgrade.bat" (del "{app_path}\AutoUpgrade.bat" /q /f)
