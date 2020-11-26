using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.PageObjects;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;
using System.Drawing;
using System.Net;
using RestSharp;

namespace Automation
{
    public class Tests
    {
        IWebDriver driver;
        public int AddToCart(string newSize,int index)
        {           
            IWebElement view=driver.FindElement(By.XPath("//ul[@id='homefeatured']//li//div[@class='product-image-container']"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", view);    
            Thread.Sleep(5000);
            Actions action =new Actions(driver);
            action.MoveToElement(view).Perform();            
            driver.FindElement(By.XPath("(//ul[@id='homefeatured']//li//div[@class='product-image-container']//a[@class='quick-view']/span)["+index+"]")).Click();
            
            try
            {
                driver.SwitchTo().Frame(0);
                Thread.Sleep(2000);
                SelectElement select =new SelectElement(driver.FindElement(By.Id("group_1")));
                if(!String.IsNullOrEmpty(newSize))
                {
                    select.SelectByText(newSize);
                }
                Thread.Sleep(2000);
                driver.FindElement(By.Name("Submit")).Click();
                Thread.Sleep(5000);                
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not switch to Frame: "+e);

            }
            var price=driver.FindElement(By.XPath("//span[@id='our_price_display']")).Text;
            int amount=Convert.ToInt32(price.Replace("$",""));
            return amount;
            //var page = PageFactory.InitElements<Page>(driver);
            //page.ClickHoverView();
        }

        [SetUp]
        public void Setup()
        {
            driver=new ChromeDriver();
            string username="testing2111@gmail.com";
            string password="FujitsuTest";  
            driver.Navigate().GoToUrl("http://automationpractice.com/");
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(30);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(60);  
            driver.FindElement(By.XPath("//a[@class='login']")).Click();
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(60);  
            driver.FindElement(By.XPath("//input[@id='email']")).SendKeys(username);
            driver.FindElement(By.XPath("//input[@id='passwd']")).SendKeys(password);
            driver.FindElement(By.XPath("//button[@id='SubmitLogin']")).Click();
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(60);              

        }

        [Test]
        public void Test1()
        {
            string newSize="L";  
            var total=0;    
            driver.Navigate().GoToUrl("http://automationpractice.com/");
            driver.Manage().Timeouts().ImplicitWait= TimeSpan.FromSeconds(30);   
            total=total+AddToCart(newSize,1);
            string productName01=driver.FindElement(By.XPath("//span[@id='layer_cart_product_title']")).Text;
            driver.FindElement(By.XPath("//span[@title='Continue shopping']")).Click();
            driver.SwitchTo().DefaultContent();
            total=total+AddToCart("",2);   
            string productName02=driver.FindElement(By.XPath("//span[@id='layer_cart_product_title']")).Text;
            driver.FindElement(By.XPath("//a[@title='Proceed to checkout']")).Click();    

            //Verifying the Cart page
            Thread.Sleep(5000); 
            for (int i=1;i<=2;i++)
            {
                var name=driver.FindElement(By.XPath("(//table/tbody/tr/td[2]/p[@class='product-name'])["+i+"]")).Text;
                Assert.IsTrue(name.Equals("productName0"+i));
            }
            var ActualPrice=Convert.ToInt32((driver.FindElement(By.XPath("//span[@id=\'total_price\']")).Text).Replace("$",""));
            var shippingCharge=Convert.ToInt32((driver.FindElement(By.XPath("//table/tfoot/tr[@class=\'cart_total_delivery\']/td[2]")).Text).Replace("$",""));
            Assert.IsTrue((total+shippingCharge)==(ActualPrice+shippingCharge));

            driver.FindElement(By.XPath("(//a[@title='Proceed to checkout'])[2]")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//button[@name='processAddress']")).Click();
            Thread.Sleep(3000);            
            driver.FindElement(By.XPath("//input[@id='cgv']")).Click();
            driver.FindElement(By.XPath("//button[@name='processCarrier']")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//a[@class='bankwire']")).Click();
            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//button/span[text()='I confirm my order']")).Click();
            Thread.Sleep(5000);
            driver.FindElement(By.XPath("//a[@class='logout']")).Click();
            Thread.Sleep(5000);

        }

        [Test]
        public void Test2()
        {
         driver.FindElement(By.XPath("//a[@class='account']")).Click();
         Thread.Sleep(3000);
         driver.FindElement(By.XPath("//ul[@class='myaccount-link-list']//li//span[text()='Order history and details']")).Click();
         Thread.Sleep(3000);
         driver.FindElement(By.XPath("//table/tbody/tr/td[contains(@class,'history_detail')]/a/span")).Click();
         Thread.Sleep(3000);
         var productDetails= driver.FindElement(By.XPath("(//table[@class='table table-bordered']/tbody/tr[@class='item']/td/label)[2]")).Text;
         IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
         js.ExecuteScript("arguments[0].scrollIntoView(true);", productDetails);  
         if(!productDetails.Contains(" Color : Aqua"))
         {
            Screenshot image = ((ITakesScreenshot)driver).GetScreenshot();
            //Save the screenshot
            //image.SaveAsFile("C:/temp/Screenshot.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
         }
         var view=driver.FindElement(By.XPath("//select[@name='id_product']"));         
         js.ExecuteScript("arguments[0].scrollIntoView(true);", view);  
         SelectElement select =new SelectElement(view);
         select.SelectByIndex(0);
         Thread.Sleep(2000);
         driver.FindElement(By.XPath("//p[@class='form-group']/textarea[@name='msgText']")).SendKeys("Commenting!!");
         driver.FindElement(By.XPath("//button[@type='submit']/span[text()='Send']")).Click();
         driver.FindElement(By.XPath("//a[@class='logout']")).Click();
         Thread.Sleep(5000);

        }

        [Test]
        public void TestPOSTAPI()
        {
         var dataRequest01="{\"name\": \"morpheus\", \"job\": \"leader\"}";
         var dataResponse01="{\"name\": \"morpheus\", \"job\": \"leader\", \"id\": \"351\", \"createdAt\": \"2020-11-26T18:05:40.378Z\"}";
         using(WebClient web=new WebClient())
         {
             var restClient=new RestClient("https://reqres.in/");
             var restRequest = new RestRequest("/api/users",Method.POST);
             restRequest.AddHeader(HttpRequestHeader.Authorization.ToString(), "Bearer ");
             restRequest.AddParameter("application/json", dataRequest01, ParameterType.RequestBody);
             var response = restClient.Execute(restRequest);           
            var status=response.StatusCode;
            var content=response.Content;

            Assert.IsTrue(content.Equals(dataResponse01));
            Assert.IsTrue(status.Equals("200"));
         }
        }

        [Test]
        public void TestPUTAPI()
        {
         var dataRequest01="{\"name\": \"morpheus\", \"job\": \"zion resident\"}";
         var dataResponse01="{\"name\": \"morpheus\", \"job\": \"zion resident\", \"updatedAt\": \"2020-11-26T18:13:26.010Z\"}";

         using(WebClient web=new WebClient())
         {
             var restClient=new RestClient("https://reqres.in/");
             var restRequest = new RestRequest("/api/users/2",Method.PUT);
             restRequest.AddHeader(HttpRequestHeader.Authorization.ToString(), "Bearer");
             restRequest.AddParameter("application/json", dataRequest01, ParameterType.RequestBody);
             var response = restClient.Execute(restRequest);           
            var status=response.StatusCode;
            var content=response.Content;

            Assert.IsTrue(content.Equals(dataResponse01));
            Assert.IsTrue(status.Equals("200"));
         }
        }

        [Test]
        public void TestDELETEAPI()
        {         
         using(WebClient web=new WebClient())
         {
             var restClient=new RestClient("https://reqres.in/");
             var restRequest = new RestRequest("/api/users/2",Method.DELETE);
             restRequest.AddHeader(HttpRequestHeader.Authorization.ToString(), "Bearer");             
             var response = restClient.Execute(restRequest);           
             var status=response.StatusCode;
             Assert.IsTrue(status.Equals("204"));
         }
        }

        [TearDown]
        public void Close()
        {
            driver.Quit();
        }
    }
}