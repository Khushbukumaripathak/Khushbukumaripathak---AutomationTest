using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Automation
{
    public class Page
    {
        [FindsBy(How = How.XPath, Using = "//ul[@id='homefeatured']//li//div[@class='product-image-container'])")]
        public IWebElement QuickViewHover { get; set; }

        [FindsBy(How = How.XPath, Using = "//ul[@id='homefeatured']//li//div[@class='product-image-container']//a[@class='quick-view']")]
        public IWebElement QuickView { get; set; }        

        public void ClickHoverView()
        {
          QuickView.Click();
        }

    }
}
