using System.Collections.Generic;
using System.Drawing;

namespace Integracje.UI.Style
{
    public enum TextAlign
    {
        left, center, right
    }

    public enum ItalicTable
    {
        none, id, title, pages, year
    }

    public class StyleTemplate
    {
        #region Properties

        public string TemplateName { get; set; }

        public IEnumerable<Color> ColorsList { get; set; }

        public Color SelectedRowColor { get; set; }

        public Color SelectedCellColor { get; set; }

        public IEnumerable<int> FontSizes { get; set; }

        public int SelectedFontSize { get; set; }

        public IEnumerable<TextAlign> TextAligns { get; set; }

        public TextAlign SelectedTextAlign { get; set; }

        public IEnumerable<ItalicTable> ItalicTables { get; set; }

        public ItalicTable SelectedItalicTable { get; set; }

        public Color SelectedDocumentColor { get; set; }

        public Color SelectedTableColor { get; set; }

        public bool BoldHeader { get; set; }

        public IEnumerable<int> BorderSizes { get; set; }

        public int SelectedBorderSize { get; set; }

        public Color SelectedBorderColor { get; set; }

        #endregion Properties

        #region Methods

        public string GenerateCssString()
        {
            string css = $"Books{{background:{Prep(SelectedDocumentColor)};display:table;width:90%;margin:auto;border-collapse:separate}}" +
                $"Book{{border-bottom:{SelectedBorderSize}px solid {Prep(SelectedBorderColor)};display:table-row;" +
                $"background:{Prep(SelectedTableColor)};border-bottom:{SelectedBorderSize}px solid {Prep(SelectedBorderColor)}}}Books " +
                $"> Book:last-child > *{{border-bottom:{SelectedBorderSize}px solid {Prep(SelectedBorderColor)}}}Book:hover{{background:{Prep(SelectedRowColor)}}}Book > *" +
                $"{{display:table-cell;text-align:{SelectedTextAlign};padding:5px;border:{SelectedBorderSize}px solid {Prep(SelectedBorderColor)};" +
                $"border-right:none;border-bottom:none;vertical-align:middle" +
                $";font-size:{SelectedFontSize}px}}Book > :last-child{{border-right:{SelectedBorderSize}px solid {Prep(SelectedBorderColor)}}}Book > :hover{{background:{Prep(SelectedCellColor)}}}";

            if (BoldHeader)
            {
                css += $".nagl{{font-weight:700}}[class=nagl]{{font-weight:700}}";
            }

            if (SelectedItalicTable != 0)
            {
                css += $"{SelectedItalicTable}{{font-style:italic}}";
            }

            return css;
        }

        private string Prep(Color c)
        {
            // background: rgba(128,128,128,1);
            return $"rgba({c.R},{c.G},{c.B},1)";
        }

        #endregion Methods
    }
}