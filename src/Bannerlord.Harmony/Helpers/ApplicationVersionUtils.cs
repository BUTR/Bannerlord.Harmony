using TaleWorlds.Library;

namespace Bannerlord.BUTRLoader.Helpers
{
    internal static class ApplicationVersionUtils
    {
        public static bool TryParse(string? versionAsString, out ApplicationVersion version)
        {
            var major = 0;
            var minor = 0;
            var revision = 0;
            var changeSet = 0;
            bool skipCheck = false;
            version = default;
            if (versionAsString is null)
                return false;

            var array = versionAsString.Split('.');
            if (array.Length != 3 && array.Length != 4 && array[0].Length == 0)
                return false;

            var applicationVersionType = ApplicationVersion.ApplicationVersionTypeFromString(array[0][0].ToString());
            if (!skipCheck && !int.TryParse(array[0].Substring(1), out major))
            {
                if (array[0].Substring(1) != "*") return false;
                major = int.MinValue;
                minor = int.MinValue;
                revision = int.MinValue;
                changeSet = int.MinValue;
                skipCheck = true;
            }
            if (!skipCheck && !int.TryParse(array[1], out minor))
            {
                if (array[1] != "*") return false;
                minor = 0;
                revision = 0;
                changeSet = 0;
                skipCheck = true;
            }
            if (!skipCheck && !int.TryParse(array[2], out revision))
            {
                if (array[2] != "*") return false;
                revision = 0;
                changeSet = 0;
                skipCheck = true;
            }

            if (!skipCheck && array.Length == 4 && !int.TryParse(array[3], out changeSet))
            {
                if (array[3] != "*") return false;
                changeSet = 0;
                skipCheck = true;
            }

            version = new ApplicationVersion(applicationVersionType, major, minor, revision, changeSet, ApplicationVersionGameType.Singleplayer);
            return true;
        }

        public static string ToString(ApplicationVersion av)
        {
            string prefix = ApplicationVersion.GetPrefix(av.ApplicationVersionType);
            var def = ApplicationVersion.FromParametersFile(ApplicationVersionGameType.Singleplayer);
            return $"{prefix}{av.Major}.{av.Minor}.{av.Revision}{(av.ChangeSet == def.ChangeSet ? "" : $".{av.ChangeSet}")}";
        }
    }
}