# Backward-compatible entry point.
# This script has been consolidated into setup-final.ps1.

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $scriptRoot "setup-final.ps1")
