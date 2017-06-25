using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections;
using System.Threading;

namespace ToolsQA
{
    /// <summary>
    /// Simple class to fetch the offered quantities of cars for every mark and model on www.bilbasen.dk
    /// </summary>
    class GetMarkModelQtyBilbasenDk
    {
        private static List<string> desiredMarks = new List<string>
        {
            "Audi",
            "BMW",
            //"Citroën",
            "Ford",
            //"Mercedes",
            //"Opel",
            //"Peugeot",
            "Toyota",
            //"Volvo",
            "VW",
            //"Alfa Romeo",
            //"Dacia",
            //"Fiat",
            //"Honda",
            //"Hyundai",
            //"Kia",
            //"Mazda",
            //"Nissan",
            //"Suzuki",
            //"Mini",
            //"Renault",
            //"Seat",
            //"Subaru",
            //"Saab",
            //"Opel",
            //"Tesla"
        };
        
        // 1. Add "geckodriver.exe" to the program execution folder 
        // 2. Download Firefox and point to it in the code below
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            string startRecordTime = start.ToString("yyyy-MM-dd_HHmmss");
            
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), $"output_{startRecordTime}.csv");
            using (StreamWriter sw = File.CreateText(outputFile))
            {
                sw.WriteLine(startRecordTime);
            }
           
            IWebDriver driver = new FirefoxDriver(new FirefoxOptions()
            {
                BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe"
            });
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Url = "http://www.bilbasen.dk";

            IWebElement weMark = driver.FindElement(By.XPath(@"//*[@id=""inline-search-collapse""]/form/div[1]/div[1]/div[2]/div/select"));
            var seMark = new SelectElement(weMark);
            
            IList<IWebElement> seoMark = seMark.Options;

            for (int i = 1; i < seoMark.Count; i++) // Skip the first entry in the mark list on purpose
            {
                var markText = seoMark[i].Text;

                #region Only scrape brands from the list (Comment out region if not)
                if (!desiredMarks.Contains(markText))
                {
                    continue;
                }
                desiredMarks.Remove(markText);
                #endregion 

                seMark.SelectByText(markText);
                Thread.Sleep(3000);

                string outputFileMark = Path.Combine(Directory.GetCurrentDirectory(), $"output_{markText}_{startRecordTime}.csv");
                if (!File.Exists(outputFileMark))
                {
                    using (StreamWriter sw = File.CreateText(outputFile))
                    {
                        sw.WriteLine(startRecordTime);
                    }
                } 

                IWebElement weModel = driver.FindElement(By.XPath(@"//*[@id=""inline-search-collapse""]/form/div[1]/div[1]/div[3]/div/select"));
                var seModel = new SelectElement(weModel);
                
                IList<IWebElement> seoModel = seModel.Options;

                for (int j = 1; j < seoModel.Count; j++) // Skip the first entry in the model list on purpose
                {
                    var modelText = seoModel[j].Text;

                    seModel.SelectByText(modelText);
                    Thread.Sleep(3000);

                    IWebElement button = driver.FindElement(By.Id("usedcaradjustsubmit"));
                    string text = button.Text;
                    string numberOfOfferedCars = new String(text.Where(Char.IsDigit).ToArray());
                    
                    string result = markText.Trim() + ";" + modelText.Trim() + ";" + numberOfOfferedCars + ";";

                    Console.WriteLine(result);

                    using (StreamWriter sw = File.AppendText(outputFileMark))
                    {
                        sw.WriteLine(result);
                    }
                }
            }
            
            driver.Close();
            driver.Quit();

            DateTime end = DateTime.Now;
            string endRecordTime = start.ToString("yyyy-MM-dd_HHmmss");

            TimeSpan elapseTime = end - start;
            string elapseTimeRecord = $"{elapseTime:hh\\:mm\\:ss}";

            Console.WriteLine(elapseTimeRecord);

            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine(endRecordTime);
                sw.WriteLine(elapseTimeRecord);
            }
        }
    }
}
