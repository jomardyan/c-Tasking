# Documentation Index

Welcome to c-Tasking documentation! Here you'll find everything you need to use and contribute to the library.

## 📚 Quick Navigation

### Getting Started
- **[README.md](../README.md)** - Main introduction, features, and quick start guide
- **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** - Quick API lookup and common patterns

### Detailed Guides
- **[LIBRARY_GUIDE.md](LIBRARY_GUIDE.md)** - Comprehensive library guide with all components
- **[RELEASE_NOTES.md](RELEASE_NOTES.md)** - What's included in v1.0.0
- **[API_REFERENCE.md](API_REFERENCE.md)** - Consolidated Application API reference (automatically generated)
- **[USAGE_AND_COMPARISON.md](USAGE_AND_COMPARISON.md)** - Usage cases and comparison with native .NET APIs

### Development
- **[PROJECT_SETUP.md](PROJECT_SETUP.md)** - Development setup and project structure
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - How to contribute to the project
- **[CHANGELOG.md](CHANGELOG.md)** - Version history and changes

### Publishing
- **[NUGET_PUBLISHING.md](NUGET_PUBLISHING.md)** - How to publish packages to NuGet
- **[DELIVERY_SUMMARY.md](DELIVERY_SUMMARY.md)** - Project completion and release status

## 🎯 Where to Start?

### I want to...

**Use the library:**
1. Start with [README.md](../README.md) for quick start
2. Check [QUICK_REFERENCE.md](QUICK_REFERENCE.md) for API lookup
3. Read [LIBRARY_GUIDE.md](LIBRARY_GUIDE.md) for detailed examples

**Contribute:**
1. Read [CONTRIBUTING.md](CONTRIBUTING.md)
2. Follow [PROJECT_SETUP.md](PROJECT_SETUP.md)
3. Check [CHANGELOG.md](CHANGELOG.md) for recent changes

**Publish a release:**
1. Follow [NUGET_PUBLISHING.md](NUGET_PUBLISHING.md)
2. Reference [DELIVERY_SUMMARY.md](DELIVERY_SUMMARY.md) for checklist

**Understand the project:**
1. Read [DELIVERY_SUMMARY.md](DELIVERY_SUMMARY.md)
2. Check [PROJECT_SETUP.md](PROJECT_SETUP.md)
3. Review [CHANGELOG.md](CHANGELOG.md)

## 📂 Document Structure

```
docs/
├── INDEX.md                   ← You are here
├── QUICK_REFERENCE.md         ← Quick API lookup
├── LIBRARY_GUIDE.md           ← Comprehensive guide
├── RELEASE_NOTES.md           ← What's new
├── PROJECT_SETUP.md           ← Development setup
├── CONTRIBUTING.md            ← Contribution guide
├── NUGET_PUBLISHING.md        ← Publishing guide
├── DELIVERY_SUMMARY.md        ← Project status
└── CHANGELOG.md               ← Version history

../
├── README.md                  ← Main documentation
├── LICENSE                    ← MIT License
├── c-Tasking.csproj          ← Project file
├── c-Tasking.sln             ← Solution file
└── icon.png                   ← Package icon
```

## 🔍 Key Components

### Core Libraries
- **TaskWrapper** - Simple task execution
- **SimpleThread** - Thread management
- **AsyncOperation** - Async tracking
- **ManagedThreadPool** - Thread pool
- **TaskScheduler** - Task scheduling
- **TaskRetry** - Retry logic
- **ConcurrentBatcher** - Batch processing
- **TaskExtensions** - LINQ extensions

See [LIBRARY_GUIDE.md](LIBRARY_GUIDE.md) for detailed documentation of each.

## 🚀 Quick Commands

```bash
# Build
dotnet build -c Release

# Pack
dotnet pack -c Release

# Publish to NuGet
dotnet nuget push bin/Release/c-Tasking.*.nupkg --api-key YOUR_KEY

# Create tag for release
git tag v1.0.0
git push origin v1.0.0
```

See [PROJECT_SETUP.md](PROJECT_SETUP.md) for more commands.

## 📞 Need Help?

- **Questions:** Check the relevant guide above
- **Issues:** Visit GitHub Issues
- **Discussions:** Start a discussion on GitHub
- **Contributions:** See [CONTRIBUTING.md](CONTRIBUTING.md)

## 📋 API Quick Links

### Core (`c_Tasking.Core`)
| Class | Purpose |
|-------|---------|
| `TaskWrapper` | Simple task execution |
| `SimpleThread` | Thread management |
| `AsyncOperation` | Async operation tracking |
| `ManagedThreadPool` | Thread pool management |

### Utilities (`c_Tasking.Utilities`)
| Class | Purpose |
|-------|---------|
| `TaskScheduler` | Task scheduling |
| `TaskRetry` | Retry with backoff |
| `ConcurrentBatcher<T>` | Batch processing |
| `ParallelTaskExecutor` | Parallel execution |

### Extensions (`c_Tasking.Extensions`)
| Class | Purpose |
|-------|---------|
| `TaskExtensions` | LINQ-style task operations |

See [QUICK_REFERENCE.md](QUICK_REFERENCE.md) for detailed API reference.
For a consolidated master API reference, see [API_REFERENCE.md](API_REFERENCE.md).

---

**Last Updated:** November 17, 2025  
**Version:** 1.0.0  
**Status:** Release Ready
