using EntityHelper;
using Integracje.UI.SrvBook;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Procedure = Integracje.UI.SrvBook.Procedure;

namespace Integracje.UI.ViewModel.Tests
{
    [TestClass()]
    public class MainPageViewModelTests
    {
        #region Methods

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

        #endregion Methods
    }
}