# Building BUTR Bannerlord Modules

This document provides the standard build instructions for BUTR (Bannerlord Unified Testing and Reporting) organization modules, including Bannerlord.Harmony and other related projects.

## Standard BUTR Module Build Pattern

All BUTR Bannerlord modules follow a consistent build pattern that you can use across the organization.

### Prerequisites

- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/download)
- [Mount & Blade II: Bannerlord](https://store.steampowered.com/app/261550/Mount__Blade_II_Bannerlord/)
- A code editor (Visual Studio 2022, VS Code, or JetBrains Rider recommended)

### Standard Build Command

The standard pattern for building any BUTR module:

```bash
dotnet build src/[ModuleName]/[ModuleName].csproj -c Stable_Release -p:GameFolder="[PATH_TO_BANNERLORD]"
```

#### For Bannerlord.Harmony specifically:

```bash
dotnet build src/Bannerlord.Harmony/Bannerlord.Harmony.csproj -c Stable_Release -p:GameFolder="C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

#### For other BUTR modules (e.g., UIExtenderEx):

```bash
dotnet build src/Bannerlord.UIExtenderEx/Bannerlord.UIExtenderEx.csproj -c Stable_Release -p:GameFolder="C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

### Environment Variables

You can set these environment variables to avoid specifying the GameFolder parameter every time:

- `BANNERLORD_GAME_DIR` - Path to your Bannerlord installation (used when no specific version is set)
- `BANNERLORD_STABLE_DIR` - Path to stable Bannerlord version
- `BANNERLORD_BETA_DIR` - Path to beta Bannerlord version

**Windows (PowerShell):**
```powershell
$env:BANNERLORD_GAME_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

**Linux/macOS:**
```bash
export BANNERLORD_GAME_DIR="~/.steam/root/steamapps/common/Mount & Blade II Bannerlord"
```

### Build Configurations

BUTR modules typically support these configurations:

- `Stable_Debug` - Debug build for stable Bannerlord version
- `Stable_Release` - Release build for stable Bannerlord version
- `Beta_Debug` - Debug build for beta Bannerlord version
- `Beta_Release` - Release build for beta Bannerlord version

### Output Location

After building with the `GameFolder` parameter, the module will be placed in:

```
[GameFolder]/Modules/[ModuleName]/
├── bin/
│   ├── Win64_Shipping_Client/           # Desktop version (Steam/GOG/Epic)
│   └── Gaming.Desktop.x64_Shipping_Client/  # Xbox/Microsoft Store version
├── ModuleData/
└── SubModule.xml
```

## CI/CD Build Pattern

All BUTR repositories use standardized GitHub Actions workflows from the shared `BUTR/workflows` repository.

### Standard CI Build Steps

1. **Setup**: Uses `butr/actions-common-setup@v2`
2. **Build**:
   ```powershell
   mkdir bannerlord
   dotnet build src/[ModuleName]/[ModuleName].csproj --configuration Release -p:GameFolder="$PWD/bannerlord"
   ```
3. **Version Extraction**: From the built DLL
4. **Publishing**: Uses shared workflows for GitHub releases, NexusMods, and Steam Workshop

### Shared Workflows

BUTR uses reusable workflows from `BUTR/workflows`:
- `release-github.yml` - GitHub releases
- `release-nexusmods.yml` - NexusMods publishing
- `release-steam.yml` - Steam Workshop publishing

## Common Build Properties

BUTR modules typically share these properties in their `build/common.props`:

```xml
<GameFolder Condition="$(Configuration) == 'Stable_Debug' OR $(Configuration) == 'Stable_Release'">$(BANNERLORD_STABLE_DIR)</GameFolder>
<GameFolder Condition="$(Configuration) == 'Beta_Debug' OR $(Configuration) == 'Beta_Release'">$(BANNERLORD_BETA_DIR)</GameFolder>
<GameFolder Condition="$(BANNERLORD_STABLE_DIR) == '' AND $(BANNERLORD_BETA_DIR) == ''">$(BANNERLORD_GAME_DIR)</GameFolder>
```

## Target Frameworks

BUTR modules typically target:
- `net472` - For Windows desktop version (Steam/GOG/Epic)
- `net6` - For Xbox/Microsoft Store version

## Quick Reference

### Clone and Build Any BUTR Module

```bash
# Clone the repository
git clone https://github.com/BUTR/[RepositoryName].git
cd [RepositoryName]

# Build with auto-detected game folder
dotnet build src/[ModuleName]/[ModuleName].csproj -c Stable_Release

# Or build with explicit game folder
dotnet build src/[ModuleName]/[ModuleName].csproj -c Stable_Release -p:GameFolder="[PATH]"
```

### Test Your Build

1. Launch Bannerlord
2. Check that the module appears in the mod list
3. Enable the module and verify it loads without errors
4. Test dependent modules work correctly

## Troubleshooting

### GameFolder Not Found

If you see warnings about GameFolder not being found:
- Set the `BANNERLORD_GAME_DIR` environment variable
- Or use the `-p:GameFolder="[PATH]"` parameter explicitly

### Package Restore Failed

```bash
dotnet nuget locals all --clear
dotnet restore src/[ModuleName]/[ModuleName].csproj
dotnet build src/[ModuleName]/[ModuleName].csproj -c Stable_Release
```

### Module Doesn't Load

1. Check `SubModule.xml` has correct game version
2. Verify module dependencies are loaded first
3. Check Bannerlord logs for errors

## Related BUTR Repositories

- [Bannerlord.Harmony](https://github.com/BUTR/Bannerlord.Harmony) - Harmony library distribution
- [Bannerlord.UIExtenderEx](https://github.com/BUTR/Bannerlord.UIExtenderEx) - UI extension framework
- [Bannerlord.ButterLib](https://github.com/BUTR/Bannerlord.ButterLib) - Shared library
- [Bannerlord.MBOptionScreen](https://github.com/BUTR/Bannerlord.MBOptionScreen) - Mod options UI
- [BUTR/workflows](https://github.com/BUTR/workflows) - Shared CI/CD workflows

## Contributing

For repository-specific contribution guidelines, check the CONTRIBUTING.md file in each repository.

For organization-wide standards and practices, refer to the BUTR organization documentation.

## Support

- Report issues in the specific module's GitHub repository
- For general BUTR questions, visit the BUTR organization page
- Join the community Discord for real-time help

---

**Note**: This document describes the standard BUTR build pattern. Individual modules may have additional requirements documented in their repository-specific CONTRIBUTING.md file.
