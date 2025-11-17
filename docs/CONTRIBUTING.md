# Contributing to c-Tasking

Thank you for your interest in contributing to c-Tasking! This document provides guidelines and instructions for contributing.

## Code of Conduct

Please be respectful and professional in all interactions with other contributors and maintainers.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/your-username/c-Tasking.git`
3. Create a branch: `git checkout -b feature/your-feature`
4. Make your changes
5. Test your changes: `dotnet build && dotnet test`
6. Commit: `git commit -am 'Add your feature'`
7. Push: `git push origin feature/your-feature`
8. Open a Pull Request

## Development Setup

### Requirements
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code with C# extension

### Building
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Creating a Package
```bash
dotnet pack -c Release
```

## Code Style Guidelines

- Follow C# naming conventions (PascalCase for public members, camelCase for local variables)
- Use meaningful variable and method names
- Add XML documentation comments to all public members
- Keep methods focused and reasonably sized
- Add unit tests for new features

### Example Code Style

```csharp
/// <summary>
/// Performs an important operation.
/// </summary>
public async Task<int> PerformOperationAsync(string input)
{
    ValidateInput(input);
    
    var result = await ProcessAsync(input);
    return result;
}

private static void ValidateInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("Input cannot be empty", nameof(input));
}
```

## Pull Request Process

1. Update documentation and examples if needed
2. Add unit tests for new functionality
3. Ensure all tests pass: `dotnet test`
4. Build the package: `dotnet pack`
5. Update CHANGELOG.md
6. Reference any related issues in the PR description

## Reporting Issues

When reporting issues, please include:
- A clear, descriptive title
- Description of the issue
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment (OS, .NET version)
- Code sample if applicable

## Feature Requests

Feature requests are welcome! Please provide:
- Clear description of the feature
- Use case and motivation
- Potential implementation approach
- Any related issues or discussions

## Documentation

Help with documentation is always appreciated:
- Improving README clarity
- Adding more examples
- Clarifying API documentation
- Fixing typos

## Project Structure

```
c-Tasking/
├── Core/                    # Core threading components
│   ├── TaskWrapper.cs
│   ├── SimpleThread.cs
│   ├── AsyncOperation.cs
│   └── ManagedThreadPool.cs
├── Utilities/               # Utility classes
│   ├── TaskScheduler.cs
│   ├── TaskRetry.cs
│   └── ConcurrentBatcher.cs
├── Extensions/              # Extension methods
│   └── TaskExtensions.cs
├── Examples/                # Usage examples
│   └── UsageExamples.cs
├── README.md
├── CHANGELOG.md
├── LICENSE
└── c-Tasking.csproj
```

## Release Process

Releases are managed by maintainers. To request a release:
1. Ensure all issues are resolved
2. Update version in `c-Tasking.csproj`
3. Update CHANGELOG.md
4. Create a release PR
5. After merge, create a GitHub release with tag
6. Package and publish to NuGet

## Questions or Need Help?

- Open an issue for questions
- Check existing documentation
- Review example code
- Ask in discussions

Thank you for contributing to c-Tasking!
