using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace ToolsQA
{
    class GetVWTouranPerFaceliftQtyBilbasenDK
    {
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

            IWebElement weMark =
                driver.FindElement(By.XPath(@"//*[@id=""inline-search-collapse""]/form/div[1]/div[1]/div[2]/div/select"));
            var seMark = new SelectElement(weMark);

            IList<IWebElement> seoMark = seMark.Options;

            for (int i = 1; i < seoMark.Count; i++) // Skip the first entry in the mark list on purpose
            {
                var markText = seoMark[i].Text.Trim();
                if (markText == "VW")
                {
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
                        var modelText = seoModel[j].Text.Trim();
                        if (modelText == "Touran")
                        {
                            seModel.SelectByIndex(j);
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

                            List<Tuple<string, string>> yearFromTo = new List<Tuple<string, string>>
                                { new Tuple<string, string>("2003", "2006"),  //  2003 - 2006 - VW Touran I
                                  new Tuple<string, string>("2007", "2010"),  //  2007 - 2010 - VW Touran I facelift I
                                  new Tuple<string, string>("2011", "2015"),  //  2011 - 2015 - VW Touran I facelift II
                                  new Tuple<string, string>("2016", "2017")}; //  2016 - 2017 - VW Touran II 

                            foreach (Tuple<string, string> tuple in yearFromTo)
                            {
                                var weYearFrom = driver.FindElement(By.XPath(@"//*[@id=""inline-search-collapse""]/form/div[1]/div[2]/div[3]/div/div[1]/select"));
                                var seYearFrom = new SelectElement(weYearFrom);
                                seYearFrom.SelectByValue(tuple.Item1);
                                Thread.Sleep(3000);

                                var weYearTo = driver.FindElement(By.XPath(@"//*[@id=""inline-search-collapse""]/ form/div[1]/div[2]/div[3]/div/div[2]/select"));
                                var seYearTo = new SelectElement(weYearTo);
                                seYearTo.SelectByValue(tuple.Item2);
                                Thread.Sleep(3000);

                                button = driver.FindElement(By.Id("usedcaradjustsubmit"));
                                text = button.Text;
                                numberOfOfferedCars = new String(text.Where(Char.IsDigit).ToArray());

                                result = markText.Trim() + ";" + modelText.Trim() + ";" + tuple.Item1 + "-" + tuple.Item2 + ";" + numberOfOfferedCars + ";";

                                Console.WriteLine(result);

                                using (StreamWriter sw = File.AppendText(outputFileMark))
                                {
                                    sw.WriteLine(result);
                                }
                            }
                            
                            break; // Prevents looping again and again for the same info. Some models can be represented more than once in the dropdown
                        }
                    }

                    break;
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
