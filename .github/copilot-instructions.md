# RingBuffer.NET Library
RingBuffer.NET is a C# implementation of a ring (circular) buffer data structure targeting .NET Framework 3.5. It provides both fixed-size (`RingBuffer<T>`) and automatically growing (`GrowingRingBuffer<T>`) variants with full IEnumerable and ICollection support.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively
- **CRITICAL**: This project targets .NET Framework 3.5 and requires Mono on Linux systems. Do NOT attempt to build with `dotnet` as it will fail.
- Bootstrap and build the repository:
  - `sudo apt update && sudo apt install -y mono-complete mono-mcs` -- takes 2-3 minutes. NEVER CANCEL.
  - `cd /path/to/RingBuffer.NET`
  - `xbuild RingBuffer.sln` -- builds entire solution, main library succeeds in < 1 second
  - `xbuild RingBuffer/RingBuffer.csproj` -- builds only main library in < 1 second
- **Testing limitations**: Official MSTest tests cannot run on Linux/Mono. Use manual validation instead.
- Build output: `RingBuffer/bin/Debug/RingBuffer.dll`

## Validation
- **ALWAYS manually validate any changes** by creating a simple test program (see template below).
- The official test suite in `RingBufferTests/` uses MSTest which is not available in Mono.
- **MANDATORY validation scenario after any changes:**
  1. Create a test program that exercises the modified functionality
  2. Compile with: `mcs TestProgram.cs -r:RingBuffer/bin/Debug/RingBuffer.dll -out:Test.exe`
  3. Run with: `mono Test.exe`
  4. Verify expected behavior matches actual output

### Validation Test Template
```csharp
using System;
using RingBuffer;

class ValidationTest {
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
    }
}
```

## Build Commands and Timing
- **Environment setup**: `sudo apt install mono-complete mono-mcs` -- 2-3 minutes, NEVER CANCEL
- **Main build**: `xbuild RingBuffer/RingBuffer.csproj` -- < 1 second
- **Full solution**: `xbuild RingBuffer.sln` -- < 1 second (tests will fail, library succeeds)
- **Manual validation**: Compile and run test -- < 30 seconds

## Repository Structure
```
RingBuffer.NET/
├── RingBuffer/              # Main library project
│   ├── RingBuffer.cs        # Core fixed-size ring buffer implementation
│   ├── GrowingRingBuffer.cs # Auto-growing ring buffer variant
│   └── RingBuffer.csproj    # Project file
├── RingBufferTests/         # Unit tests (MSTest - Linux incompatible)
│   ├── RingBufferTests.cs   # Test cases
│   └── RingBufferTests.csproj
├── RingBuffer.sln           # Visual Studio solution file
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
   - Build: `xbuild RingBuffer/RingBuffer.csproj`
   - Validate with custom test program

2. **Adding new functionality**:
   - Add methods to appropriate class
   - Ensure thread-safety considerations (current implementation is not thread-safe)
   - Create validation test covering new functionality
   - Update this documentation if public API changes

3. **Performance modifications**:
   - Focus on `addToBuffer()`, `Get()`, and `Put()` methods
   - Test with larger buffer sizes in validation
   - Consider memory allocation patterns in GrowingRingBuffer

## Troubleshooting
- **Error: "dotnet build fails with framework not found"** → Use `xbuild` instead of `dotnet`
- **Error: "MSTest assembly not found"** → Expected on Linux, use manual validation
- **Error: "RingBuffer.dll not found when running test"** → Copy DLL to test directory: `cp RingBuffer/bin/Debug/RingBuffer.dll .`
- **Error: "xbuild command not found"** → Install Mono: `sudo apt install mono-complete`

## What NOT to Do
- Do not try to build with `dotnet build` - it will fail due to .NET Framework 3.5 target
- Do not attempt to run `RingBufferTests.cs` directly - MSTest is not available on Linux
- Do not modify framework target to newer versions without extensive testing - library maintains .NET 2.0+ compatibility
- Do not skip manual validation - there are no automated tests available on Linux

## Development Environment Notes
- **OS Compatibility**: Builds and runs on Linux via Mono, originally designed for Windows/.NET Framework
- **IDE Support**: Works with MonoDevelop, VS Code with Mono extensions, or any text editor
- **Dependencies**: Only requires System.dll and System.Core.dll (both included in Mono)
- **Output**: Produces standard .NET assembly (RingBuffer.dll) compatible with any .NET application

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
$ xbuild RingBuffer/RingBuffer.csproj
>>>> xbuild tool is deprecated and will be removed in future updates, use msbuild instead <<<<
XBuild Engine Version 14.0
Build started [timestamp].
Project "/path/to/RingBuffer.csproj" (default target(s)):
    Target PrepareForBuild: Configuration: Debug Platform: AnyCPU
    Target CoreCompile: Tool /usr/lib/mono/4.5/mcs.exe execution started...
    Target DeployOutputFiles: Copying file from 'obj/Debug/RingBuffer.dll' to 'bin/Debug/RingBuffer.dll'
Done building project.

Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:00.3292140
```

### Build output contents:
```bash
$ ls -la RingBuffer/bin/Debug/
total 20
drwxr-xr-x 2 user user 4096 date .
drwxr-xr-x 3 user user 4096 date ..
-rwxr-xr-x 1 user user 8192 date RingBuffer.dll
-rw-r--r-- 1 user user 2299 date RingBuffer.dll.mdb
```

### Full solution build (with expected test failure):
```bash
$ xbuild RingBuffer.sln
[... main library builds successfully ...]
Project "RingBufferTests.csproj" (default target(s)):
    Target ResolveAssemblyReferences:
/usr/lib/mono/xbuild/14.0/bin/Microsoft.Common.targets: warning: Reference 'Microsoft.VisualStudio.QualityTools.UnitTestFramework' not resolved
    Target CoreCompile:
RingBufferTests.cs(21,17): error CS0234: The type or namespace name 'VisualStudio' does not exist in the namespace 'Microsoft'

Build FAILED.
    1 Warning(s)
    1 Error(s)
```

### Validation test compilation and execution:
```bash
$ mcs ValidationTest.cs -r:RingBuffer/bin/Debug/RingBuffer.dll -out:Test.exe
# (no output = success)

$ mono Test.exe
Size: 3 (expected: 3)
Got: 1 (expected: 1)
Capacity after growing: 4 (expected: 4)
```

### Expected failure with dotnet:
```bash
$ dotnet build RingBuffer/RingBuffer.csproj
MSBuild version 17.8.32+74df0b3f5 for .NET
error MSB3644: The reference assemblies for .NETFramework,Version=v3.5 were not found.
Build FAILED.
```