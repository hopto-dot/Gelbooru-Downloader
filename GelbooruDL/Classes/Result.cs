using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GelbooruDL;

namespace GelbooruDL.Classes
{
    public class Result
    {
        public string pageURL = "";
        public List<string> tags = new List<string>();
        public string thumImage = "";
        public string imageURL = "";

        public Result(string pageURL, List<string> tags, string thumImage)
        {
            this.pageURL = pageURL;
            this.tags = tags;
            this.thumImage = thumImage;
        }

        public string GetTagString()
        {
            return string.Join(", ", tags);
        }

        public string GetTagSummary()
        {
            int k = tags.Count;
            if (k > 10) { k = 10; }

            List<string> toReturn = new List<string>();
            for (int i = 0; i < k; i++)
            {
                toReturn.Add(tags[i]);
            }


            return string.Join(", ", toReturn);
        }

        public string GetImageExtension()
        {
            try
            {
                return imageURL.Split(".")[3];
            } catch
            {
                Program.printColour("Something unexpected happened when trying to get the image extension", ConsoleColor.Red);
                Program.printColour("Trying to carry on", ConsoleColor.Red);
                return "jpg";
            }
            
        }

        public bool downloadImageFromPage(bool fullSize = true)
        {
            WebClient client = new WebClient();
            string html = client.DownloadString(pageURL);

            string snipStart = string.Empty;
            int snipIndex = -1;

            snipIndex = html.IndexOf("<a href=\"https://img3.gelbooru.com/images/") + 9;
            html = html.Substring(snipIndex);

            snipIndex = html.IndexOf("\"");
            html = html.Substring(0, snipIndex);
            imageURL = html;

            string filename = GetTagSummary();
            string fileExtension = GetImageExtension();
            try
            {
                client.DownloadFile(imageURL, $"Downloads\\{filename}.{fileExtension}");
                string captionFilePath = $"Downloads\\{filename}.txt";
                File.CreateText(captionFilePath).Dispose();
                File.WriteAllText(captionFilePath, GetTagString());
            }
            catch (Exception ex)
            {
                Program.printColour($"Failed to download image: {imageURL}", ConsoleColor.Red);
                Program.printColour($"{ex.Message} ({ex.GetType()})", ConsoleColor.Red);
                if (ex.InnerException.Message != null)
                {
                    Program.printColour(ex.InnerException.Message, ConsoleColor.Red);
                }
                return false;
            }


            Program.printColour($"Saved image with tags '{GetTagSummary()}...'");
            return true;
        }
    }
}
