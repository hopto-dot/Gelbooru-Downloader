using HtmlAgilityPack;
using System;
using System.Net;
using GelbooruDL.Classes;
using System.Drawing;
using System.Linq;

namespace GelbooruDL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (!Directory.Exists("Downloads"))
            {
                Directory.CreateDirectory("Downloads");
            }

            if (args.Length > 0) //if exe is called with arguments
            {
                string searchArgument = string.Join(" ", args);
                search(searchArgument);
                return;
            }
        start:
            Console.Write("Enter tags or url: ");
            string input = Console.ReadLine().Replace(", ", " ").Trim();
            Console.WriteLine();
            if (input.Contains("danbooru") || input.Contains("view&id") || input.Contains(".exe") || input.Contains(".js")) { printColour("That won't work!", ConsoleColor.Red); goto start; }

            search(input);
            printColour("Finished job", ConsoleColor.Blue);
            Console.WriteLine();
            goto start;
        }
        
        static void search(string searchString)
        {
            string url = !searchString.StartsWith("https://gelbooru.com/") ? $"https://gelbooru.com/index.php?page=post&s=list&tags={searchString}" : searchString;
            printColour($"Searching images from {url}", ConsoleColor.Blue);

            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            List<Result> results = getResults(html);
            printColour($"Found {results.Count} images", ConsoleColor.White);

            printColour($"Downloading images...", ConsoleColor.White);
            downloadSearchResults(results);

            Console.WriteLine("Done!");
        }

        static void downloadSearchResults(List<Result> results)
        {
            foreach (Result result in results)
            {
                result.downloadImageFromPage();
                Thread.Sleep(1000);
            }
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
                List<string> tagsList = resultChildren.Attributes["title"].Value.Split("  ")[0].Trim().Split(" ").ToList();
                string tags = string.Join(", ", tagsList);
                string pageURL = result.ChildNodes.ToList()[1].Attributes["href"].DeEntitizeValue;

                Result newResult = new Result(pageURL, tagsList, sauce);
                results.Add(newResult);
            }

            return results;
        }

        public static void printColour(string message, ConsoleColor colour = ConsoleColor.Green)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}