# .NET 9 vs .NET 10 Runtime Async Benchmark Runner (PowerShell)
# This script provides easy ways to run different benchmark configurations

param(
    [Parameter(Position=0)]
    [ValidateSet("net9-vs-net10", "runtime-async", "all", "build", "check", "help")]
    [string]$Action = "help"
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 .NET 9 vs .NET 10 Runtime Async Benchmark Runner" -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if .NET versions are available
function Test-DotNetVersions {
    Write-Host "🔍 Checking .NET versions..." -ForegroundColor Yellow
    
    $runtimes = dotnet --list-runtimes
    
    # Check .NET 9
    if ($runtimes -match "Microsoft\.NETCore\.App 9\.") {
        Write-Host "✅ .NET 9 runtime found" -ForegroundColor Green
    } else {
        Write-Host "❌ .NET 9 runtime not found" -ForegroundColor Red
        Write-Host "   Please install .NET 9 SDK to run .NET 9 benchmarks" -ForegroundColor Yellow
    }
    
    # Check .NET 10
    if ($runtimes -match "Microsoft\.NETCore\.App 10\.") {
        Write-Host "✅ .NET 10 runtime found" -ForegroundColor Green
    } else {
        Write-Host "❌ .NET 10 runtime not found" -ForegroundColor Red
        Write-Host "   Please install .NET 10 RC 1 SDK to run .NET 10 benchmarks" -ForegroundColor Yellow
    }
    
    Write-Host ""
}

# Function to build the project
function Build-Project {
    Write-Host "🔨 Building benchmark project..." -ForegroundColor Yellow
    
    # Build for .NET 9
    Write-Host "   Building for .NET 9..." -ForegroundColor Gray
    dotnet build -f net9.0 -c Release --no-restore
    
    # Build for .NET 10
    Write-Host "   Building for .NET 10..." -ForegroundColor Gray
    dotnet build -f net10.0 -c Release --no-restore
    
    Write-Host "✅ Build completed" -ForegroundColor Green
    Write-Host ""
}

# Function to restore packages
function Restore-Packages {
    Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore
    Write-Host "✅ Package restore completed" -ForegroundColor Green
    Write-Host ""
}

# Function to run .NET 9 vs .NET 10 comparison
function Start-Net9VsNet10Benchmarks {
    Write-Host "🔄 Running .NET 9 vs .NET 10 comparison benchmarks..." -ForegroundColor Cyan
    Write-Host "=====================================================" -ForegroundColor Cyan
    Write-Host ""
    
    dotnet run -c Release --no-build -- --net9-vs-net10
}

# Function to run .NET 10 with Runtime Async enabled
function Start-RuntimeAsyncEnabledBenchmarks {
    Write-Host "⚡ Running .NET 10 with Runtime Async enabled benchmarks..." -ForegroundColor Cyan
    Write-Host "==========================================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Set environment variable and run
    $env:DOTNET_RuntimeAsync = "1"
    dotnet run -c Release --no-build -f net10.0 -- --runtime-async-enabled
    Remove-Item env:DOTNET_RuntimeAsync
}

# Function to run all benchmarks
function Start-AllBenchmarks {
    Write-Host "🏁 Running all benchmark configurations..." -ForegroundColor Cyan
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host ""
    
    Start-Net9VsNet10Benchmarks
    Write-Host ""
    Start-RuntimeAsyncEnabledBenchmarks
}

# Function to show usage
function Show-Usage {
    Write-Host "📖 Usage: .\run-benchmarks.ps1 [ACTION]" -ForegroundColor White
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor White
    Write-Host "  net9-vs-net10         Run .NET 9 vs .NET 10 (no runtime async) comparison" -ForegroundColor Gray
    Write-Host "  runtime-async         Run .NET 10 with runtime async enabled benchmarks" -ForegroundColor Gray
    Write-Host "  all                   Run all benchmark configurations" -ForegroundColor Gray
    Write-Host "  build                 Build the project only" -ForegroundColor Gray
    Write-Host "  check                 Check .NET version availability" -ForegroundColor Gray
    Write-Host "  help                  Show this help message" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor White
    Write-Host "  .\run-benchmarks.ps1 net9-vs-net10      # Compare .NET 9 vs .NET 10 performance" -ForegroundColor Gray
    Write-Host "  .\run-benchmarks.ps1 runtime-async      # Test .NET 10 runtime async feature" -ForegroundColor Gray
    Write-Host "  .\run-benchmarks.ps1 all               # Run complete benchmark suite" -ForegroundColor Gray
    Write-Host ""
}

# Main script logic
switch ($Action) {
    "net9-vs-net10" {
        Test-DotNetVersions
        Restore-Packages
        Build-Project
        Start-Net9VsNet10Benchmarks
    }
    "runtime-async" {
        Test-DotNetVersions
        Restore-Packages
        Build-Project
        Start-RuntimeAsyncEnabledBenchmarks
    }
    "all" {
        Test-DotNetVersions
        Restore-Packages
        Build-Project
        Start-AllBenchmarks
    }
    "build" {
        Test-DotNetVersions
        Restore-Packages
        Build-Project
    }
    "check" {
        Test-DotNetVersions
    }
    "help" {
        Show-Usage
    }
    default {
        Write-Host "❌ Unknown action: $Action" -ForegroundColor Red
        Write-Host ""
        Show-Usage
        exit 1
    }
}