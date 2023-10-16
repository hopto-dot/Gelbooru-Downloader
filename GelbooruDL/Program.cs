using HtmlAgilityPack;
using System;
using System.Net;
using GelbooruDL.Classes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace GelbooruDL
{
    internal class Program
    {
        static TagTemplate tagTemplate = new TagTemplate();
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (!Directory.Exists("Downloads"))
            {
                Directory.CreateDirectory("Downloads");
            }

            if (args.Length > 0) // if exe is called with arguments
            {
                string searchArgument = string.Join(" ", args);
                search(searchArgument);
                return;
            }
start: // this is called after scaping or failure to provide correct arguments
            Console.Write("Enter tags or url: ");
            string input = Console.ReadLine().Replace(", ", " ").Trim() + " -animated";
            Console.WriteLine();

            if (checkIfSoloImageURL(input))
            {
                getSingleImageResult(input);
                goto start;
            }

            if (input.Contains("danbooru") || input.Contains("view&id") || input.Contains(".exe") || input.Contains(".js")) { printColour("That won't work!", ConsoleColor.Red); goto start; }

            if (input.ToLower().Contains("pages:")) {
                string tempString = input;
                int snipIndex = -1;
                snipIndex = input.IndexOf("pages:") + 6;
                tempString = tempString.Substring(snipIndex);
                tempString = tempString.Split(" ")[0].Trim();
                int startPage = 1;
                int endPage = 1;
                if (!tempString.Contains("-"))
                {
                    //scraping from pages 1 to X, finding out what X is
                    startPage = int.Parse(tempString);
                    endPage = startPage;
                    input = input.Replace($"pages:{tempString} ", "").Trim();
                }
                else
                {
                    //scraping from pages X to Y (in format X-Y), finding out what X and Y are
                    string[] components = tempString.Split("-");
                    input = input.Replace($"pages:{tempString} ", "").Trim();

                    try
                    {
                        startPage = int.Parse(components[0]);
                        endPage = int.Parse(components[1]);
                    }
                    catch
                    {
                        printColour("Pages argument should be in the format 'pages:X-Y'");
                        Console.WriteLine();
                        goto start;
                    }
                }
                Console.WriteLine($"Scraping from pages {startPage}-{endPage}");
                search(input, startPage, endPage);
                goto start;
            }

            search(input);
            printColour("Finished job", ConsoleColor.Blue);
            Console.WriteLine();
            goto start;
        }
        
        static void search(string searchString, int startPage = 1, int endPage = 1)
        {
            for (int p = startPage; p <= endPage; p++)
            {
                int pageID = (p - 1) * 42;

                string url = !searchString.StartsWith("https://gelbooru.com/") ? $"https://gelbooru.com/index.php?page=post&s=list&tags={searchString}&pid={pageID}" : searchString;
                printColour($"Searching page {p}", ConsoleColor.Blue);
                printColour($"Page url: {url}", ConsoleColor.Blue);

                var client = new HttpClient();
                var request = new HttpRequestMessage(new HttpMethod("GET"), url);
                request.Headers.TryAddWithoutValidation("cookie", "fringeBenefits=yup"); // yup
                var response = client.SendAsync(request).Result;
                var html = response.Content.ReadAsStringAsync().Result;

                List<Result> results = getResults(html);
                printColour($"Found {results.Count} images", ConsoleColor.White);

                printColour($"Downloading images...", ConsoleColor.White);
                downloadSearchResults(results);
            }
        }

        static void downloadSearchResults(List<Result> results)
        {
            foreach (Result result in results)
            {
                result.downloadImageFromPage(tagTemplate);
                if (!tagTemplate.scrapeTagsOnly)
                {
                    Thread.Sleep(500);
                }
            }
        }

        static Result getSingleImageResult (string url)
        {

            return null;
        }
        
        static List<Result> getResults(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var searchResults = htmlDoc.DocumentNode.SelectNodes(@"/html/body/div[1]/main/div[7]").ToList()[0];

            List<Result> results = new List<Result>();
            foreach (var result in searchResults.ChildNodes)
            {
                if (result.Name != "article") { continue; }
                var resultChildren = result.ChildNodes.ToList()[1].ChildNodes.ToList()[1];
                /*
                resultChildren attributes:
                    * src (thumnail image)
                    * title (tags)
                    * alt (alt string containing tags)
                    * class (contains nothing)
                */

                string sauce = resultChildren.Attributes["src"].Value; //mhmm saucy
                List<string> tagsList = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(resultChildren.Attributes["title"].Value)).Split("  ")[0].Trim().Split(" ").ToList();
                tagsList = Result.orderTags(tagsList, tagTemplate);
                string tags = string.Join(", ", tagsList);
                string pageURL = result.ChildNodes.ToList()[1].Attributes["href"].DeEntitizeValue;

                Result newResult = new Result(pageURL, tagsList, sauce);
                if (tags.Count() >= tagTemplate.minimumTagTotal)
                {
                    results.Add(newResult);
                }
                else
                {
                    continue;
                }
            }

            return results;
        }

        public static void printColour(string message, ConsoleColor colour = ConsoleColor.Green)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static bool checkIfSoloImageURL (string userInput)
        {
            return userInput.StartsWith("https://gelbooru.com/index.php?page=post&s=view&id=") && userInput.Length > 56;
        }
    }
}