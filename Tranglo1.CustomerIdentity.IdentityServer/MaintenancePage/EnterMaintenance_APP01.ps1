# ===========================
# Pre-Deployment: Enable Maintenance Mode
# ===========================
# Run this before deployment
# Purpose: Copy app_offline.htm into IIS app root

$maintenanceFile = "D:\MaintenancePage\app_offline.htm"   # maintenance page path

# List all IIS application physical paths here
$applications = @(
    "D:\Tranglo1\IdentityService"
)

foreach ($app in $applications) {
    try {
        Write-Host "Copying app_offline.htm into $app ..." -ForegroundColor Cyan
        Copy-Item -Path $maintenanceFile -Destination (Join-Path $app "app_offline.htm") -Force
        Write-Host "$app maintenance mode enabled" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to copy to $app : $($_.Exception.Message)" -ForegroundColor Red
    }
}
