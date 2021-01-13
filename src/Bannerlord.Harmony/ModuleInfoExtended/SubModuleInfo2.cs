using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bannerlord.BUTRLoader.ModuleInfoExtended
{
    /// <summary>
    /// https://github.com/BUTR/Bannerlord.ButterLib/blob/dev/src/Bannerlord.ButterLib/Helpers/ModuleInfo/ExtendedSubModuleInfo.cs
    /// </summary>
    internal sealed class SubModuleInfo2 : IEquatable<SubModuleInfo2>
    {
        public enum SubModuleTags
        {
            RejectedPlatform,
            ExclusivePlatform,
            DedicatedServerType,
            IsNoRenderModeElement,
            DependantRuntimeLibrary
        }

        public string Name { get; internal set; } = string.Empty;
		public string DLLName { get; internal set; } = string.Empty;
		public bool DLLExists { get; internal set; }
		public List<string> Assemblies { get; internal set; } = new();
		public string SubModuleClassType { get; internal set; } = string.Empty;
        public List<Tuple<SubModuleTags, string>> Tags { get; internal set; } = new();

		public void LoadFrom(XmlNode subModuleNode, string path)
		{
            Assemblies.Clear();
			Tags.Clear();
			Name = subModuleNode?.SelectSingleNode("Name")?.Attributes["value"]?.InnerText ?? string.Empty;
			DLLName = subModuleNode?.SelectSingleNode("DLLName")?.Attributes["value"]?.InnerText ?? string.Empty;
			string text = Path.Combine(path, "bin\\Win64_Shipping_Client", DLLName);
			if (!string.IsNullOrEmpty(DLLName))
			{
				DLLExists = File.Exists(text);
            }
			SubModuleClassType = subModuleNode?.SelectSingleNode("SubModuleClassType")?.Attributes["value"]?.InnerText ?? string.Empty;
			if (subModuleNode?.SelectSingleNode("Assemblies") != null)
			{
				var assembliesList = subModuleNode?.SelectSingleNode("Assemblies")?.SelectNodes("Assembly");
				for (var i = 0; i < assembliesList?.Count; i++)
				{
                    if (assembliesList[i]?.Attributes["value"]?.InnerText is { } value)
					    Assemblies.Add(value);
				}
			}

            var tagsList = subModuleNode?.SelectSingleNode("Tags")?.SelectNodes("Tag");
            for (var i = 0; i < tagsList?.Count; i++)
			{
                if (tagsList[i]?.Attributes["key"]?.InnerText is { } key && tagsList[i]?.Attributes["value"]?.InnerText is { } value && Enum.TryParse<SubModuleTags>(key, out var subModuleTags))
				{
                    Tags.Add(new Tuple<SubModuleTags, string>(subModuleTags, value));
                }
			}
		}

        public override string ToString() => Name;

        public bool Equals(SubModuleInfo2? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }
        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || (obj is SubModuleInfo2 other && Equals(other));

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(SubModuleInfo2? left, SubModuleInfo2? right) => Equals(left, right);
        public static bool operator !=(SubModuleInfo2? left, SubModuleInfo2? right) => !Equals(left, right);
    }
}