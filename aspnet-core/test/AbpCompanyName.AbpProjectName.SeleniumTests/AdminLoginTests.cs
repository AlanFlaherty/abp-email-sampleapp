using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.IE;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AbpCompanyName.AbpProjectName.SeleniumTests
{
    [TestClass]
    [TestCategory("Selenium")]
    public class AdminLoginTests
    {
        private string baseURL = "https://at-abp-email-develop.azurewebsites.net/def";
        private RemoteWebDriver driver;
        private string browser = string.Empty;
        public TestContext TestContext { get; set; }

        [TestMethod]
        // [TestCategory("Selenium")]
        [Priority(1)]
        [Owner("FireFox")]
        public void Admin_Login_With_Valid_Credentials_Shows_Application()
        {
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl(baseURL);

            //do other Selenium things here!
            // Assert.IsTrue()
            IWebElement txtUserNameOrEmailAddress = driver.FindElement(By.Name("userNameOrEmailAddress"));
            IWebElement txtPassword = driver.FindElement(By.Name("password"));
            IWebElement btnLogin = driver.FindElementById("LoginButton");
            
            txtUserNameOrEmailAddress.SendKeys("admin");
            txtPassword.SendKeys("123qwe");
            btnLogin.Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(a => a.FindElement(By.ClassName("block-header")).Displayed );
            
            Assert.AreEqual(".\\admin", driver.FindElementByCssSelector(".name").Text);
        }

        [TestMethod]
        // [TestCategory("Selenium")]
        [Priority(1)]
        [Owner("FireFox")]
        public void Admin_Login_With_InValid_Credentials_Shows_Error_Messsage()
        {
//            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl(baseURL);

            //do other Selenium things here!
            // Assert.IsTrue()
            IWebElement txtUserNameOrEmailAddress = driver.FindElement(By.Name("userNameOrEmailAddress"));
            IWebElement txtPassword = driver.FindElement(By.Name("password"));
            IWebElement btnLogin = driver.FindElementById("LoginButton");

            txtUserNameOrEmailAddress.SendKeys("admin");
            txtPassword.SendKeys("BADPASS");
            btnLogin.Click();

            // Error Message Holder should get displayed
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(a => a.FindElement(By.ClassName("sweet-alert")).Displayed);

            // Test 
            Assert.AreEqual("Login failed!", driver.FindElementByCssSelector(".sweet-alert>h2").Text);
            Assert.AreEqual("Invalid user name or password", driver.FindElementByCssSelector(".sweet-alert>p").Text);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            driver.Quit();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Set the browswer from a build
            browser = this.TestContext.Properties["browser"] != null ? this.TestContext.Properties["browser"].ToString() : "chrome";
            switch (browser)
            {
                case "firefox":
                    driver = new FirefoxDriver();
                    break;
                case "chrome":
                    // TODO: Failing on some tests, need to investigate
                    // ChromeOptions options = new ChromeOptions();
                    // options.AddArgument("--headless");
                    // driver = new ChromeDriver(options);
                    
                    driver = new ChromeDriver();
                    break;
                case "ie":
                    driver = new InternetExplorerDriver();
                    break;
                case "PhantomJS":
                    driver = new PhantomJSDriver();
                    break;
                default:
                    driver = new PhantomJSDriver();
                    break;
            }
            if (this.TestContext.Properties["webAppUrl"] != null) //Set URL from a build
            {
                this.baseURL = this.TestContext.Properties["webAppUrl"].ToString();
            }
            else
            {
                this.baseURL = "https://at-abp-email-develop.azurewebsites.net/null"; //default URL just to get started with
            }
        }
    }
}
