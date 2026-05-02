@echo off
chcp 65001 >nul
set "MANAGED="
for %%P in ("%ProgramFiles(x86)%\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed" "D:\SteamLibrary\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed" "E:\SteamLibrary\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed") do if exist "%%P\Assembly-CSharp.dll" set "MANAGED=%%~P"
if not defined MANAGED (echo [!] Вставь путь к Managed: & set /p "MANAGED=Path: ")
if not exist "%MANAGED%\Assembly-CSharp.dll" (echo [X] DLL не найдена & pause & exit /b)

echo [>] Компиляция...
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /t:library /out:AutomaticGeyserCalculation.dll ^
  /r:"%MANAGED%\Assembly-CSharp.dll" ^
  /r:"%MANAGED%\Assembly-CSharp-firstpass.dll" ^
  /r:"%MANAGED%\UnityEngine.CoreModule.dll" ^
  /r:"%MANAGED%\netstandard.dll" ^
  /r:"%MANAGED%\0Harmony.dll" ^
  /platform:x64 /optimize+ /nologo /utf8output Mod.cs > build_log.txt 2>&1

if exist AutomaticGeyserCalculation.dll (
    mkdir "%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\AutomaticGeyserCalculation" 2>nul
    copy AutomaticGeyserCalculation.dll "%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\AutomaticGeyserCalculation\" >nul
    copy mod.yaml "%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\AutomaticGeyserCalculation\" >nul
    copy mod_info.yaml "%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\AutomaticGeyserCalculation\" >nul
    echo [OK] Готово. Папка: %USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\AutomaticGeyserCalculation\
) else (
    echo [X] Ошибка. Смотри build_log.txt
    type build_log.txt
)
pause