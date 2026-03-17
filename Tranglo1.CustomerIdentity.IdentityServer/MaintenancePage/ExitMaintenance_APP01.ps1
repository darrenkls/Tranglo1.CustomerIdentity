# ===========================
# Post-Deployment: Disable Maintenance Mode
# ===========================
# Run this after deployment
# Purpose: Remove app_offline.htm from IIS app root

# List all IIS application physical paths here
$applications = @(
    "D:\Tranglo1\IdentityService"
)

foreach ($app in $applications) {
    $offlineFile = Join-Path $app "app_offline.htm"
    if (Test-Path $offlineFile) {
        try {
            Write-Host "Removing $offlineFile ..." -ForegroundColor Cyan
            Remove-Item -Path $offlineFile -Force
            Write-Host "Maintenance mode disabled for $app" -ForegroundColor Green
        }
        catch {
            Write-Host "Failed to remove $offlineFile : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "No app_offline.htm found in $app (already active)" -ForegroundColor Yellow
    }
}
