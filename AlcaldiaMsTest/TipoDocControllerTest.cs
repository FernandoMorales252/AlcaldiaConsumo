using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading; 

namespace AlcaldiaMsTest
{
    [TestClass]
    public class TipoDocControllerTest
    {
        private IWebDriver _driver;
        private readonly string _base = "https://localhost:44321";
        private string _testName;
       
        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _testName = "TEST_" + Guid.NewGuid().ToString().Substring(0, 8);
        }

      
        private void CrearRegistroBase(string name)
        {
            _driver.Navigate().GoToUrl($"{_base}/TipoDoc/Create");
            _driver.FindElement(By.Name("Nombre")).SendKeys(name);
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            Assert.IsTrue(_driver.Url.StartsWith($"{_base}/TipoDoc"), "La creación no redirigió correctamente al Index.");
        }

        [TestMethod]
        [Description("Verifica que la creación redirige y el registro aparece en la lista.")]
        public void Test_01_CrearTipoDoc()
        {
            CrearRegistroBase(_testName);

            IWebElement createdRow = _driver.FindElement(By.XPath($"//td[contains(text(), '{_testName}')]"));
            Assert.IsNotNull(createdRow, "El registro creado no se encontró en la página de Index.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }
        }
    }
}
