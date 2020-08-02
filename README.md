<p align="center">
   <a href="https://www.nuget.org/packages/Lib.Harmony" alt="NuGet Harmony">
   <img src="https://img.shields.io/nuget/v/Lib.Harmony.svg?label=NuGet%20Lib.Harmony&colorB=blue" /></a>
   </br>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="Nexus Harmony">
   <img src="https://img.shields.io/badge/Nexus-Harmony-yellow.svg" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="Nexus Harmony">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-version-pzk4e0ejol6j.runkit.sh%3FgameId%3Dmountandblade2bannerlord%26modId%3D2006" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="Nexus Harmony">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dunique%26gameId%3D3174%26modId%3D2006" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" alt="Nexus Harmony">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dtotal%26gameId%3D3174%26modId%3D2006" /></a>
</p>

This is an unofficial distribution of the [Harmony](https://github.com/pardeike/Harmony) library maintained by the community to have an easier way to manage external library dependencies.  
Fully automated, it checks every 1 hour for a new release of Harmony.  

## Installation
This module should be the highest in loading order. Any other module that requires to be set at the top (MCM, etc.) should be loaded after this mod.  

## For Modders
The Module Id is ``Bannerlord.Harmony``.  
You still need to reference Harmony as a NuGet package, it is not required to include 0Harmony.dll in the final /bin output of your module.  
You do need to add this to your ``SubModule.xml``
```xml
<DependedModules>
    <DependedModule Id="Bannerlord.Harmony" />
</DependedModules>
```
This way the game will ensure that the Harmony library is loaded before your mod.  
