using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AlcaldiaMsTest
{
    [TestClass]
    public class EmpleadoControllerTest
    {
        private IWebDriver _driver;
        private readonly string _base = "https://localhost:7234";

        // Datos únicos
        private string _testNombre;
        private string _testApellido;
        private readonly string _fechaContratacion = "01-01-2025";

        // IDs de Claves Foráneas (Se ASUME que estos IDs existen en la base de datos de pruebas)
        private const string CargoId = "1";
        private const string MunicipioId = "1";

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            // Generar datos únicos
            _testNombre = "NOMBRE_TEST_" + Guid.NewGuid().ToString().Substring(0, 5);
            _testApellido = "APELLIDO_TEST_" + Guid.NewGuid().ToString().Substring(0, 4);
        }

        // --- FUNCIÓN AUXILIAR PARA GARANTIZAR LA EXISTENCIA DEL DATO ---
        /// <summary>
        /// Navega a la vista de Crear y envía un empleado con datos únicos.
        /// </summary>
        private void CrearEmpleadoUnico(string nombre, string apellido)
        {
            _driver.Navigate().GoToUrl($"{_base}/Empleado/Create");

            // Llenar campos de texto
            _driver.FindElement(By.Name("Nombre")).SendKeys(nombre);
            _driver.FindElement(By.Name("Apellido")).SendKeys(apellido);
            _driver.FindElement(By.Name("Fecha_contratacion")).SendKeys(_fechaContratacion);

            // Seleccionar Dropdowns
            new SelectElement(_driver.FindElement(By.Name("CargoId"))).SelectByValue(CargoId);
            new SelectElement(_driver.FindElement(By.Name("MunicipioId"))).SelectByValue(MunicipioId);
            new SelectElement(_driver.FindElement(By.Name("Estado"))).SelectByValue("activo");

            // Enviar el formulario
            IWebElement submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();
        }

        // -----------------------------------------------------------------
        // --- TEST DE CREACIÓN (C) ---
        // -----------------------------------------------------------------
        [TestMethod]
        [Description("Verifica la creación exitosa de un nuevo Empleado con campos obligatorios.")]
        public void Test_01_CrearEmpleado()
        {
            // ACT: Utilizar los datos únicos de Setup
            CrearEmpleadoUnico(_testNombre, _testApellido);

            // ASSERT: Verificar que la creación fue exitosa
            // 1. Verificar redirección al Index
            Assert.IsTrue(_driver.Url.StartsWith($"{_base}/Empleado"), "La creación no redirigió correctamente al Index de Empleado.");

            // 2. Verificar que el empleado recién creado aparece en la lista (buscando por el nombre único)
            IWebElement createdEmployee = _driver.FindElement(By.XPath($"//td[contains(text(), '{_testNombre}')]"));
            Assert.IsNotNull(createdEmployee, $"El empleado con nombre '{_testNombre}' no se encontró en la lista.");
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