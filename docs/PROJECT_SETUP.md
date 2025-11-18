# Project Setup and Distribution Guide

## 📁 Project Structure

```
c-Tasking/
├── Core/                        # Core threading and task management
│   ├── TaskWrapper.cs           # Simple task execution wrapper
│   ├── SimpleThread.cs          # Thread management with cancellation
│   ├── AsyncOperation.cs        # Async operation tracking
│   └── ManagedThreadPool.cs     # Thread pool with auto-queuing
│
├── Utilities/                   # Utility and helper classes
│   ├── TaskScheduler.cs         # Task scheduling utilities
│   ├── TaskRetry.cs             # Retry logic with backoff
│   ├── ConcurrentBatcher.cs     # Batch processing utilities
│   └── AdvancedUtilities.cs     # Advanced parallel/throttling utilities
│
├── Extensions/                  # LINQ-style extension methods
│   └── TaskExtensions.cs        # Task extension methods
│
├── Examples/                    # Usage examples
│   └── UsageExamples.cs         # Comprehensive examples
│
├── .github/workflows/           # CI/CD workflows
│   ├── build.yml                # Build and test workflow
│   └── publish.yml              # NuGet publishing workflow
│
├── README.md                    # Main documentation
├── QUICK_REFERENCE.md           # Quick API reference
├── LIBRARY_GUIDE.md             # Comprehensive library guide
├── CONTRIBUTING.md              # Contributing guidelines
├── CHANGELOG.md                 # Version history
├── NUGET_PUBLISHING.md          # NuGet publishing guide
├── LICENSE                      # MIT License
├── icon.png                     # Package icon
├── c-Tasking.csproj             # Project file with NuGet metadata
├── c-Tasking.sln                # Visual Studio solution
└── .gitignore                   # Git ignore rules
```

## 🛠️ Development Setup

### Requirements
- .NET 10.0 SDK or later
- Visual Studio 2022 / VS Code with C# extension
- Git

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/jomardyan/c-Tasking.git
cd c-Tasking

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run any tests
dotnet test

# Create local NuGet package
dotnet pack -c Release -o ./nupkg
```

## 📦 Package Information

### NuGet Package Details

| Property | Value |
|----------|-------|
| Package ID | c-Tasking |
| Current Version | 1.0.0 |
| Framework | .NET 8.0+ |
| License | MIT |
| Author | Jomar Dyan |
| Repository | github.com/jomardyan/c-Tasking |
| Tags | threading, async, tasks, multitasking, concurrency |

### Package Contents

The NuGet package includes:

```
c-Tasking.1.0.0.nupkg
├── lib/
│   └── net10.0/
│       ├── c-Tasking.dll
│       └── c-Tasking.xml (documentation)
├── README.md
├── LICENSE
├── icon.png
└── package metadata (nuspec)
```

## 🚀 Distribution Channels

### 1. NuGet.org (Primary)

**Official Package:** https://www.nuget.org/packages/c-Tasking/

**Installation:**
```bash
dotnet add package c-Tasking
```

**Publishing Process:**
1. Create a GitHub release with tag (e.g., `v1.0.0`)
2. GitHub Actions automatically publishes to NuGet
3. Verify at https://www.nuget.org/packages/c-Tasking/

### 2. GitHub Releases

Releases are available at: https://github.com/jomardyan/c-Tasking/releases

**Features:**
- Source code archives (zip/tar.gz)
- Change logs
- Release notes
- Binary downloads

### 3. Source Code

**Repository:** https://github.com/jomardyan/c-Tasking

Clone directly:
```bash
git clone https://github.com/jomardyan/c-Tasking.git
```

## 📝 Documentation Files

### Core Documentation

| File | Purpose |
|------|---------|
| `README.md` | Main introduction, features, quick start, API reference |
| `QUICK_REFERENCE.md` | Quick API lookup guide |
| `LIBRARY_GUIDE.md` | Comprehensive usage guide with examples |
| `CHANGELOG.md` | Version history and changes |
| `CONTRIBUTING.md` | Contribution guidelines |
| `LICENSE` | MIT License terms |

### Development Documentation

| File | Purpose |
|------|---------|
| `NUGET_PUBLISHING.md` | NuGet publishing guide |
| `.github/workflows/` | CI/CD workflows |

## 🔄 CI/CD Pipeline

### Automated Workflows

**1. Build Workflow** (`.github/workflows/build.yml`)
- Triggers on: push to main/develop, pull requests
- Steps:
  - Setup .NET 8.0
  - Restore dependencies
  - Build in Release mode
  - Run tests
  - Create NuGet package
  - Upload artifacts

**2. Publishing Workflow** (`.github/workflows/publish.yml`)
- Triggers on: version tag push (e.g., `v1.0.0`)
- Steps:
  - Build release version
  - Create NuGet package
  - Push to NuGet.org
  - Create GitHub release

### Manual Publishing

```bash
# Locally
dotnet pack -c Release
dotnet nuget push bin/Release/c-Tasking.1.0.0.nupkg --api-key YOUR_API_KEY

# Using GitHub CLI
gh release create v1.0.0 --generate-notes
```

## 🔐 Security & Best Practices

### API Key Management
- Never commit API keys to repository
- Use GitHub Secrets for CI/CD
- Rotate keys periodically
- Use read-only keys when possible

### Code Quality
- XML documentation on all public APIs
- Zero compiler warnings in Release mode
- Nullable reference types enabled
- Consistent code style

### Testing
- Comprehensive examples in `UsageExamples.cs`
- All public APIs documented
- Example patterns for common scenarios

## 📊 Versioning Strategy

Follow Semantic Versioning (MAJOR.MINOR.PATCH):

- **1.0.0** → Initial release
- **1.1.0** → New features, backward compatible
- **1.0.1** → Bug fixes, backward compatible
- **2.0.0** → Breaking changes

Update version in:
1. `c-Tasking.csproj` - `<Version>` tag
2. `CHANGELOG.md` - Add release section
3. `CONTRIBUTING.md` - Update if needed

## 📋 Release Checklist

Before releasing a new version:

- [ ] Update version in `.csproj`
- [ ] Update `CHANGELOG.md`
- [ ] Verify all tests pass
- [ ] Test NuGet package locally
- [ ] Verify documentation is current
- [ ] Commit changes: `git commit -am "Release v1.x.x"`
- [ ] Create tag: `git tag v1.x.x`
- [ ] Push: `git push origin v1.x.x`
- [ ] GitHub Actions publishes to NuGet
- [ ] Create GitHub release with notes

## 🎯 Quick Command Reference

### Building & Testing
```bash
dotnet build                          # Debug build
dotnet build -c Release               # Release build
dotnet test                           # Run tests
dotnet clean                          # Clean build artifacts
```

### Packaging
```bash
dotnet pack -c Release                # Create NuGet package
dotnet pack -c Release -o ./nupkg     # Create and output to folder
```

### Publishing
```bash
# Manual NuGet push
dotnet nuget push ./nupkg/c-Tasking.1.0.0.nupkg \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json

# GitHub release
gh release create v1.0.0 --generate-notes
```

### Documentation Generation
```bash
# The project generates XML documentation automatically
# Located at: bin/Release/net10.0/c-Tasking.xml
```

## 🔗 Important Links

- **NuGet Package:** https://www.nuget.org/packages/c-Tasking/
- **GitHub Repository:** https://github.com/jomardyan/c-Tasking
- **GitHub Issues:** https://github.com/jomardyan/c-Tasking/issues
- **Discussions:** https://github.com/jomardyan/c-Tasking/discussions

## 📞 Support

For questions or issues:
1. Check documentation files
2. Review `UsageExamples.cs`
3. Open GitHub issue
4. Visit GitHub discussions

## 🎓 Additional Resources

- [Microsoft .NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Last Updated:** November 17, 2025
**Version:** 1.0.0
