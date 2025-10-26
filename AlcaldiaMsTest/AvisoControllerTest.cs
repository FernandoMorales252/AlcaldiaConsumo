using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlcaldiaMsTest
{
    [TestClass]
    public class AvisoControllerTest
    {
        private IWebDriver _driver;
        private readonly string _base = "https://localhost:44321";
        private string _testAviso;

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _testAviso = "Aviso-" + Guid.NewGuid().ToString().Substring(0, 6);
        }


        // --- PRUEBA DE CREACIÓN DE DOCUMENTO ---
        [TestMethod]
        [Description("Verifica la creación exitosa de un Aviso con campos obligatorios.")]
        public void Test_02_CrearAviso()
        {
            // ARRANGE
            string expectedNombre = "Item Test Selenium";
            string currentDate = DateTime.Now.ToString("10-10-2025");

            const string MunicipioId = "1";
            // 1. Navegar a la página de creación de Documento
            _driver.Navigate().GoToUrl($"{_base}/Aviso/Create");

            // 2. Llenar campos de texto únicos y fijos
            _driver.FindElement(By.Name("Titulo")).SendKeys(_testAviso);
            _driver.FindElement(By.Name("Fecha_Registro")).SendKeys(currentDate);
            _driver.FindElement(By.Name("Descripcion")).SendKeys($"Detalles del documento {_testAviso}");

            // 3. Seleccionar Dropdowns (Claves Foráneas)

            // b) Municipio
            IWebElement municipioDropdown = _driver.FindElement(By.Name("MunicipioId"));
            SelectElement selectMunicipio = new SelectElement(municipioDropdown);
            selectMunicipio.SelectByValue(MunicipioId); // Selecciona por el valor del ID (ej: "1")

            // c) Estado (Dropdown simple)
            IWebElement estadoDropdown = _driver.FindElement(By.Name("Tipo"));
            SelectElement selectEstado = new SelectElement(estadoDropdown);
            selectEstado.SelectByValue("climatico"); // Selecciona por el valor "climatico"


            // Escrollea para que pueda ver el botón de submit y lo cliquea
            IWebElement submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);

            submitButton.Click();

            //ASSERT
            Assert.IsTrue(_driver.Url.StartsWith($"{_base}/Aviso"), "La creación no redirigió correctamente al Index de Aviso.");


            IWebElement createdDocument = _driver.FindElement(By.XPath($"//td[contains(text(), '{_testAviso}')]"));
            Assert.IsNotNull(createdDocument, $"El Aviso con nombre {_testAviso} no se encontró en la lista.");
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
