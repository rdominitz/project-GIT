using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;
using Server;
using Entities;
using Constants;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AutomationTestsForGUI
{


    [TestClass]
    public class LoginAuto
    {
        private string baseURL = "http://localhost:9153/login";
        private RemoteWebDriver driver;



        [TestInitialize()]
        public void Initialize()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-extensions");
            options.AddArguments("--start-maximized");
            options.ToCapabilities();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            driver = new ChromeDriver(service, options);
        }



        [TestMethod]
        [TestCategory("Selenium")]
        public void Login_Test()
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            //Thread.Sleep(3 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            //Thread.Sleep(3 * 1000);
            driver.Close();

        }

        [TestMethod]
        [TestCategory("Selenium")]
        public void LoginFail_Test()
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("something");
            //Thread.Sleep(3 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsFalse(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            //Thread.Sleep(2 * 1000);
            driver.Close();

        }
    }
}
