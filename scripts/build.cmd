@ECHO OFF

IF "%1" == "" (

  @powershell -NoProfile -ExecutionPolicy unrestricted -File "build.ps1" "-buildParams /p:Configuration=Release"
  
) else (

  @powershell -NoProfile -ExecutionPolicy unrestricted -File "build.ps1" -buildParams "/p:Configuration=%1%"
)