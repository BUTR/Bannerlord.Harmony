# Contributing to Bannerlord.Harmony

Thank you for your interest in contributing to Bannerlord.Harmony! This document provides information on how to build, test, and contribute to this project.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Building the Module](#building-the-module)
  - [Local Development Build](#local-development-build)
  - [CI Build Process](#ci-build-process)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before building Bannerlord.Harmony, ensure you have the following installed:

- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/download)
- [Mount & Blade II: Bannerlord](https://store.steampowered.com/app/261550/Mount__Blade_II_Bannerlord/) (for testing)
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider recommended)

### Optional Environment Variables

You can set the following environment variables to customize the build:

- `BANNERLORD_GAME_DIR` - Path to your Bannerlord installation
- `BANNERLORD_STABLE_DIR` - Path to stable Bannerlord version
- `BANNERLORD_BETA_DIR` - Path to beta Bannerlord version

If these are not set, the build system will attempt to auto-detect the game folder from the default Steam installation path.

## Building the Module

### Local Development Build

#### Quick Start

1. Clone the repository:
   ```bash
   git clone https://github.com/BUTR/Bannerlord.Harmony.git
   cd Bannerlord.Harmony
   ```

2. Build the project:
   ```bash
   dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release
   ```

#### Building with Custom Game Folder

To build and deploy directly to your Bannerlord installation:

```bash
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release -p:GameFolder="C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

Replace the path with your actual Bannerlord installation directory.

#### Build Configurations

The project supports multiple build configurations:

- `Stable_Debug` - Debug build for stable Bannerlord version
- `Stable_Release` - Release build for stable Bannerlord version
- `Beta_Debug` - Debug build for beta Bannerlord version
- `Beta_Release` - Release build for beta Bannerlord version

Example:
```bash
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Debug
```

### CI Build Process

The GitHub Actions workflow automatically builds the module on every push to paths:
- `.github/workflows/publish.yml`
- `build/**`
- `src/Bannerlord.Harmony/**`

#### CI Build Steps

1. **Setup**: Uses `butr/actions-common-setup@v2` to configure the build environment
2. **Build**: Compiles the project using:
   ```powershell
   mkdir bannerlord
   dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj --configuration Release -p:GameFolder="$PWD/bannerlord"
   ```
3. **Version Extraction**: Retrieves the module version from the built DLL
4. **Artifact Upload**: Uploads the built module as a GitHub artifact
5. **Publishing**: (on master branch only) Publishes to GitHub releases

The build output is placed in:
```
bannerlord/Modules/Bannerlord.Harmony/
├── bin/
│   ├── Win64_Shipping_Client/     # Desktop version
│   └── Gaming.Desktop.x64_Shipping_Client/  # Xbox/Microsoft Store version
├── ModuleData/
└── SubModule.xml
```

## Project Structure

```
Bannerlord.Harmony/
├── .github/
│   └── workflows/
│       ├── publish.yml           # Main build and publish workflow
│       ├── check-version.yml     # Version checking workflow
│       └── dotnet-format-daily.yml
├── build/
│   └── common.props              # Shared build properties and versions
├── src/
│   └── Bannerlord.Harmony/
│       ├── Bannerlord.Harmony.csproj  # Main project file
│       └── [source files]
├── README.md                     # User-facing documentation
└── CONTRIBUTING.md               # This file
```

## Development Workflow

### Making Changes

1. Create a new branch for your changes:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes to the codebase

3. Build and test locally:
   ```bash
   dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Debug
   ```

4. Test the module in Bannerlord to ensure it works correctly

5. Commit your changes:
   ```bash
   git add .
   git commit -m "Description of your changes"
   ```

6. Push to your fork and create a pull request

### Testing Your Changes

1. Build the module with your local game folder:
   ```bash
   dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release -p:GameFolder="[YOUR_BANNERLORD_PATH]"
   ```

2. Launch Bannerlord and verify:
   - The module loads without errors
   - The module appears in the mod list
   - Dependent mods can find and use Harmony
   - The Debug UI (CTRL+ALT+H) functions correctly

## Configuration

### Version Configuration

Versions are managed in `build/common.props`:

- `HarmonyVersion` - Version of Lib.Harmony being used
- `GameVersion` - Target Bannerlord version
- `BuildResourcesVersion` - Version of Bannerlord.BuildResources
- Other dependency versions

The final module version is automatically calculated by combining the Harmony version with the GitHub Actions run number.

### Target Frameworks

The module targets two frameworks:
- `net472` - For standard Windows desktop version
- `net6` - For Xbox/Microsoft Store version

Both are built simultaneously by default.

## Troubleshooting

### Common Issues

#### Issue: "GameFolder not found" warnings

**Solution**: Set the `BANNERLORD_GAME_DIR` environment variable or use the `-p:GameFolder` parameter when building.

```bash
# Windows (PowerShell)
$env:BANNERLORD_GAME_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release

# Linux/macOS
export BANNERLORD_GAME_DIR="~/.steam/root/steamapps/common/Mount & Blade II Bannerlord"
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release
```

#### Issue: Build fails with "Package restore failed"

**Solution**: Clear the NuGet cache and restore packages:

```bash
dotnet nuget locals all --clear
dotnet restore src/Bannerlord.Harmony/Bannerlord.Harmony.csproj
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release
```

#### Issue: Module doesn't appear in Bannerlord

**Solution**:
1. Verify the module was copied to the correct location
2. Check the `SubModule.xml` has the correct game version
3. Ensure Bannerlord.Harmony is loaded first in the load order
4. Check the Bannerlord logs for any loading errors

### Getting Help

- Check existing [GitHub Issues](https://github.com/BUTR/Bannerlord.Harmony/issues)
- Create a new issue with detailed information about your problem
- Join the BUTR Discord community for real-time help

## Additional Resources

- [Bannerlord Modding Documentation](https://docs.bannerlordmodding.com/)
- [BUTR Organization](https://github.com/BUTR)
- [Lib.Harmony Documentation](https://harmony.pardeike.net/)
- [NexusMods Page](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006)

## License

This project is licensed under the same license as the repository. See the LICENSE file for details.
