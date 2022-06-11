namespace Bannerlord.Harmony.Helpers
{
    internal sealed class InformationMessageWrapper
    {
        public static InformationMessageWrapper Create(object @object) => new(@object);

        public object Object { get; }

        private InformationMessageWrapper(object @object)
        {
            Object = @object;
        }
    }
}