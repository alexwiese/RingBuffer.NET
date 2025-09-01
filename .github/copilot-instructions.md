# RingBuffer.NET Library
RingBuffer.NET is a C# implementation of a ring (circular) buffer data structure targeting .NET 8.0. It provides both fixed-size (`RingBuffer<T>`) and automatically growing (`GrowingRingBuffer<T>`) variants with full IEnumerable and ICollection support.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively
- **CRITICAL**: This project targets .NET 8.0 and uses modern SDK-style projects. Use `dotnet` commands for all operations.
- Bootstrap and build the repository:
  - No special environment setup required - .NET 8 SDK is already available
  - `cd /path/to/RingBuffer.NET`
  - `dotnet build` -- builds entire solution in ~7 seconds (includes restore)
  - `dotnet build RingBuffer/RingBuffer.csproj` -- builds only main library in ~4 seconds
  - `dotnet test` -- runs all tests in ~3 seconds
- **Testing**: Official MSTest tests work perfectly and run with `dotnet test`.
- Build output: `RingBuffer/bin/Debug/net8.0/RingBuffer.dll`

## Validation
- **ALWAYS validate any changes** by running the existing test suite with `dotnet test`.
- The official test suite in `RingBufferTests/` uses MSTest and works perfectly with .NET 8.
- **MANDATORY validation scenario after any changes:**
  1. Build the solution: `dotnet build`
  2. Run all tests: `dotnet test`
  3. For manual testing, create a console app: `dotnet new console -n ValidationTest`
  4. Add project reference: `dotnet add ValidationTest reference RingBuffer/RingBuffer.csproj`
  5. Run with: `dotnet run --project ValidationTest`

### Manual Validation Test Template (if needed)
```csharp
using System;
using RingBuffer;

class Program {
    static void Main() {
        // Test basic RingBuffer functionality
        var buffer = new RingBuffer<int>(4);
        buffer.Put(1); buffer.Put(2); buffer.Put(3);
        Console.WriteLine($"Size: {buffer.Size} (expected: 3)");
        
        int value = buffer.Get();
        Console.WriteLine($"Got: {value} (expected: 1)");
        
        // Test GrowingRingBuffer if modified
        var growing = new GrowingRingBuffer<int>(2);
        growing.Put(1); growing.Put(2); growing.Put(3);
        Console.WriteLine($"Capacity after growing: {growing.Capacity} (expected: 4)");
        Console.WriteLine("Validation completed successfully!");
    }
}
```

## Build Commands and Timing
- **No environment setup needed**: .NET 8 SDK is already available
- **Main build**: `dotnet build RingBuffer/RingBuffer.csproj` -- ~4 seconds (including restore)
- **Full solution**: `dotnet build` -- ~7 seconds (builds both library and tests)
- **Run tests**: `dotnet test` -- ~3 seconds (runs all 9 MSTest unit tests)
- **Manual validation**: `dotnet new console && dotnet add reference && dotnet run` -- ~10 seconds

## Repository Structure
```
RingBuffer.NET/
├── RingBuffer/              # Main library project
│   ├── RingBuffer.cs        # Core fixed-size ring buffer implementation
│   ├── GrowingRingBuffer.cs # Auto-growing ring buffer variant
│   └── RingBuffer.csproj    # Modern SDK-style project file (.NET 8)
├── RingBufferTests/         # Unit tests (MSTest - works perfectly)
│   ├── RingBufferTests.cs   # Test cases
│   └── RingBufferTests.csproj # Modern SDK-style test project (.NET 8)
├── RingBuffer.sln           # Visual Studio solution file
├── global.json              # SDK version specification
└── README.md                # Project documentation
```

## Key Classes and Functionality
- **`RingBuffer<T>`**: Fixed-capacity circular buffer with overflow protection
  - Constructor options: `RingBuffer()`, `RingBuffer(capacity)`, `RingBuffer(capacity, allowOverflow)`
  - Key methods: `Put(item)`, `Get()`, `Clear()`, `Contains(item)`, `Remove(item)`
  - Properties: `Size`, `Capacity`, `AllowOverflow`
- **`GrowingRingBuffer<T>`**: Automatically expands when full
  - Constructor: `GrowingRingBuffer()`, `GrowingRingBuffer(startCapacity)`
  - Doubles capacity when overflow would occur

## Common Development Tasks
1. **Making changes to core logic**:
   - Edit `RingBuffer/RingBuffer.cs` for fixed-size buffer
   - Edit `RingBuffer/GrowingRingBuffer.cs` for growing buffer
   - Build: `dotnet build RingBuffer/RingBuffer.csproj`
   - Test: `dotnet test`

2. **Adding new functionality**:
   - Add methods to appropriate class
   - Ensure thread-safety considerations (current implementation is not thread-safe)
   - Write unit tests in `RingBufferTests/RingBufferTests.cs`
   - Run tests: `dotnet test`
   - Update this documentation if public API changes

3. **Performance modifications**:
   - Focus on `addToBuffer()`, `Get()`, and `Put()` methods
   - Test with larger buffer sizes using the existing test suite
   - Consider memory allocation patterns in GrowingRingBuffer
   - Validate with: `dotnet test`

## Troubleshooting
- **Error: "A compatible .NET SDK was not found"** → Check `global.json` for correct SDK version (should be 8.0.0)
- **Error: "TargetFramework 'net9.0' not found"** → Update project files to use `net8.0` instead of `net9.0`
- **Error: Build warnings about file copying** → These are harmless MSBuild warnings, builds still succeed
- **Error: "dotnet command not found"** → .NET SDK should be pre-installed, but verify with `dotnet --version`

## What NOT to Do
- Do not try to build with legacy `xbuild` or `msbuild` - use `dotnet build` instead
- Do not install Mono or mono-mcs - not needed for .NET 8 development
- Do not skip running `dotnet test` - comprehensive test suite is available and fast
- Do not modify framework target without testing - library maintains compatibility across modern .NET versions

## Development Environment Notes
- **OS Compatibility**: Builds and runs on Linux, Windows, and macOS with .NET 8 SDK
- **IDE Support**: Works with VS Code, Visual Studio, Rider, or any text editor with dotnet CLI
- **Dependencies**: Modern NuGet package references for MSTest framework
- **Output**: Produces standard .NET assembly (RingBuffer.dll) compatible with any .NET 8+ application

## Expected Command Outputs

### Repository structure:
```bash
$ ls -la
drwxr-xr-x 4 user user 4096 date .
drwxr-xr-x 3 user user 4096 date ..
drwxr-xr-x 8 user user 4096 date .git
-rw-r--r-- 1 user user 4606 date .gitignore
-rw-r--r-- 1 user user 1084 date LICENSE
-rw-r--r-- 1 user user 1301 date README.md
drwxr-xr-x 3 user user 4096 date RingBuffer
-rw-r--r-- 1 user user  905 date RingBuffer.sln
drwxr-xr-x 3 user user 4096 date RingBufferTests
```

### Successful main library build:
```bash
$ dotnet build RingBuffer/RingBuffer.csproj
MSBuild version 17.8.32+74df0b3f5 for .NET
  Determining projects to restore...
  Restored /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBuffer/RingBuffer.csproj (in 562 ms).
  RingBuffer -> /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBuffer/bin/Debug/net8.0/RingBuffer.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:06.76
```

### Build output contents:
```bash
$ ls -la RingBuffer/bin/Debug/net8.0/
total 20
drwxr-xr-x 2 runner docker 4096 date .
drwxr-xr-x 3 runner docker 4096 date ..
-rw-r--r-- 1 runner docker 8192 date RingBuffer.dll
-rw-r--r-- 1 runner docker 2048 date RingBuffer.pdb
```

### Full solution build with tests:
```bash
$ dotnet build
MSBuild version 17.8.32+74df0b3f5 for .NET
  Determining projects to restore...
  Restored /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBufferTests/RingBufferTests.csproj (in 2.86 sec).
  1 of 2 projects are up-to-date for restore.
  RingBuffer -> /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBuffer/bin/Debug/net8.0/RingBuffer.dll
  RingBufferTests -> /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBufferTests/bin/Debug/net8.0/RingBufferTests.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:06.56
```

### Running tests:
```bash
$ dotnet test
Determining projects to restore...
  1 of 2 projects are up-to-date for restore.
  RingBuffer -> /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBuffer/bin/Debug/net8.0/RingBuffer.dll
  RingBufferTests -> /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBufferTests/bin/Debug/net8.0/RingBufferTests.dll
Test run for /home/runner/work/RingBuffer.NET/RingBuffer.NET/RingBufferTests/bin/Debug/net8.0/RingBufferTests.dll (.NETCoreApp,Version=v8.0)
Microsoft (R) Test Execution Command Line Tool Version 17.8.0 (x64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9, Duration: 92 ms - RingBufferTests.dll (net8.0)
```

### Manual validation (if needed):
```bash
$ dotnet new console -n ValidationTest
The template "Console App" was created successfully.

$ cd ValidationTest && dotnet add reference ../RingBuffer/RingBuffer.csproj
Reference `..\..\RingBuffer\RingBuffer.csproj` added to the project.

$ dotnet run
Size: 3 (expected: 3)
Got: 1 (expected: 1)
Capacity after growing: 4 (expected: 4)
Validation completed successfully!
```