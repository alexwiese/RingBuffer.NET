# RingBuffer.NET
RingBuffer.NET is a C# .NET 9.0 library that provides generic ring buffer (circular buffer) implementations with fixed capacity and auto-expanding variants.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Initial Setup
- Install .NET 9.0 SDK: `wget https://dot.net/v1/dotnet-install.sh && chmod +x dotnet-install.sh && ./dotnet-install.sh --channel 9.0`
- Add to PATH: `export PATH="$HOME/.dotnet:$PATH"`
- Verify installation: `dotnet --version` (should show 9.0.x)

### Build and Test the Repository
- Restore packages: `dotnet restore` -- takes 1-2 seconds
- Build solution: `dotnet build` -- takes 1-2 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- Build release: `dotnet build -c Release` -- takes 1-2 seconds
- Run tests: `dotnet test` -- takes 2-3 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- Create NuGet package: `dotnet pack -c Release` -- takes 1-2 seconds

### Code Quality and Formatting
- Format code: `dotnet format` -- applies .editorconfig rules automatically
- Verify formatting: `dotnet format --verify-no-changes` -- exits with code 2 if formatting needed
- **ALWAYS** run `dotnet format` before committing changes or CI builds will fail

### Validation
- **ALWAYS** build and test your changes after making any code modifications
- **ALWAYS** run `dotnet format` to ensure code formatting is correct
- **CRITICAL**: Manual validation scenarios:
  1. Create a RingBuffer<int> with capacity 4, add 4 items, verify size and capacity
  2. Get items from buffer, verify FIFO behavior and size decreases
  3. Add more items after getting some, verify circular behavior
  4. Create a GrowingRingBuffer<string> with capacity 2, add 3 items using Put() method
  5. Verify the buffer auto-expands to capacity 4 when the third item is added
- Test functionality with: Create a console project, add reference to RingBuffer.csproj, and run test scenarios
  ```bash
  dotnet new console -n TestApp
  cd TestApp
  dotnet add reference /path/to/RingBuffer.csproj
  # Copy validation code and run: dotnet run
  ```

## Key Projects in this Codebase

### RingBuffer Project (/RingBuffer/)
Main library project containing:
- **RingBuffer.cs**: Fixed-capacity circular buffer implementation
- **GrowingRingBuffer.cs**: Auto-expanding circular buffer implementation
- **Target Framework**: net9.0
- **Key Classes**:
  - `RingBuffer<T>`: Implements IEnumerable<T>, ICollection<T> with fixed capacity
  - `GrowingRingBuffer<T>`: Extends RingBuffer<T> with auto-expansion capability

### RingBufferTests Project (/RingBufferTests/)
Unit test project using MSTest framework:
- **RingBufferTests.cs**: Comprehensive test suite with 9 tests
- **Framework**: MSTest v3.1.1
- **Coverage**: Tests both RingBuffer and GrowingRingBuffer functionality

## Important API Usage Notes
- **RingBuffer<T>**: Use `Add(item)` or `Put(item)` to add items, `Get()` to retrieve FIFO
- **GrowingRingBuffer<T>**: Use `Put(item)` for auto-expansion, `Add(item)` calls base class Put() which may not expand
- **Key Properties**: `Size` (current items), `Capacity` (maximum items), `AllowOverflow`
- **Iteration**: Implements IEnumerable<T>, can use foreach loops

## Common Commands Reference

### Repository Structure
```
/RingBuffer.NET/
├── .editorconfig          # Code formatting rules
├── .gitignore            # Git ignore patterns  
├── global.json           # .NET SDK version (9.0.119)
├── LICENSE               # GNU GPL v3.0
├── README.md             # Project documentation
├── RingBuffer.sln        # Solution file
├── RingBuffer/           # Main library project
│   ├── RingBuffer.csproj
│   ├── RingBuffer.cs
│   └── GrowingRingBuffer.cs
└── RingBufferTests/      # Test project
    ├── RingBufferTests.csproj
    └── RingBufferTests.cs
```

### Build Artifacts
- Debug builds: `*/bin/Debug/net9.0/`
- Release builds: `*/bin/Release/net9.0/`
- NuGet packages: `RingBuffer/bin/Release/*.nupkg`

### Timing Expectations
- **dotnet restore**: 1-2 seconds
- **dotnet build**: 1-2 seconds
- **dotnet test**: 2-3 seconds  
- **dotnet pack**: 1-2 seconds
- **Total clean build + test**: ~5 seconds

### Common Gotchas
- The project targets .NET 9.0 - ensure you have the correct SDK installed
- GrowingRingBuffer requires using `Put(item)` method for auto-expansion, not `Add(item)`
- Always export PATH with .dotnet location: `export PATH="$HOME/.dotnet:$PATH"`
- Code formatting is enforced - run `dotnet format` before committing