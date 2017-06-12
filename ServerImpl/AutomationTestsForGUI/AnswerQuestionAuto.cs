using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutomationTestsForGUI
{
    [TestClass]
    public class AnswerQuestionAuto
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
        public void AnswerQuestionWithMultiQuestionsAndAnswersAtTheEnd_Test()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(this.baseURL);


            driver.FindElementById("email").SendKeys("user@gmail.com");
            driver.FindElementById("password").Clear();
            driver.FindElementById("password").SendKeys("password");
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));

            driver.FindElementById("test").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Get Test"));
            SelectElement selector = new SelectElement(driver.FindElementById("subject"));
            selector.SelectByIndex(0);
            SelectElement topic_selector = new SelectElement(driver.FindElementById("topics"));
            topic_selector.SelectByIndex(0);
            driver.FindElementById("numberOfQuestions").Clear();
            driver.FindElementById("numberOfQuestions").SendKeys("3");
            SelectElement when_to_answer = new SelectElement(driver.FindElementByName("whenToShowAnswer"));
            when_to_answer.SelectByIndex(1);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Answer Question"));
            driver.Close();

        }
    }
}