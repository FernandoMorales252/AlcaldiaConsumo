using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AlcaldiaMsTest
{
    [TestClass]
    public class DocumentoControllerTest
    {
        private IWebDriver _driver;
        private readonly string _base = "https://localhost:44321";
        private string _testName;
        private string _testNumeroDocumento;

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _testName = "TIPO_DOC_" + Guid.NewGuid().ToString().Substring(0, 8);
            _testNumeroDocumento = "DOC-" + Guid.NewGuid().ToString().Substring(0, 6); 
        }

        
        // --- PRUEBA DE CREACIÓN DE DOCUMENTO ---
        [TestMethod]
        [Description("Verifica la creación exitosa de un Documento con campos obligatorios.")]
        public void Test_02_CrearDocumento()
        {
            // ARRANGE
            string expectedPropietario = "Propietario Test Selenium";
            string currentDate = DateTime.Now.ToString("10-10-2025"); 
            
            // Suponemos que estos IDs existen en la base de datos de pruebas.
            const string TipoDocId = "3"; 
            const string MunicipioId = "1";
            
            // 1. Navegar a la página de creación de Documento
            _driver.Navigate().GoToUrl($"{_base}/Documento/Create");

            // 2. Llenar campos de texto únicos y fijos
            _driver.FindElement(By.Name("Numero_documento")).SendKeys(_testNumeroDocumento);
            _driver.FindElement(By.Name("Fecha_emision")).SendKeys(currentDate); 
            _driver.FindElement(By.Name("Propietario")).SendKeys(expectedPropietario);
            _driver.FindElement(By.Name("Detalles")).SendKeys($"Detalles del documento {_testNumeroDocumento}");

            // 3. Seleccionar Dropdowns (Claves Foráneas)
            
            // a) Tipo de Documento
            IWebElement tipoDocDropdown = _driver.FindElement(By.Name("TipoDocumentoId"));
            SelectElement selectTipoDoc = new SelectElement(tipoDocDropdown);
            selectTipoDoc.SelectByValue(TipoDocId); // Selecciona por el valor del ID (ej: "1")

            // b) Municipio
            IWebElement municipioDropdown = _driver.FindElement(By.Name("MunicipioId"));
            SelectElement selectMunicipio = new SelectElement(municipioDropdown);
            selectMunicipio.SelectByValue(MunicipioId); // Selecciona por el valor del ID (ej: "1")

            // c) Estado (Dropdown simple)
            IWebElement estadoDropdown = _driver.FindElement(By.Name("Estado"));
            SelectElement selectEstado = new SelectElement(estadoDropdown);
            selectEstado.SelectByValue("vigente"); // Selecciona por el valor "vigente"

            IWebElement submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);

            submitButton.Click();

        
            Assert.IsTrue(_driver.Url.StartsWith($"{_base}/Documento"), "La creación no redirigió correctamente al Index de Documento.");
            
            
            IWebElement createdDocument = _driver.FindElement(By.XPath($"//td[contains(text(), '{_testNumeroDocumento}')]"));
            Assert.IsNotNull(createdDocument, $"El documento con número {_testNumeroDocumento} no se encontró en la lista.");
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
