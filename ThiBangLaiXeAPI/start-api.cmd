@echo off
setlocal

echo [INFO] Stopping old HeThongThiBangLai.Api instances (if any)...
taskkill /F /IM HeThongThiBangLai.Api.exe >nul 2>&1

echo [INFO] Starting API with launch profile http...
dotnet run --project .\HeThongThiBangLai.Api\HeThongThiBangLai.Api.csproj --launch-profile http
