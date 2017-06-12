using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace AutomationTestsForGUI
{
    [TestClass]
    public class GroupTestsAuto
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
        public void Group_Test()
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            //Thread.Sleep(2 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("myGroups").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("MyGroups"));
            driver.FindElementByClassName("navbar-brand").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("myGroups").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("MyGroups"));
            SelectElement selector = new SelectElement(driver.FindElementById("groupInvites"));
            selector.SelectByText("Basic diagnosis (created by aCohen@post.bgu.ac.il)");
            driver.FindElementById("submit").Click();
            selector = new SelectElement(driver.FindElementById("groupInvites"));
            selector.SelectByIndex(0);
            selector.SelectByIndex(1);
            driver.FindElementsByName("submitButton")[1].Click();
            driver.FindElementById("Basic diagnosis (created by aCohen@post.bgu.ac.il)").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Group"));
            selector = new SelectElement(driver.FindElementById("testID"));
            selector.SelectByIndex(0);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Answer Question"));
            driver.FindElementsByName("norm")[1].Click();
            IWebElement slider = driver.FindElementByName("sure1");
            OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(driver);
            actions.ClickAndHold(slider);
            actions.MoveByOffset(40, 0).Build().Perform();
            actions.Release(slider);
            driver.FindElementByClassName("TokenSearch").FindElement(By.TagName("input")).SendKeys("ca");
            driver.FindElementByClassName("Dropdown").FindElement(By.TagName("li")).Click();
            IWebElement slider2 = driver.FindElementByName("sure2");
            actions.ClickAndHold(slider2);
            actions.MoveByOffset(25, 0).Build().Perform();
            actions.Release();
            driver.FindElementById("submit").Click();
            for (int i = 1; i <= 8; i++)
            {
                driver.FindElementById("submit").Click();
            }
            bool presentFlag = false;

            try
            {
                // Check the presence of alert
                IAlert alert = driver.SwitchTo().Alert();
                // Alert present; set the flag
                presentFlag = true;
                //Thread.Sleep(3000);
                // if present consume the alert
                alert.Accept();
            }
            catch (NoAlertPresentException ex)
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(presentFlag);
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.Close();
        }
    }
}