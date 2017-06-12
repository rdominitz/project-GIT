
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace AutomationTestsForGUI
{
    [TestClass]
    public class AdminTestsAuto
    {
        private string baseURL = "http://localhost:9153/login";
        private static RemoteWebDriver driver;

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
        public void Administration_Test()
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);
            driver.FindElementById("email").Clear();
            driver.FindElementById("email").SendKeys("defaultadmin@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            //Thread.Sleep(2 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("administration").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("administration"));
            driver.FindElementByName("addSubject").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("add subject"));
            driver.FindElementById("subjectName").SendKeys("newSubject");
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementByName("addTopic").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("add topic"));
            SelectElement selector = new SelectElement(driver.FindElementById("subject"));
            selector.SelectByValue("newSubject");
            driver.FindElementById("topicName").SendKeys("newTopic");
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementByName("removeQuestions").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Select subject and topic to remove questions"));
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("remove questions"));
            Assert.IsTrue(driver.FindElementsByName("QuestionData").Count == 4);
            driver.FindElementsByName("QuestionData")[0].Click();
            driver.FindElementById("reason").SendKeys("some reason");
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementByName("removeQuestions").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Select subject and topic to remove questions"));
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("remove questions"));
            Assert.IsTrue(driver.FindElementsByName("QuestionData").Count == 3);
            driver.FindElementByClassName("navbar-brand").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("administration").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("administration"));
            driver.FindElementByName("createGroup").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Create Group"));
            driver.FindElementById("groupName").SendKeys("newGroup");
            driver.FindElementByName("inviteEmails").SendKeys("user@gmail.com");
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementByName("manageMyGroups").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Manage group"));
            selector = new SelectElement(driver.FindElementById("groupName"));
            selector.SelectByValue("newGroup (created by defaultadmin@gmail.com)");
            driver.FindElementByName("addTestToGroup").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Add test to group"));
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("see test details"));
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementByName("setUserAsAdmin").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Set User As Admin"));
            driver.FindElementById("userEmail").SendKeys("user@gmail.com");
            driver.FindElementById("submit").Click();
            submitAndPopup();
            driver.FindElementsByClassName("navbar-brand")[1].Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Login"));
            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("administration").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("administration"));
            driver.FindElementByClassName("navbar-brand").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            driver.FindElementById("myGroups").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("MyGroups"));
            selector = new SelectElement(driver.FindElementById("groupInvites"));
            selector.SelectByText("newGroup (created by defaultadmin@gmail.com)");
            driver.FindElementById("submit").Click();
            driver.FindElementById("newGroup (created by defaultadmin@gmail.com)").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Group"));
            selector = new SelectElement(driver.FindElementById("testID"));
            selector.SelectByIndex(0);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Answer Question"));
            driver.Close();
        }


        private void submitAndPopup()
        {
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
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("administration"));
        }
    }
}