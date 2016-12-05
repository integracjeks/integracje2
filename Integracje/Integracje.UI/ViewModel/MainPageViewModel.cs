using EntityHelper;
using Integracje.UI.Helpers;
using Integracje.UI.Model;
using Integracje.UI.SrvBook;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Procedure = Integracje.UI.SrvBook.Procedure;

namespace Integracje.UI.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Fields

        private ICommand m_DownloadCommand;
        private bool m_IsSaveButtonVisible;
        private string m_OutputTextBox;
        private string m_ParameterTextBox;
        private ObservableCollection<Procedure> m_Procedures;
        private ICommand m_SaveCommand;
        private SaveFileDialog m_SaveFileDialog;
        private Procedure m_SelectedProcedure;

        #endregion Fields

        #region Constructors

        public MainPageViewModel()
        {
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

        public ICommand DownloadCommand
        {
            get
            {
                if (m_DownloadCommand == null)
                {
                    m_DownloadCommand = new DelegateCommand(ExecuteSelectedProcedure, CanExecuteDownloadCommand);
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

        #endregion Properties

        #region Methods

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
        }

        private void AnalyzeIfNoError()
        {
            if (Result.EmptyResult)
            {
                OutputTextBox = "Brak rekordów";
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
            return SelectedProcedure != null ? true : false;
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
                //no code
                //no problem
            }
        }

        private void ExecuteSelectedProcedure()
        {
            IsSaveButtonVisible = false;
            ConfigureSaveFileDialog();
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
                OutputTextBox = e.Message;
            }
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
                    document = Result.Xml;
                    break;
            }

            File.WriteAllText(fileName, document);
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