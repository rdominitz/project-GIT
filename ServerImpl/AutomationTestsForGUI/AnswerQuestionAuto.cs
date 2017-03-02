using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace AutomationTestsForGUI
{
    [TestClass]
    public class AnswerQuestionAuto
    {
        private string baseURL = "http://localhost:9153/";
        private RemoteWebDriver driver;

        [TestMethod]
        [TestCategory("Selenium")]
        public void AnswerQuestionWithMultiQuestionsAndAnswersAtTheEnd_Test()
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
            Thread.Sleep(2 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Main"));
            Thread.Sleep(2 * 1000);
            driver.FindElementById("test").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Get Test"));
            SelectElement selector = new SelectElement(driver.FindElementById("subject"));
            selector.SelectByIndex(0);
            SelectElement topic_selector = new SelectElement(driver.FindElementById("topic"));
            topic_selector.SelectByIndex(0);
            Thread.Sleep(2 * 1000);
            driver.FindElementById("numberOfQuestions").Clear();
            driver.FindElementById("numberOfQuestions").SendKeys("3");
            SelectElement when_to_answer = new SelectElement(driver.FindElementByName("whenToShowAnswer"));
            when_to_answer.SelectByIndex(1);
            Thread.Sleep(2 * 1000);
            driver.FindElementById("submit").Click();
            Assert.IsTrue(driver.FindElementById("meta").GetAttribute("name").Equals("Answer Question"));
            Thread.Sleep(2 * 1000);

            driver.Close();

        }
    }
}
