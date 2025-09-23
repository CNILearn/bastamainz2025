#!/bin/bash

# .NET 9 vs .NET 10 Runtime Async Benchmark Runner
# This script provides easy ways to run different benchmark configurations

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR"

echo "🚀 .NET 9 vs .NET 10 Runtime Async Benchmark Runner"
echo "==================================================="
echo ""

# Function to check if .NET versions are available
check_dotnet_versions() {
    echo "🔍 Checking .NET versions..."
    
    # Check .NET 9
    if dotnet --list-runtimes | grep -q "Microsoft.NETCore.App 9."; then
        echo "✅ .NET 9 runtime found"
    else
        echo "❌ .NET 9 runtime not found"
        echo "   Please install .NET 9 SDK to run .NET 9 benchmarks"
    fi
    
    # Check .NET 10
    if dotnet --list-runtimes | grep -q "Microsoft.NETCore.App 10."; then
        echo "✅ .NET 10 runtime found"
    else
        echo "❌ .NET 10 runtime not found"
        echo "   Please install .NET 10 RC 1 SDK to run .NET 10 benchmarks"
    fi
    
    echo ""
}

# Function to build the project
build_project() {
    echo "🔨 Building benchmark project..."
    cd "$PROJECT_DIR"
    
    # Build for .NET 9
    echo "   Building for .NET 9..."
    dotnet build -f net9.0 -c Release --no-restore
    
    # Build for .NET 10
    echo "   Building for .NET 10..."
    dotnet build -f net10.0 -c Release --no-restore
    
    echo "✅ Build completed"
    echo ""
}

# Function to restore packages
restore_packages() {
    echo "📦 Restoring NuGet packages..."
    cd "$PROJECT_DIR"
    dotnet restore
    echo "✅ Package restore completed"
    echo ""
}

# Function to run .NET 9 vs .NET 10 comparison
run_net9_vs_net10() {
    echo "🔄 Running .NET 9 vs .NET 10 comparison benchmarks..."
    echo "====================================================="
    echo ""
    
    cd "$PROJECT_DIR"
    dotnet run -c Release --no-build -- --net9-vs-net10
}

# Function to run .NET 10 with Runtime Async enabled
run_runtime_async_enabled() {
    echo "⚡ Running .NET 10 with Runtime Async enabled benchmarks..."
    echo "=========================================================="
    echo ""
    
    cd "$PROJECT_DIR"
    
    # Set environment variable and run
    export DOTNET_RuntimeAsync=1
    dotnet run -c Release --no-build -f net10.0 -- --runtime-async-enabled
}

# Function to run all benchmarks
run_all_benchmarks() {
    echo "🏁 Running all benchmark configurations..."
    echo "========================================="
    echo ""
    
    run_net9_vs_net10
    echo ""
    run_runtime_async_enabled
}

# Function to show usage
show_usage() {
    echo "📖 Usage: $0 [OPTION]"
    echo ""
    echo "Options:"
    echo "  net9-vs-net10         Run .NET 9 vs .NET 10 (no runtime async) comparison"
    echo "  runtime-async         Run .NET 10 with runtime async enabled benchmarks"
    echo "  all                   Run all benchmark configurations"
    echo "  build                 Build the project only"
    echo "  check                 Check .NET version availability"
    echo "  help                  Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 net9-vs-net10      # Compare .NET 9 vs .NET 10 performance"
    echo "  $0 runtime-async      # Test .NET 10 runtime async feature"
    echo "  $0 all               # Run complete benchmark suite"
    echo ""
}

# Main script logic
case "${1:-help}" in
    "net9-vs-net10")
        check_dotnet_versions
        restore_packages
        build_project
        run_net9_vs_net10
        ;;
    "runtime-async")
        check_dotnet_versions
        restore_packages
        build_project
        run_runtime_async_enabled
        ;;
    "all")
        check_dotnet_versions
        restore_packages
        build_project
        run_all_benchmarks
        ;;
    "build")
        check_dotnet_versions
        restore_packages
        build_project
        ;;
    "check")
        check_dotnet_versions
        ;;
    "help"|"-h"|"--help")
        show_usage
        ;;
    *)
        echo "❌ Unknown option: $1"
        echo ""
        show_usage
        exit 1
        ;;
esac