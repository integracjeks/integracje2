using EntityHelper;
using Integracje.UI.Base;
using Integracje.UI.Helpers;
using Integracje.UI.Model;
using Integracje.UI.SrvBook;
using Integracje.UI.Style;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Procedure = Integracje.UI.SrvBook.Procedure;

namespace Integracje.UI.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        //test

        #region Constructors

        public MainPageViewModel()
        {
            InitializeStyles();
            mmStyleSource = new List<StyleTemplate> { style1, style2, style3 };
            StyleSource = new List<StyleTemplate>(mmStyleSource);
            TabIndex = 0;

            Procedures = new ObservableCollection<Procedure>
            {
                new Procedure {Name= "GetAllBooks",HasParameter=false},
                new Procedure {Name= "GetBookById",HasParameter=true,ParameterName="@id"},
                new Procedure {Name= "GetMostTranslatedBooks",HasParameter=false},
                new Procedure {Name= "GetOldestBookOrBooks",HasParameter=false},
                new Procedure {Name= "GetYoungestBookOrBooks",HasParameter=false},
                new Procedure {Name= "GetAllFactBasedBooks",HasParameter=false},
                new Procedure {Name= "GetAllNonFactBasedBooks",HasParameter=false},
                new Procedure {Name= "GetAllBooksInYear",HasParameter=true,ParameterName="@year"},
                new Procedure {Name= "GetAllBooksWrittenByMen",HasParameter=false},
                new Procedure {Name= "GetAllBooksWrittenByWomen",HasParameter=false}
            };
        }

        #endregion Constructors

        #region Properties

        public int TabIndex
        {
            get
            {
                return m_TabIndex;
            }
            set
            {
                SetProperty(ref m_TabIndex, value);
            }
        }

        public ICommand SaveAndCloseCustomizePanel
        {
            get
            {
                if (m_SaveAndCloseCustomizePanel == null)
                {
                    m_SaveAndCloseCustomizePanel = new DelegateCommand(() =>
                    {
                        IsCustomizePanelVisible = false;
                    });
                }
                return m_SaveAndCloseCustomizePanel;
            }
        }

        public ICommand DownloadCommand
        {
            get
            {
                if (m_DownloadCommand == null)
                {
                    m_DownloadCommand = new DelegateCommand(async () => await ExecuteSelectedProcedure(), CanExecuteDownloadCommand);
                }
                return m_DownloadCommand;
            }
        }

        public bool IsSaveButtonVisible
        {
            get { return m_IsSaveButtonVisible; }
            set { SetProperty(ref m_IsSaveButtonVisible, value); }
        }

        public string OutputTextBox
        {
            get { return m_OutputTextBox; }
            set { SetProperty(ref m_OutputTextBox, value); }
        }

        public string ParameterTextBox
        {
            get { return m_ParameterTextBox; }
            set { SetProperty(ref m_ParameterTextBox, value); }
        }

        public ObservableCollection<Procedure> Procedures
        {
            get { return m_Procedures; }
            set { SetProperty(ref m_Procedures, value); }
        }

        public ResultFromProcedure Result { get; set; }
        public List<Book> ResultBooks { get; set; }

        public ICommand SaveCommand
        {
            get
            {
                if (m_SaveCommand == null)
                {
                    m_SaveCommand = new DelegateCommand(SaveCommandAction);
                }
                return m_SaveCommand;
            }
        }

        public Procedure SelectedProcedure
        {
            get { return m_SelectedProcedure; }
            set
            {
                SetProperty(ref m_SelectedProcedure, value);
                IsSaveButtonVisible = false;
                ((DelegateCommand)DownloadCommand).RaiseCanExecuteChanged();
            }
        }

        public bool StyleXml
        {
            get
            {
                return m_StyleXml;
            }

            set
            {
                SetProperty(ref m_StyleXml, value);

                if (StyleXml)
                {
                    IsCustomizePanelVisible = true;
                }
                Debug.WriteLine($"---------- StyleXml: {StyleXml}");
            }
        }

        public bool IsLoadingState
        {
            get { return m_IsLoadingState; }
            set
            {
                SetProperty(ref m_IsLoadingState, value);
            }
        }

        public bool IsCustomizePanelVisible
        {
            get { return m_IsCustomizePanelVisible; }
            set { SetProperty(ref m_IsCustomizePanelVisible, value); }
        }

        public List<StyleTemplate> StyleSource
        {
            get { return m_StyleSource; }
            set { SetProperty(ref m_StyleSource, value); }
        }

        #endregion Properties

        #region Fields

        private readonly List<int> fontSizes = new List<int> { 10, 12, 14, 16, 18 };
        private readonly List<TextAlign> textAligns = new List<TextAlign> { TextAlign.left, TextAlign.center, TextAlign.right };
        private readonly List<Color> colorList = new List<Color> { Color.LightBlue, Color.LightCoral, Color.LightGray, Color.Lime, Color.LimeGreen, Color.DeepPink, Color.Gray, Color.Brown, Color.Aqua, Color.Red, Color.Blue, Color.Green, Color.Magenta, Color.DeepSkyBlue };
        private readonly List<ItalicTable> italicTable = new List<ItalicTable> { ItalicTable.none, ItalicTable.id, ItalicTable.pages, ItalicTable.year, ItalicTable.title };
        private readonly List<int> borderSize = new List<int> { 1, 2, 3 };

        private readonly IEnumerable<StyleTemplate> mmStyleSource;

        private StyleTemplate style1;

        private StyleTemplate style2;

        private StyleTemplate style3;

        private int m_TabIndex;

        private ICommand m_SaveAndCloseCustomizePanel;

        private bool m_StyleXml;

        private ICommand m_DownloadCommand;

        private bool m_IsSaveButtonVisible;

        private string m_OutputTextBox;

        private string m_ParameterTextBox;

        private ObservableCollection<Procedure> m_Procedures;

        private ICommand m_SaveCommand;

        private SaveFileDialog m_SaveFileDialog;

        private Procedure m_SelectedProcedure;

        private bool m_IsLoadingState;

        private bool m_IsCustomizePanelVisible;

        private List<StyleTemplate> m_StyleSource;

        #endregion Fields

        #region Methods

        private void InitializeStyles()
        {
            style1 = new StyleTemplate
            {
                TemplateName = "Styl 1",
                ColorsList = colorList,
                SelectedRowColor = colorList[0],
                SelectedCellColor = colorList[1],
                FontSizes = fontSizes,
                SelectedFontSize = fontSizes.FirstOrDefault(),
                TextAligns = textAligns,
                SelectedTextAlign = textAligns[0],
                ItalicTables = italicTable,
                SelectedItalicTable = italicTable[0],
                SelectedDocumentColor = colorList[6],
                SelectedTableColor = colorList[7],
                BoldHeader = true,
                BorderSizes = borderSize,
                SelectedBorderSize = borderSize[2],
                SelectedBorderColor = colorList[4]
            };
            style2 = new StyleTemplate
            {
                TemplateName = "Styl 2",
                ColorsList = colorList,
                SelectedRowColor = colorList[2],
                SelectedCellColor = colorList[3],
                FontSizes = fontSizes,
                SelectedFontSize = fontSizes[1],
                TextAligns = textAligns,
                SelectedTextAlign = textAligns[1],
                ItalicTables = italicTable,
                SelectedItalicTable = italicTable[1],
                SelectedDocumentColor = colorList[8],
                SelectedTableColor = colorList[9],
                BoldHeader = false,
                BorderSizes = borderSize,
                SelectedBorderSize = borderSize[1],
                SelectedBorderColor = colorList[7]
            };
            style3 = new StyleTemplate
            {
                TemplateName = "Styl 3",
                ColorsList = colorList,
                SelectedRowColor = colorList[4],
                SelectedCellColor = colorList[5],
                FontSizes = fontSizes,
                SelectedFontSize = fontSizes[3],
                TextAligns = textAligns,
                SelectedTextAlign = textAligns[2],
                ItalicTables = italicTable,
                SelectedItalicTable = italicTable[2],
                SelectedDocumentColor = colorList[10],
                SelectedTableColor = colorList[11],
                BoldHeader = false,
                BorderSizes = borderSize,
                SelectedBorderSize = borderSize[2],
                SelectedBorderColor = colorList[1]
            };
        }

        private void AnalyzeIfError()
        {
            if (Result.WrongParameter)
            {
                OutputTextBox = "Zły parametr";
            }
            else
            {
                OutputTextBox = Result.ErrorMessage;
            }
            IsSaveButtonVisible = false;
            MessageBox.Show(OutputTextBox);
        }

        private void AnalyzeIfNoError()
        {
            if (Result.EmptyResult)
            {
                OutputTextBox = "Brak rekordów";
                IsSaveButtonVisible = false;
                MessageBox.Show(OutputTextBox);
            }
            else
            {
                OutputTextBox = Result.Xml;
                CreateResultBooksList();
                IsSaveButtonVisible = true;
            }
        }

        private void AnalyzeResult()
        {
            if (Result.HasError)
            {
                AnalyzeIfError();
            }
            else
            {
                AnalyzeIfNoError();
            }
        }

        private bool CanExecuteDownloadCommand()
        {
            return SelectedProcedure != null;
        }

        private void ConfigureSaveFileDialog()
        {
            if (m_SaveFileDialog == null)
            {
                m_SaveFileDialog = new SaveFileDialog();
            }
            m_SaveFileDialog.FileOk -= Sfd_FileOk;
            m_SaveFileDialog.FileOk += Sfd_FileOk;

            m_SaveFileDialog.FileName = "Result";
            m_SaveFileDialog.Filter = "Plik XML (*.xml)|*.xml|Plik YAML (*.yaml)|*.yaml|" +
                "Plik OGDL (*.ogdl)|*.ogdl|Plik JSON (*.json)|*.json";
        }

        private void CreateResultBooksList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Book>), new XmlRootAttribute("Books"));
            StringReader stringReader = new StringReader(Result.Xml);
            ResultBooks = null;
            try
            {
                var r = serializer.Deserialize(stringReader);
                ResultBooks = r as List<Book>;
            }
            catch
            {
                //
            }
        }

        private async Task ExecuteSelectedProcedure()
        {
            IsLoadingState = true;
            StyleXml = false;
            IsSaveButtonVisible = false;
            ConfigureSaveFileDialog();
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    SelectedProcedure.Parameter = ParameterTextBox;
                    var ws = new BookService();
                    var resultJson = ws.GetResultFromProcedure(SelectedProcedure);
                    Result = JsonConvert.DeserializeObject<ResultFromProcedure>(resultJson);
                    AnalyzeResult();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    OutputTextBox = e.Message;
                }
            });
            await task;
            IsLoadingState = false;
        }

        private string GetJsonFromResult()
        {
            if (ResultBooks == null)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(ResultBooks);
        }

        private string GetOgdlFromResult()
        {
            if (ResultBooks == null)
            {
                return string.Empty;
            }

            return OgdlCreator.GetOgdl(GetYamlFromResult());
        }

        private string GetYamlFromResult()
        {
            if (ResultBooks == null)
            {
                return string.Empty;
            }

            var serializer = new Serializer();
            return serializer.Serialize(ResultBooks);
        }

        private void SaveCommandAction()
        {
            m_SaveFileDialog.ShowDialog();
        }

        private void SaveDocument(string fileName, SaveFormatName saveFormatName)
        {
            string document;

            switch (saveFormatName)
            {
                case SaveFormatName.JSON:
                    document = GetJsonFromResult();
                    break;

                case SaveFormatName.OGDL:
                    document = GetOgdlFromResult();
                    break;

                case SaveFormatName.YAML:
                    document = GetYamlFromResult();
                    break;

                default:
                    document = GetXmlFromResult();
                    if (StyleXml)
                    {
                        var cssfilename = fileName.Substring(0, fileName.Length - 3) + "css";
                        File.WriteAllText(cssfilename, StyleSource[TabIndex].GenerateCssString());
                    }
                    break;
            }

            File.WriteAllText(fileName, document);
        }

        private string GetXmlFromResult()
        {
            if (StyleXml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(Result.Xml);
                doc.AppendChild(doc.CreateProcessingInstruction(
                    "xml-stylesheet",
                    "type='text/css' href='Result.css'"));

                var node = doc.CreateNode("element", "Book", "");
                node.InnerXml = "<id class='nagl'>id</id><title class='nagl'>title</title><pages class='nagl'>pages</pages><year class='nagl'>year</year><isbn class='nagl'>isbn</isbn><genre class='nagl'>genre</genre><price class='nagl'>price</price><authors_first_name class='nagl'>name</authors_first_name><authors_last_name class='nagl'>last name</authors_last_name><fact_based class='nagl'>fact based</fact_based><toms_quantity class='nagl'>toms quantity</toms_quantity><authors_email class='nagl'>authors email</authors_email><authors_gender class='nagl'>authors gender</authors_gender><original_lanuguage class='nagl'>original language</original_lanuguage><translated_languages_quantity class='nagl'>translated languages quantity</translated_languages_quantity>";

                doc.FirstChild.InsertBefore(node, doc.FirstChild.FirstChild);

                var styledXml = doc.InnerXml;
                return styledXml;
            }
            return Result.Xml;
        }

        private void Sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var saveFileDialog = sender as SaveFileDialog;
            if (saveFileDialog == null) return;

            var safeFileNameArray = saveFileDialog.SafeFileName.Split('.');
            var extension = safeFileNameArray[1].ToLowerInvariant();

            SaveFormatName saveFormatName = SaveFormatName.XML;

            if (extension.Equals(SaveFormat.XML))
            {
                saveFormatName = SaveFormatName.XML;
            }
            else if (extension.Equals(SaveFormat.JSON))
            {
                saveFormatName = SaveFormatName.JSON;
            }
            else if (extension.Equals(SaveFormat.OGDL))
            {
                saveFormatName = SaveFormatName.OGDL;
            }
            else if (extension.Equals(SaveFormat.YAML))
            {
                saveFormatName = SaveFormatName.YAML;
            }

            SaveDocument(saveFileDialog.FileName, saveFormatName);
        }

        #endregion Methods
    }
}