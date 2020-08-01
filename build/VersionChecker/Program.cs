using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace VersionChecker
{
    public class NuGetVersions
    {
        public List<string> Versions { get; set; }
    }

    public class NexusModsLatestVersion
    {
        public string Message { get; set; }
    }

    public static class Program
    {
        private static readonly string NuGet = "https://api.nuget.org/v3-flatcontainer/lib.harmony/index.json";
        private static readonly string RunKit = "https://nexusmods-version-pzk4e0ejol6j.runkit.sh/?gameId=mountandblade2bannerlord&modId=2006";

        public static async Task Main(string[] args)
        {
            Version nuGetVersion;
            Version nexusModsLatestVersion;

            try
            {
                var client = new HttpClient();
                nuGetVersion = JsonConvert.DeserializeObject<NuGetVersions>(await client.GetStringAsync(NuGet)).Versions
                    .Select(Version.Parse)
                    .OrderBy(x => x)
                    .LastOrDefault();
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
        }
    }
}