using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OpenQA.Selenium.Support.UI; // Necesario para SelectElement, aunque no se use en Cargo si no hay FKs

namespace AlcaldiaMsTest
{
    [TestClass]
    public class CargoControllerTest
    {
        private IWebDriver _driver;
        private readonly string _base = "https://localhost:7234";
        private string _testCargoNombre; // Nombre único para el test de creación
        private string _testCargoDescripcion;

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            // Generar nombres y descripciones únicos para evitar colisiones
            _testCargoNombre = "CARGO_TEST_" + Guid.NewGuid().ToString().Substring(0, 5);
            _testCargoDescripcion = $"Descripción única para {_testCargoNombre}";
        }

        // --- TEST DE CREACIÓN (C) ---
        [TestMethod]
        [Description("Verifica la creación exitosa de un nuevo Cargo.")]
        public void Test_01_CrearCargo()
        {
            // ARRANGE & ACT: Simular el proceso de creación
            _driver.Navigate().GoToUrl($"{_base}/Cargo/Create");

            // 1. Llenar los campos del formulario
            _driver.FindElement(By.Name("Nombre_cargo")).SendKeys(_testCargoNombre);
            _driver.FindElement(By.Name("Descripcion")).SendKeys(_testCargoDescripcion);

            // 2. Enviar el formulario
            IWebElement submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            // ASSERT: Verificar que la creación fue exitosa
            // 1. Verificar redirección al Index
            Assert.IsTrue(_driver.Url.StartsWith($"{_base}/Cargo"), "La creación no redirigió correctamente al Index de Cargo.");

            // 2. Verificar que el cargo recién creado aparece en la lista
            IWebElement createdCargo = _driver.FindElement(By.XPath($"//td[contains(text(), '{_testCargoNombre}')]"));
            Assert.IsNotNull(createdCargo, $"El cargo con nombre '{_testCargoNombre}' no se encontró en la lista.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_driver != null)
            {
                // Cierra el navegador y libera recursos
                _driver.Quit();
            }
        }
    }
}