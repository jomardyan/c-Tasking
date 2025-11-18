# NuGet Publishing Guide

## Prerequisites

- .NET 10.0 SDK or later
- NuGet account at https://www.nuget.org/
- NuGet API key from your account settings

## Publishing Steps

### 1. Local Testing

Build and test the package locally:

```bash
# Build the project
dotnet build -c Release

# Create the NuGet package
dotnet pack -c Release -o ./nupkg

# Test the package locally (optional)
# You can test by creating a temporary project and adding the package from the local path
```

### 2. Get Your API Key

1. Sign in to https://www.nuget.org/
2. Go to your profile settings
3. Click "Edit Profile"
4. Go to "API Keys"
5. Create a new key with "Push" permissions
6. Copy the key

### 3. Configure API Key

Store your NuGet API key (choose one method):

**Method 1: Global Configuration**
```bash
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
dotnet nuget update source nuget.org --username __USERNAME__ --password YOUR_API_KEY --store-password-in-clear-text
```

**Method 2: GitHub Secrets (Recommended for CI/CD)**
1. Go to your repository settings
2. Click "Secrets and variables" → "Actions"
3. Create a new secret named `NUGET_API_KEY`
4. Paste your API key value

### 4. Publish to NuGet

**Option A: Manual Publishing**

```bash
# Create package
dotnet pack -c Release

# Push to NuGet
dotnet nuget push "./bin/Release/c-Tasking.1.0.0.nupkg" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

**Option B: Using GitHub Actions (Recommended)**

1. The project includes GitHub Actions workflows in `.github/workflows/`
2. Create a release with tag format `v1.0.0`
3. The `publish.yml` workflow will automatically:
   - Build the project
   - Create the NuGet package
   - Push to NuGet
   - Create a GitHub release

```bash
# Create and push a tag
git tag v1.0.0
git push origin v1.0.0

# GitHub Actions will automatically publish to NuGet
```

### 5. Verify Publication

After publishing, verify the package:

```bash
# Wait a few minutes for indexing, then search
dotnet package search c-Tasking

# Or visit: https://www.nuget.org/packages/c-Tasking/
```

## Version Management

### Updating Version

1. Update the version in `c-Tasking.csproj`:
   ```xml
   <Version>1.1.0</Version>
   ```

2. Update `CHANGELOG.md` with changes

3. Commit and create a tag:
   ```bash
   git add c-Tasking.csproj CHANGELOG.md
   git commit -m "Version 1.1.0"
   git tag v1.1.0
   git push origin v1.1.0
   ```

## Semantic Versioning

Follow [Semantic Versioning](https://semver.org/):

- **MAJOR** (X.0.0): Breaking changes
- **MINOR** (1.X.0): New features, backward compatible
- **PATCH** (1.0.X): Bug fixes, backward compatible

## Pre-release Versions

For pre-release versions, use:

```xml
<Version>1.0.0-beta.1</Version>
<Version>1.0.0-rc.1</Version>
```

## Package Contents Verification

The NuGet package includes:

- All compiled assemblies
- XML documentation files
- README.md
- LICENSE
- Icon (icon.png)
- Source link support

Verify in `c-Tasking.csproj`:

```xml
<ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\"/>
    <None Include="LICENSE" Pack="true" PackagePath="\"/>
    <None Include="icon.png" Pack="true" PackagePath="\"/>
</ItemGroup>
```

## Troubleshooting

### Package Already Exists

If you get "Package with this version already exists":

- This is normal - NuGet doesn't allow republishing the same version
- Update the version number and try again

### API Key Invalid

```bash
# Verify your API key is correct
dotnet nuget list source

# Update the key
dotnet nuget update source nuget.org --username __USERNAME__ --password YOUR_NEW_KEY
```

### Symbol Packages

To publish symbols for debugging:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <DebugType>embedded</DebugType>
</PropertyGroup>
```

## Package Icons

Add an icon to your package:

1. Create a 128x128 PNG file named `icon.png`
2. Place it in the project root
3. It will be included in the NuGet package

## References

- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [NuGet CLI Reference](https://docs.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## Support

For issues with NuGet publishing, refer to:
- NuGet official documentation
- GitHub Actions logs in your repository
- NuGet support community
