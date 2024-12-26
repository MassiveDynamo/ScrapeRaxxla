using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {
        var postFolder = @"d:\git\ScrapeRaxxla\Data\Posts\Pages";
        if (!Directory.Exists(postFolder))
        {
            Directory.CreateDirectory(postFolder);
        }

        // Set up ChromeDriver
        using IWebDriver driver = new ChromeDriver();

        var pageNumber = GetLastPage(postFolder);
        while (true)
        {
            try {
                Console.WriteLine($"Navigation to page {pageNumber}");
                driver.Navigate().GoToUrl($"https://forums.frontier.co.uk/threads/the-quest-to-find-raxxla.168253/page-{pageNumber}");
                List<string[]> items = [];
                IReadOnlyCollection<IWebElement> post = driver.FindElements(By.TagName("article"));
                foreach (var item in post)
                {
                    var author = item.GetDomAttribute("data-author");
                    var id = item.GetDomAttribute("data-content");
                    if (String.IsNullOrEmpty(id))
                    {
                        continue;
                    }

                    var folder = new DirectoryInfo(Path.Combine(postFolder, pageNumber.ToString()));
                    if (!folder.Exists)
                    {
                        folder.Create();
                    }

                    var fileName = Path.Combine(folder.FullName, id + ".txt");
                    if (!File.Exists(fileName))
                    {
                        File.WriteAllText(fileName, item.Text);
                    }
                }

                pageNumber++;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                break;
            }
        }

        // Close the browser
        driver.Quit();
    }

    private static int GetLastPage(string PagesFolder)
    {
        var folders = Directory.GetDirectories(PagesFolder);

        // Return the last page number, i.e. last foldername
        if (folders == null)
            return 1;

        var lastPage = folders
         .Select(ExtractNumberFromText)
         .DefaultIfEmpty(0)
         .Max();

        return lastPage == 0 ? 1 : lastPage;
    }

    private static int ExtractNumberFromText(string value)
    {
        var pageNumberPart = new DirectoryInfo(value);
        return int.Parse(pageNumberPart.Name);
    }
}