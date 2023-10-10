<p align="center">
  <a href="https://github.com/BUTR/Bannerlord.Harmony/actions/workflows/publish.yml?query=branch%3Amaster+event%3Apush">
    <img alt="GitHub Workflow Status (event)" src="https://img.shields.io/github/actions/workflow/status/BUTR/Bannerlord.Harmony/publish.yml?branch=master&event=push&label=Latest%20Commit">
  </a>
  <a title="Crowdin" target="_blank" href="https://crowdin.com/project/harmony">
    <img src="https://badges.crowdin.net/harmony/localized.svg">
  </a>
  </br>
  <a href="https://www.nuget.org/packages/Lib.Harmony" alt="NuGet Harmony">
    <img src="https://img.shields.io/nuget/v/Lib.Harmony.svg?label=NuGet%20Lib.Harmony&colorB=blue" />
  </a>
  <a href="https://www.nuget.org/packages/Bannerlord.Lib.Harmony" alt="NuGet Harmony">
    <img src="https://img.shields.io/nuget/v/Bannerlord.Lib.Harmony.svg?label=NuGet%20Bannerlord.Lib.Harmony&colorB=blue" />
  </a>
  </br>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="NexusMods Harmony">
    <img src="https://img.shields.io/badge/NexusMods-Harmony-yellow.svg" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="NexusMods Harmony">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-version-pzk4e0ejol6j.runkit.sh%3FgameId%3Dmountandblade2bannerlord%26modId%3D2006" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="NexusMods Harmony">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dunique%26gameId%3D3174%26modId%3D2006" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="NexusMods Harmony">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dtotal%26gameId%3D3174%26modId%3D2006" />
  </a>
  <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="NexusMods Harmony">
    <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dviews%26gameId%3D3174%26modId%3D2006" />
  </a>
  </br>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859188632">
    <img alt="Steam Mod Configuration Menu" src="https://img.shields.io/badge/Steam-Harmony-blue.svg" />
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859188632">
    <img alt="Steam Downloads" src="https://img.shields.io/steam/downloads/2859188632?label=Downloads&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859188632">
    <img alt="Steam Views" src="https://img.shields.io/steam/views/2859188632?label=Views&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859188632">
    <img alt="Steam Subscriptions" src="https://img.shields.io/steam/subscriptions/2859188632?label=Subscriptions&color=blue">
  </a>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2859188632">
    <img alt="Steam Favorites" src="https://img.shields.io/steam/favorites/2859188632?label=Favorites&color=blue">
  </a>
  </br>
  <img src="https://staticdelivery.nexusmods.com/mods/3174/images/2006/2006-1615240039-1903390080.png" width="800">
</p>
 
This is an unofficial distribution of the [Lib.Harmony](https://github.com/pardeike/Harmony) library maintained by the community to have an easier way to manage external library dependencies.  
  
## Installation
This module should be the highest in loading order. Any other module that requires to be set at the top (Better Exception Window, DoubleDrawDistance, MCM, etc.) should be loaded after this mod.
  
## For Players
This mod is intended to ensure that all mods in a player's modlist are using the latest version of Harmony to minimize conflicts.  
  
## For Modders
**We added a Debug UI! Available via CTRL+ALT+H.**

You still need to reference Harmony as a NuGet package, it is not required to include ``0Harmony.dll`` in the final /bin output of your module.  
You do need to add this to your ``SubModule.xml``  
```xml
<DependedModules>
    <DependedModule Id="Bannerlord.Harmony" DependentVersion="v2.10.1" />
</DependedModules>
```
This way the game will ensure that the Harmony library is loaded before your mod.  
  
## Versioning  
The Module combines the Harmony version used and the Build Id that published the Module. The Build Id is simply added to the end.  
``2.0.2.22`` indicates that Harmony ``2.0.2.0`` is used and ``22`` is the Build Id.  
``2.0.0.1025`` indicates that Harmony ``2.0.0.10`` is used and ``25`` is the Build Id.  
  
We considered using the Rimworld approach with introducing our own version system, but it won't give a clear way to detect which Harmony version the Module contains. It's easier for the user to report the version of the Module used than to check the assembly version/send it. 
  
