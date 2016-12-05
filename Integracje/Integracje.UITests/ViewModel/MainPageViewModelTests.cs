using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integracje.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integracje.UI.SrvBook;
using Newtonsoft.Json;
using EntityHelper;
using Procedure = Integracje.UI.SrvBook.Procedure;

namespace Integracje.UI.ViewModel.Tests
{
    [TestClass()]
    public class MainPageViewModelTests
    {
        [TestMethod()]
        public void MainPageViewModelTest()
        {
            var SelectedProcedure = new Procedure { Name = "GetBookById", Parameter = "2", HasParameter = true, ParameterName = "@id" };
            var ws = new BookService();
            var resultJson = ws.GetResultFromProcedure(SelectedProcedure);
            var Result = JsonConvert.DeserializeObject<ResultFromProcedure>(resultJson);

            Assert.AreEqual(Result.HasError, false);
            Assert.AreEqual(Result.EmptyResult, false);
            Assert.AreEqual(Result.WrongParameter, false);
        }
    }
}