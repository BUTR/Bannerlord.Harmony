namespace Bannerlord.Harmony.Utils
{
    internal record InformationMessageWrapper(object Object)
    {
        public static InformationMessageWrapper Create(object @object) => new(@object);
    }
}