# Smart Notification Manager

[![CI](https://github.com/kathizeal/Smart_NotificationManager/actions/workflows/ci.yml/badge.svg)](https://github.com/kathizeal/Smart_NotificationManager/actions/workflows/ci.yml)
[![CD - Release](https://github.com/kathizeal/Smart_NotificationManager/actions/workflows/cd.yml/badge.svg)](https://github.com/kathizeal/Smart_NotificationManager/actions/workflows/cd.yml)

A WinUI3 notification management application built with .NET 9.

## Project Structure

- **SmartNotificationManager** - Main WinUI3 startup project (executable)
- **SmartNotificationManager.View** - WinUI3 view components
- **WinUI3Component** - Reusable WinUI3 components
- **SmartNotificationLibrary** - Core notification library (.NET 9)
- **SmartNotificationManger.Entities** - Data entities (.NET 9)

## Prerequisites

- .NET 9.0 SDK
- Windows 10 SDK (version 10.0.19041.0 or later)
- Visual Studio 2022 (recommended) with:
  - .NET desktop development workload
  - Universal Windows Platform development workload

## Building the Project

### Command Line

```bash
# Navigate to the solution directory
cd SmartNotificationManager

# Restore dependencies
dotnet restore SmartNotificationManager.sln

# Build the solution
dotnet build SmartNotificationManager.sln --configuration Release /p:Platform=x64

# Publish the application
dotnet publish SmartNotificationManager/SmartNotificationManager.csproj --configuration Release --runtime win-x64 --self-contained true
```

### Visual Studio

1. Open `SmartNotificationManager/SmartNotificationManager.sln`
2. Set `SmartNotificationManager` as the startup project
3. Select your desired platform (x64, x86, or ARM64)
4. Build and run (F5)

## CI/CD

The project includes automated CI/CD workflows using GitHub Actions:

### Continuous Integration (CI)

**Workflow**: `.github/workflows/ci.yml`

- **Trigger**: Push or pull request to main/master/develop branches
- **Platforms**: x64
- **Configurations**: Debug and Release
- **Actions**:
  - Restore dependencies
  - Build solution
  - Run tests (if available)
  - Upload build artifacts (Release only)

### Pull Request Validation

**Workflow**: `.github/workflows/pr-validation.yml`

- **Trigger**: Pull request opened, synchronized, or reopened
- **Platform**: x64
- **Configurations**: Debug and Release
- **Actions**:
  - Validate both Debug and Release builds
  - Post build status comment on PR

### Continuous Deployment (CD)

**Workflow**: `.github/workflows/cd.yml`

- **Trigger**: Push tags matching `v*.*.*` pattern (e.g., v1.0.0) or manual workflow dispatch
- **Platforms**: x64, x86, ARM64
- **Actions**:
  - Build and publish for all platforms
  - Create release packages
  - Automatically create GitHub release with artifacts

#### Creating a Release

To trigger a release:

```bash
# Tag your commit
git tag v1.0.0
git push origin v1.0.0
```

Or use the manual workflow dispatch in the GitHub Actions tab.

## Known Limitations

The project references the following dependencies that are not included in this repository:
- WinCommon
- WinLogger
- WinSQLiteDBAdapter

These projects need to be added or the references removed for a complete build.

## Target Frameworks

- **WinUI3 Projects**: net9.0-windows10.0.19041.0
- **Library Projects**: net9.0

## License

[Add your license information here]
