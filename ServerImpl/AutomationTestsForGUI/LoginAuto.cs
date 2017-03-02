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
        private Process _iisProcess;
        private string baseURL = "http://localhost:9153/";
        private RemoteWebDriver driver;
        public TestContext TestContext { get; set; }
        private void StartIIS()
        {
            var applicationPath = GetApplicationPath("communication");
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            _iisProcess.StartInfo.Arguments = string.Format("/path:{0}/{1}",  applicationPath, "2020");
            _iisProcess.Start();
        }


        protected virtual string GetApplicationPath(string applicationName)
        {
            var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            return Path.Combine(solutionFolder, applicationName);
        }


        public string GetAbsoluteUrl(string relativeUrl)
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return String.Format("http://localhost:{0}{1}", 9153, relativeUrl);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Start IISExpress
            //StartIIS();
            //Thread.Sleep(70 * 1000);

            
        }


        [TestCleanup]
        public void TestCleanup()
        {
            // Ensure IISExpress is stopped
         //   if (_iisProcess.HasExited == false)
        //    {
        //        _iisProcess.Kill();
          //  }

            
        }



        /*     [TestInitialize]
             public void TestInitialize()
             {
                 _server = new ServerImpl(new FakeMedTrainDBContext());
                 _server.login("defaultadmin@gmail.com", "password");
                 _server.addSubject(Users.USER_UNIQUE_INT, "subject");
                 _server.addTopic(Users.USER_UNIQUE_INT, "subject", "topic");
                 _server.addQuestion(Users.USER_UNIQUE_INT, "subject", true, "", new List<string>() { "topic" });
             }*/

        [TestMethod]
        [TestCategory("Selenium")]
        public void Login_Test()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            Thread.Sleep(3 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            Thread.Sleep(3 * 1000);
            driver.Close();

        }

        [TestMethod]
        [TestCategory("Selenium")]
        public void LoginFail_Test()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("something");
            Thread.Sleep(3 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsFalse(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            Thread.Sleep(2 * 1000);
            driver.Close();

        }
    }
}
