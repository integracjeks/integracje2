namespace Integracje.UI.Model
{
    public enum SaveFormatName
    {
        XML,
        JSON,
        OGDL,
        YAML
    }

    public static class SaveFormat
    {
        #region Fields

        public const string JSON = "json";
        public const string OGDL = "ogdl";
        public const string XML = "xml";
        public const string YAML = "yaml";

        #endregion Fields
    }
}