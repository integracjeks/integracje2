﻿using EntityHelper;
using Integracje.UI.Base;
using Integracje.UI.Helpers;
using Integracje.UI.Model;
using Integracje.UI.SrvBook;
using Integracje.UI.View;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        public bool StyleXml
        {
            get
            {
                return m_StyleXml;
            }

            set
            {
                SetProperty(ref m_StyleXml, value);
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

        #endregion Properties

        #region Fields

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

        #endregion Fields

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

        private async void ExecuteSelectedProcedure()
        {
            IsLoadingState = true;
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