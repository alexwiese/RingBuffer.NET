# RingBuffer.NET

RingBuffer.NET is a C# library providing generic ring buffer implementations with fixed-size (`RingBuffer<T>`) and auto-expanding (`GrowingRingBuffer<T>`) variants. The library implements standard .NET collection interfaces and is distributed as a NuGet package.

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Prerequisites and Setup
- Install .NET 9.0 SDK (required - do NOT use .NET 8.0):
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.100
  export PATH="$HOME/.dotnet:$PATH"
  ```
- Verify installation: `dotnet --version` (should show 9.0.100 or later)

### Build and Test Commands
- **Restore packages**: `dotnet restore` -- takes ~4 seconds
- **Build solution**: `dotnet build` -- takes ~5-6 seconds. Set timeout to 60+ seconds
- **Run tests**: `dotnet test` -- takes ~2-3 seconds, runs 9 MSTest unit tests. Set timeout to 30+ seconds
- **Build release**: `dotnet build --configuration Release` -- takes ~5-6 seconds
- **Create NuGet package**: `dotnet pack --configuration Release` -- takes ~2 seconds

### Code Quality
- **Format code**: `dotnet format` -- ALWAYS run before committing (existing code has formatting issues)
- **Check formatting**: `dotnet format --verify-no-changes` -- returns exit code 2 if formatting needed
- **CRITICAL**: Always run `dotnet format` before committing changes or CI equivalent will fail

## Validation Scenarios

After making any changes to the core library code, ALWAYS validate functionality by running this test:

```bash
# Create test project
dotnet new console -n ValidationTest
cd ValidationTest
dotnet add reference ../RingBuffer/RingBuffer.csproj

# Replace Program.cs with validation code:
cat > Program.cs << 'EOF'
using System;
using RingBuffer;

class Program 
{
    static void Main()
    {
        Console.WriteLine("Testing RingBuffer...");
        
        // Test basic RingBuffer
        var buffer = new RingBuffer<int>(4);
        buffer.Put(1);
        buffer.Put(2);
        buffer.Put(3);
        
        Console.WriteLine($"Size: {buffer.Size}, Capacity: {buffer.Capacity}");
        
        // Test enumeration
        foreach (int item in buffer)
            Console.WriteLine($"Item: {item}");
        
        // Test retrieval
        int first = buffer.Get();
        Console.WriteLine($"Got: {first}");
        
        // Test GrowingRingBuffer
        var growing = new GrowingRingBuffer<string>(2);
        for (int i = 0; i < 5; i++)
        {
            growing.Put($"Item{i}");
            Console.WriteLine($"Added Item{i}, capacity now: {growing.Capacity}");
        }
        
        Console.WriteLine("All validations passed!");
    }
}
EOF

# Run validation
dotnet run
```

Expected output shows:
- Basic buffer operations working correctly
- Enumeration functioning
- GrowingRingBuffer expanding from capacity 2 to 4 to 6
- No exceptions thrown

## Repository Structure

### Key Files and Locations
```
RingBuffer.NET/
├── RingBuffer/
│   ├── RingBuffer.cs              # Main fixed-size ring buffer implementation
│   ├── GrowingRingBuffer.cs       # Auto-expanding variant
│   └── RingBuffer.csproj          # Library project file
├── RingBufferTests/
│   ├── RingBufferTests.cs         # Complete unit test suite (9 tests)
│   └── RingBufferTests.csproj     # Test project file
├── RingBuffer.sln                 # Visual Studio solution file
├── global.json                    # Specifies .NET 9.0 requirement
├── README.md                      # Project documentation
└── LICENSE                        # GNU GPL V3.0 license
```

### Important Implementation Details
- **RingBuffer<T>**: Fixed capacity, throws exception on overflow (unless AllowOverflow=true)
- **GrowingRingBuffer<T>**: Automatically expands by original capacity when full
- **Interfaces**: Implements `IEnumerable<T>`, `ICollection<T>`, `ICollection`
- **Thread Safety**: NOT thread-safe (IsSynchronized returns false)
- **Default Capacity**: 4 elements for both buffer types

## Common Tasks

### Working with the Library Code
- **Main logic**: Focus on `RingBuffer/RingBuffer.cs` for core functionality
- **Growth behavior**: Check `RingBuffer/GrowingRingBuffer.cs` for auto-expansion logic
- **Key methods**: `Put(item)`, `Get()`, `Clear()`, `Contains(item)`, `Remove(item)`

### Testing Changes
1. ALWAYS run the full test suite: `dotnet test`
2. Run manual validation scenario (see above)
3. Test both buffer types in your validation
4. Verify exception handling for empty buffers

### Before Committing
1. **Format code**: `dotnet format`
2. **Build clean**: `dotnet build --configuration Release`
3. **Run all tests**: `dotnet test`
4. **Run validation scenario**: Create and run the test program above

### Package Creation
- Generate NuGet package: `dotnet pack --configuration Release`
- Package location: `RingBuffer/bin/Release/RingBuffer.1.0.0.nupkg`
- Package includes both RingBuffer<T> and GrowingRingBuffer<T> classes

## Development Notes

- **License**: GNU GPL V3.0 - all derivatives must maintain this license
- **Target Framework**: .NET 9.0 (specified in global.json)
- **No External Dependencies**: Library uses only standard .NET types
- **Memory Management**: Uses standard arrays, no unsafe code
- **Performance**: Designed for high-performance scenarios with minimal allocations

## Troubleshooting

- **"SDK not found" error**: Install .NET 9.0 SDK using the curl command above
- **Formatting issues**: Run `dotnet format` to fix automatically
- **Test failures**: Check if changes broke core Put/Get/enumeration logic
- **Build errors**: Ensure you're using .NET 9.0 SDK, not .NET 8.0

## Timing Expectations

All operations are very fast in this simple library:
- Restore: ~4 seconds
- Build: ~5-6 seconds  
- Tests: ~2-3 seconds
- Format: <1 second
- Pack: ~2 seconds

**NEVER CANCEL** any of these operations - they complete quickly but allow 30-60 second timeouts for safety.