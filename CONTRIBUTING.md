# Contributing to Smart Notification Manager

Thank you for your interest in contributing to Smart Notification Manager! This document provides guidelines and instructions for contributing to the project.

## Development Environment Setup

### Prerequisites

1. **Operating System**: Windows 10/11 (required for WinUI3 development)
2. **Visual Studio 2022** (recommended) with the following workloads:
   - .NET desktop development
   - Universal Windows Platform development
3. **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
4. **Git** for version control

### Getting Started

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/Smart_NotificationManager.git
   cd Smart_NotificationManager
   ```
3. Open the solution:
   ```bash
   cd SmartNotificationManager
   # Open SmartNotificationManager.sln in Visual Studio
   ```

## Building the Project

### Using Visual Studio

1. Open `SmartNotificationManager/SmartNotificationManager.sln`
2. Set `SmartNotificationManager` as the startup project (if not already set)
3. Select your target platform (x64 recommended)
4. Press F5 to build and run

### Using Command Line

```bash
cd SmartNotificationManager

# Restore dependencies
dotnet restore SmartNotificationManager.sln

# Build
dotnet build SmartNotificationManager.sln --configuration Debug /p:Platform=x64

# Or for Release
dotnet build SmartNotificationManager.sln --configuration Release /p:Platform=x64
```

## Making Changes

### Branch Strategy

- `main` or `master` - Production-ready code
- `develop` - Development branch (if used)
- Feature branches - `feature/your-feature-name`
- Bug fix branches - `fix/issue-description`

### Workflow

1. Create a new branch from `main` (or `develop` if used):
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes
   - Follow existing code style and conventions
   - Keep changes focused and atomic
   - Update documentation if needed

3. Build and test your changes:
   ```bash
   dotnet build SmartNotificationManager/SmartNotificationManager.sln --configuration Debug /p:Platform=x64
   ```

4. Commit your changes:
   ```bash
   git add .
   git commit -m "Brief description of your changes"
   ```

5. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

6. Create a Pull Request
   - Go to the original repository on GitHub
   - Click "New Pull Request"
   - Select your branch
   - Fill in the PR template with details about your changes
   - Wait for CI validation to complete

## Pull Request Guidelines

### Before Submitting

- [ ] Code builds successfully on x64 platform
- [ ] No compiler warnings introduced
- [ ] Documentation updated (if applicable)
- [ ] PR description clearly explains the changes

### PR Description Template

```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## Related Issues
Fixes #(issue number)

## Testing
Describe how you tested your changes

## Screenshots (if applicable)
Add screenshots to demonstrate UI changes
```

## Code Style Guidelines

### C# Code Style

- Follow standard C# naming conventions
- Use meaningful variable and method names
- Keep methods focused and concise
- Add XML documentation comments for public APIs
- Use `nullable` reference types appropriately

### XAML Style

- Use consistent indentation (4 spaces or 1 tab)
- Group related properties together
- Use resource dictionaries for reusable styles
- Follow WinUI3 best practices

## Project Structure

```
SmartNotificationManager/
├── SmartNotificationManager/           # Main WinUI3 startup project
│   └── SmartNotificationManager/       # Actual project files
├── SmartNotificationManager.View/      # WinUI3 view components
├── WinUI3Component/                    # Reusable WinUI3 components
├── SmartNotificationLibrary/           # Core library (.NET 9 + WinUI3)
├── SmartNotificationManger.Entities/   # Data entities (.NET 9)
└── .github/
    └── workflows/                      # CI/CD workflows
```

## CI/CD Pipeline

All pull requests automatically trigger:

1. **PR Validation** - Quick build validation
   - Builds both Debug and Release configurations
   - Posts status comment on PR

2. **CI Workflow** - Full continuous integration
   - Runs on all pushes and PRs to main branches
   - Builds and tests the solution
   - Uploads build artifacts

The CI must pass before a PR can be merged.

## Known Issues

### Missing Dependencies

The project currently references the following projects that are not in the repository:
- `WinCommon`
- `WinLogger`
- `WinSQLiteDBAdapter`

These will cause build warnings but should not prevent compilation of the existing code.

## Getting Help

- **Issues**: Check existing [GitHub Issues](https://github.com/kathizeal/Smart_NotificationManager/issues)
- **Discussions**: Start a [GitHub Discussion](https://github.com/kathizeal/Smart_NotificationManager/discussions)
- **Documentation**: See [README.md](README.md)

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person
- Help create a welcoming environment for all contributors

## License

By contributing to this project, you agree that your contributions will be licensed under the same license as the project.
