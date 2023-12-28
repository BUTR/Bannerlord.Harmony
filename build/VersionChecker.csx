#r "nuget: Newtonsoft.Json, 13.0.3"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;

using Newtonsoft.Json;

public class NuGetVersions
{
    public required List<string> Versions { get; init; }
}

public class NexusModsLatestVersion
{
    public required string Message { get; init; }
}

private static readonly string NuGet = "https://api.nuget.org/v3-flatcontainer/lib.harmony/index.json";
private static readonly string RunKit = "https://nexusmods-version-pzk4e0ejol6j.runkit.sh/?gameId=mountandblade2bannerlord&modId=2006";

Version nuGetVersion;
Version nexusModsLatestVersion;

try
{
    var client = new HttpClient();
    nuGetVersion = JsonConvert.DeserializeObject<NuGetVersions>(await client.GetStringAsync(NuGet)).Versions.Select(Version.Parse).MaxBy(x => x);
    nexusModsLatestVersion = Version.Parse(JsonConvert.DeserializeObject<NexusModsLatestVersion>(await client.GetStringAsync(RunKit)).Message);
}
catch (Exception e)
{
    Console.WriteLine(1);
    //Console.WriteLine(e);
    return;
}

if (nexusModsLatestVersion == nuGetVersion)
    Console.WriteLine(2);

if (nexusModsLatestVersion > nuGetVersion)
    Console.WriteLine(3);

if (nexusModsLatestVersion < nuGetVersion)
    Console.WriteLine(0);