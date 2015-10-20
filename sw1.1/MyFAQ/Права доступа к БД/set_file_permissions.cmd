@echo off

call :set_file_permissions *.sq3 
call :set_file_permissions *.mdb
call :set_file_permissions *.txt 

goto :EOF

:set_file_permissions
call :set_permissions %1 everyone
rem call :set_permissions %1 Все
goto :EOF

:set_permissions
set _FILE=%1
set _SID=%2
iCACLS "%_FILE%" /inheritance:d
iCACLS "%_FILE%" /remove:g %_SID%
iCACLS "%_FILE%" /grant:r %_SID%:RW
goto :EOF
  
