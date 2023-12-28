namespace Bannerlord.Harmony;

/// <summary>
/// Class to make the game go find 0Harmony.
/// </summary>
internal class HarmonyRef : HarmonyLib.Harmony
{
    public HarmonyRef(string id) : base(id) { }
}