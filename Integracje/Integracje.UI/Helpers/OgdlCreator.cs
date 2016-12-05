using System;

namespace Integracje.UI.Helpers
{
    public static class OgdlCreator
    {
        #region Methods

        internal static string GetOgdl(string yaml)
        {
            var ogdl = yaml;

            ogdl = ogdl.Replace("- ", $"Book{Environment.NewLine}  ");
            ogdl = ogdl.Replace(": ", " ");
            ogdl = ogdl.Replace("  ", "\t");

            return ogdl;
        }

        #endregion Methods
    }
}