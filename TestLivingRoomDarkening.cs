using System;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections;
using System.Threading;
using NUnit.Framework;

namespace BlindsAssignment
{

    [TestFixture]
    public class TestLivingRoomDarkening
    {
        public static IWebDriver driver;
        String baseURL;

        [SetUp]
        public static void SetUp()
        {
            driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
        }

        [TearDown]
        public static void TearDown()
        {
            driver.Quit();
        }
        [TestCase]
        public void ShouldBeAbleClickBlindsSearchAndGetDisplayByRoomPage()
        {
            baseURL = "http://www.blinds.com";
            driver.Navigate().GoToUrl(baseURL);
            try
            {
                clickSearchBlindsButton();
                Assert.AreEqual(true, verifyNextPageHasSearchByRoomSection());
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element Not found TC00001 failed");

            }
        }

        
        [TestCase]
        public void ShouldBeAbleClickLivingRoomAndDisplayAllProductsSortedByFeatured()
        {
            baseURL = "http://www.blinds.com/infopage/blind-finder";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                clickSearchLivingRoom();
                Assert.AreEqual(true, verifyAllProductsDisplayedSortedByFeatured());
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element Not found TC00002 failed");

            }

        }

        [TestCase]
        public void ShouldBeAbleSelectBlackoutOnlyProductsAndSortByPrice()
        {
            baseURL = "https://www.blinds.com/m/room/living-room/";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                checkOpacityBlackout();
                clickSortByPriceLowToHigh();
                Assert.AreEqual(true, verifyProductsSortedByPrice());
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element not found:TC00003 and TC00004 failed");
            }
        }
        [TestCase]
        public void ShouldBeAbleSelectOtherOptionsAndMatchNumberOfProducts()
        {
            baseURL = "https://www.blinds.com/m/room/living-room/";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                checkOpacityLightFiltering();
                clickSortByPriceLowToHigh();
                Assert.AreEqual(true, verifyProductsNumberMatch());
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element not found: TC00005 failed");
            }

        }

        [TestCase]
        public void ShouldNotDisplayDuplicateProducts()
        {
            baseURL = "https://www.blinds.com/m/room/living-room/";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                checkOpacityLightFiltering();
                clickSortByPriceLowToHigh();
                Assert.AreEqual(true, verifyNoDuplicateProducts());
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element not found: TC00006 failed");
            }

        }

        [TestCase]
        public void ShouldNotUpdatePriceIfNoChange()
        {
            baseURL = "https://www.blinds.com/m/room/living-room/";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                checkOpacityBlackout();
                clickSortByPriceLowToHigh();

                int priceBeforeUpdate = getPrice();
                clickUpdatePrice();
                int priceAfterUpdate = getPrice();

                Assert.AreEqual(priceBeforeUpdate, priceAfterUpdate);

            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element not found: TC00007 failed");
            }
        }

        [TestCase]
        public void ShouldBeAbleChangeHeightAndWidthAndUpdatePrice()
        {
            baseURL = "https://www.blinds.com/m/room/living-room/";
            driver.Navigate().GoToUrl(baseURL);

            try
            {
                checkOpacityBlackout();
                clickSortByPriceLowToHigh();

                int priceBeforeUpdate = getPrice();
                changeWidthAndHeight();
                int priceAfterUpdate = getPrice();

                Assert.AreNotEqual(priceBeforeUpdate, priceAfterUpdate);
            }
            catch (NoSuchElementException e)
            {
                System.Diagnostics.Trace.WriteLine("Element not found: TC00008 failed");
            }
        }

        private int getPrice()
        {
            int price = 0;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//ul[@id='bdc-product-search-result']/li[1]//div[@class='bdc-productsearch-pricing-real']//span[@class='bdc-productsearch-pricing-price']")));
            price = int.Parse(element.Text);
            return price;
        }

        private void changeWidthAndHeight()
        {
            IWebElement element = driver.FindElement(By.XPath("//div[@class='bdc-search-priceupdater-container']//select[@id='SelectedWidth']"));
            SelectElement dropDown = new SelectElement(element);
            dropDown.SelectByText("40");

            IWebElement element1 = driver.FindElement(By.XPath("//div[@class='bdc-search-priceupdater-container']//select[@id='SelectedHeight']"));
            SelectElement dropDown1 = new SelectElement(element1);
            dropDown1.SelectByText("65");

            clickUpdatePrice();
        }

        private void clickUpdatePrice()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@class='bdc-search-priceupdater-container']//button[@class='bdc-searchfilter-priceupdate-button']"))).Click();
        }


        private bool verifyNoDuplicateProducts()
        {
            HashSet<String> listOfNames = new HashSet<String>();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//ul[@id='bdc-product-search-result']/li[1]")));

            IList elements = driver.FindElements(By.XPath("//ul[@id='bdc-product-search-result']/li"));
            for (int i = 0; i < elements.Count; i++)
            {
                String name = ((IWebElement)elements[i]).FindElement(By.XPath("//ul[@id='bdc-product-search-result']/li[" + (i + 1) + "]//h2[@class='bdc-productwidget-title']//span[2]")).Text;
                if (!listOfNames.Add(name)) return false;
            }
            return true;
        }


        private void checkOpacityLightFiltering()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//section[@id='bdc-search-filter']//a[contains(@href,'Light Filtering')]"))).Click();
        }

        private bool verifyProductsNumberMatch()
        {
            int numOfProducts = 0;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//section[@id='bdc-search-filter']//a[text()='Opacity: Light Filtering']//following-sibling::span")));
            numOfProducts = int.Parse(element.Text);
            if (numberOfProductsDisplayed() == numOfProducts)
                return true;
            else
                return false;
        }


        private void checkOpacityBlackout()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//section[@id='bdc-search-filter']//a[contains(@href,'Blackout')]"))).Click();
        }

        private void clickSortByPriceLowToHigh()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            IWebElement element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@class='bdc-productsearch-sort']//a[text() = 'Price Low-High']")));
            try
            {
                element.Click();
            }
            catch (StaleElementReferenceException s)
            {
                driver.FindElement(By.XPath("//section[@id='bdc-search-filter']//a[contains(@href,'Blackout')]")).Click();
            }
        }

        private bool verifyProductsSortedByPrice()
        {
            int previousPrice = 0;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//ul[@id='bdc-product-search-result']/li[1]")));

            IList elements = driver.FindElements(By.XPath("//ul[@id='bdc-product-search-result']/li"));
            for (int i = 0; i < elements.Count; i++)
            {
                int currentPrice = int.Parse(((IWebElement)elements[i]).FindElement(By.XPath("//ul[@id='bdc-product-search-result']/li[" + (i + 1) + "]//div[@class='bdc-productsearch-pricing-real']//span[@class='bdc-productsearch-pricing-price']")).Text);
                if (currentPrice < previousPrice) return false;
                previousPrice = currentPrice;
            }
            return true;
        }

        private void clickSearchBlindsButton()
        {

            IWebElement element = driver.FindElement(By.XPath("//section[contains(@class, 'highlights')]//a[@href='/infopage/blind-finder']"));
            element.Click();
        }
        private bool verifyNextPageHasSearchByRoomSection()
        {
            return driver.FindElement(By.XPath("//div//h2[@id='by-room']")).Displayed;
        }

        private void clickSearchLivingRoom()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@id='getstarted']//a[@href='/m/room/living-room']")));
            element.Click();
        }

        private bool verifyAllProductsDisplayedSortedByFeatured()
        {
            return numberOfProductsDisplayed() > 0 && isProductsSortedByFeature();
        }
        
        private int numberOfProductsDisplayed()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//ul[@id='bdc-product-search-result']/li[1]")));
            return driver.FindElements(By.XPath(".//ul[@id='bdc-product-search-result']/li")).Count;
        }

        private bool isProductsSortedByFeature()
        {
            driver.FindElement(By.XPath("//div[@class='bdc-productsearch-sort']//a[@class='bdc-productsearch-sort-selected' and text() = 'Featured']"));
            return true;
        }
        
    }
}