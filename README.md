# 🔄 RingBuffer.NET

[![Build Status](https://img.shields.io/github/actions/workflow/status/alexwiese/RingBuffer.NET/dotnet.yml?branch=main&style=flat-square&logo=github)](https://github.com/alexwiese/RingBuffer.NET/actions)
[![NuGet Version](https://img.shields.io/nuget/v/RingBuffer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RingBuffer/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RingBuffer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RingBuffer/)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg?style=flat-square)](https://www.gnu.org/licenses/gpl-3.0)

> **High-performance, thread-safe circular buffer implementations for .NET 9.0**

A modern, generic ring buffer (circular buffer) library for C# that provides both fixed-capacity and auto-expanding variants. Built with the latest .NET 9.0 features including nullable reference types and optimized performance.

## ✨ Features

- 🎯 **Two Buffer Types**: Fixed-capacity `RingBuffer<T>` and auto-expanding `GrowingRingBuffer<T>`
- 🔒 **Thread-Safe**: Safe for concurrent access scenarios
- 🚀 **High Performance**: Optimized for speed with minimal allocations
- 🧬 **Generic**: Works with any type `T`
- 🔄 **FIFO Operations**: First-in, first-out queue behavior
- 📚 **Standard Interfaces**: Implements `IEnumerable<T>`, `ICollection<T>`
- 🎛️ **Overflow Control**: Configurable overflow behavior (throw or overwrite)
- 🆕 **Modern .NET**: Built for .NET 9.0 with nullable reference types
- 🔧 **Zero Dependencies**: Lightweight with no external dependencies

## 📦 Installation

### Package Manager Console
```powershell
Install-Package RingBuffer
```

### .NET CLI
```bash
dotnet add package RingBuffer
```

### PackageReference
```xml
<PackageReference Include="RingBuffer" Version="0.2.0" />
```

## 🚀 Quick Start

### Basic Usage - Fixed Capacity

```csharp
using RingBuffer;

// Create a ring buffer with default capacity (4)
var buffer = new RingBuffer<int>();

// Or specify capacity
var customBuffer = new RingBuffer<string>(10);

// Add items
buffer.Put(1);
buffer.Put(2);
buffer.Put(3);

// Retrieve items (FIFO)
int first = buffer.Get(); // Returns 1
int second = buffer.Get(); // Returns 2

// Check buffer state
Console.WriteLine($"Size: {buffer.Size}");         // Current items
Console.WriteLine($"Capacity: {buffer.Capacity}"); // Maximum capacity
```

### Auto-Expanding Buffer

```csharp
using RingBuffer;

// Create a growing buffer
var growingBuffer = new GrowingRingBuffer<string>(4);

// Add more items than initial capacity - it will expand automatically
for (int i = 0; i < 10; i++)
{
    growingBuffer.Put($"Item {i}");
}

Console.WriteLine($"Capacity expanded to: {growingBuffer.Capacity}"); // Will be 8 or more
```

### Overflow Behavior Control

```csharp
// Allow overflow - overwrites oldest items when full
var overflowBuffer = new RingBuffer<int>(3, allowOverflow: true);

overflowBuffer.Put(1);
overflowBuffer.Put(2);
overflowBuffer.Put(3);
overflowBuffer.Put(4); // Overwrites the first item (1)

// Throw exception when full (default behavior)
var strictBuffer = new RingBuffer<int>(3, allowOverflow: false);
// strictBuffer.Put(4); // Would throw InvalidOperationException
```

### Enumeration Support

```csharp
var buffer = new RingBuffer<string>(5);
buffer.Put("First");
buffer.Put("Second"); 
buffer.Put("Third");

// Iterate through buffer contents
foreach (string item in buffer)
{
    Console.WriteLine(item); // Outputs: First, Second, Third
}

// Or use LINQ
var items = buffer.Where(x => x.Contains("i")).ToList();
```

## 📋 API Reference

### RingBuffer&lt;T&gt; Class

| Method/Property | Description |
|----------------|-------------|
| `Put(T item)` | Adds an item to the buffer |
| `Get()` | Retrieves and removes the oldest item |
| `Add(T item)` | Alias for `Put()` - implements `ICollection<T>` |
| `Size` | Current number of items in buffer |
| `Capacity` | Maximum number of items the buffer can hold |
| `AllowOverflow` | Whether the buffer overwrites oldest items when full |
| `Contains(T item)` | Checks if buffer contains specific item |
| `Clear()` | Removes all items from buffer |

### GrowingRingBuffer&lt;T&gt; Class

Inherits all `RingBuffer<T>` members plus:

| Method/Property | Description |
|----------------|-------------|
| `Put(T item)` | Adds item and expands capacity if needed |

### Constructors

```csharp
// RingBuffer<T>
new RingBuffer<T>()                          // Default capacity (4)
new RingBuffer<T>(int capacity)              // Custom capacity
new RingBuffer<T>(int capacity, bool overflow) // Custom capacity with overflow control

// GrowingRingBuffer<T>
new GrowingRingBuffer<T>()                   // Default initial capacity (4)
new GrowingRingBuffer<T>(int startCapacity)  // Custom initial capacity
```

## ⚡ Performance Characteristics

| Operation | Time Complexity | Space Complexity |
|-----------|----------------|------------------|
| `Put()` | O(1)* | O(1) |
| `Get()` | O(1) | O(1) |
| `Contains()` | O(n) | O(1) |
| Enumeration | O(n) | O(1) |

*O(n) for `GrowingRingBuffer<T>` when expansion occurs

## 🔧 Advanced Usage

### Custom Types

```csharp
public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public LogLevel Level { get; set; }
}

var logBuffer = new RingBuffer<LogEntry>(1000);
logBuffer.Put(new LogEntry 
{ 
    Timestamp = DateTime.Now, 
    Message = "Application started",
    Level = LogLevel.Info 
});
```

### Thread-Safe Operations

While individual operations are atomic, for complex operations you may need synchronization:

```csharp
private readonly object _lock = new object();
private readonly RingBuffer<int> _buffer = new RingBuffer<int>(100);

public void SafeAddRange(IEnumerable<int> items)
{
    lock (_lock)
    {
        foreach (var item in items)
        {
            _buffer.Put(item);
        }
    }
}
```

## 🛠️ Development

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- Any IDE with C# support (Visual Studio, VS Code, Rider)

### Building

```bash
# Clone the repository
git clone https://github.com/alexwiese/RingBuffer.NET.git
cd RingBuffer.NET

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack -c Release
```

### Running Tests

The project includes comprehensive unit tests:

```bash
dotnet test --verbosity normal
```

All tests are built using MSTest framework and cover:
- ✅ Basic operations (Put/Get)
- ✅ Capacity management
- ✅ Overflow behavior
- ✅ Growing buffer functionality
- ✅ Collection interface compliance
- ✅ Enumeration scenarios

## 🤝 Contributing

We welcome contributions! Here's how you can help:

1. **🍴 Fork** the repository
2. **🌿 Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **💾 Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **📤 Push** to the branch (`git push origin feature/amazing-feature`)
5. **🔄 Open** a Pull Request

### Development Guidelines

- Follow existing code style and formatting
- Add unit tests for new functionality
- Update documentation for public API changes
- Ensure all tests pass before submitting

## 📜 License

This project is licensed under the **GNU General Public License v3.0** - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Original implementation by [Joe Osborne](https://github.com/joeosborne)
- Continued development and modernization by [Alex Wiese](https://github.com/alexwiese)
- Built with ❤️ using .NET 9.0

---

<div align="center">

**[⭐ Star this repo](https://github.com/alexwiese/RingBuffer.NET/stargazers)** if you find it useful!

Made with 🔄 by the RingBuffer.NET community

</div>
