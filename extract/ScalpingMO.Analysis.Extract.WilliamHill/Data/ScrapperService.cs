using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using MongoDB.Bson.Serialization.IdGenerators;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScalpingMO.Analysis.Extract.WilliamHill.Models;
using System;
using System.Globalization;

namespace ScalpingMO.Analysis.Extract.WilliamHill.Data
{
    public class ScrapperService
    {
        private IWebDriver _driver;
        private string _url;

        public ScrapperService(string url)
        {
            _url = url;

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");  // Para rodar no Docker sem interface gráfica
            chromeOptions.AddArgument("--no-sandbox");  // Recomendado para Docker
            chromeOptions.AddArgument("--disable-dev-shm-usage");  // Evita problemas com espaço limitado

            chromeOptions.BinaryLocation = Environment.GetEnvironmentVariable("CHROME_BIN");

            _driver = new ChromeDriver(chromeOptions);
            _driver.Navigate().GoToUrl(_url);
            Thread.Sleep(2000);
        }

        public List<Fixture> GetFixtures()
        {
            List<string> days = GetListDays();
            List<Fixture> fixtures = new List<Fixture>();

            for (int i = 0; i < days.Count; i++)
            {
                DateTime date = DateTime.Now.AddDays(i);
                string dayLink= days[i];

                List<Fixture> fixturesByDay = GetFixtureByDay(dayLink, date);
                fixtures.AddRange(fixturesByDay);
            }

            _driver.Quit();
            return fixtures;
        }

        #region Private methods

        private List<string> GetListDays()
        {
            List<IWebElement> listDays = _driver.FindElements(By.XPath("//li[@class=\"css-xnumgp\"]/div/a")).ToList();
            List<string> days = new List<string>();

            foreach (IWebElement element in listDays)
            {
                string dayString = element.GetAttribute("href");

                if (dayString.Contains("matches/competition") && !dayString.Contains("future"))
                    days.Add(dayString);
            }

            return days;
        }

        private List<Fixture> GetFixtureByDay(string dayUrl, DateTime date)
        {
            List<Fixture> fixtures = new List<Fixture>();

            _driver.Navigate().GoToUrl(dayUrl);
            Thread.Sleep(2000);

            List<IWebElement> matchLinks = GetAllElementsInDay("//div[@class=\"sp-o-market__title\"]/a");
            List<IWebElement> matchDates = GetAllElementsInDay("//span[@class=\"sp-o-market__clock__time\"]");

            for (int i = 0;i < matchLinks.Count && i < matchDates.Count; i++)
            {
                IWebElement linkElement = matchLinks[i];
                IWebElement dateElement = matchDates[i];

                string hour = dateElement.GetAttribute("innerHTML");
                string[] linkArray = linkElement.GetAttribute("href").Split("/");
                string matchName = linkArray[linkArray.Length - 1];

                int id = Convert.ToInt32(linkArray[linkArray.Length - 2].Replace("OB_EV", ""));
                string homeTeamName = ToTitleCase(matchName.Split("-vs-")[0]);
                string awayTeamName = ToTitleCase(matchName.Split("-vs-")[1]);

                Fixture fixture = new Fixture(id, homeTeamName, awayTeamName, date, hour);
                fixtures.Add(fixture);
            }

            return fixtures;
        }

        private List<IWebElement> GetAllElementsInDay(string xpath)
        {
            List<IWebElement> links = new List<IWebElement>();
            int previousCount = 0;

            while (true)
            {
                //List<IWebElement> currentLinks = _driver.FindElements(By.XPath()).ToList();
                List<IWebElement> currentLinks = _driver.FindElements(By.XPath(xpath)).ToList();
                links.AddRange(currentLinks.Except(links)); 

                if (links.Count == previousCount)
                    break;

                previousCount = links.Count;

                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                System.Threading.Thread.Sleep(3000); 
            }

            return links;
        }

        private string ToTitleCase(string input)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titleCase = textInfo.ToTitleCase(input.Replace("-", " ").ToLower());

            string[] titleSplitted = titleCase.Split(' ');

            for (int i = 0; i < titleSplitted.Length; i++)
            {
                if (titleSplitted[i].Length <= 2)
                    titleSplitted[i] = titleSplitted[i].ToUpper();
            }

            return string.Join(' ', titleSplitted);
        }

        #endregion
    }
}
