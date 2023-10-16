using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GelbooruDL;
using HtmlAgilityPack;

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

        public Result(string pageURL)
        {
            this.pageURL = pageURL;
        }

        public static List<string> orderTags(List<string> toReorderTags, TagTemplate tagTemplate = null)
        {
            List<List<string>> outputOrder = new List<List<string>>();
            foreach (TagGroup group in tagTemplate.template)
            {
                List<string> newOutputList = new List<string>();
                foreach (string templateTag in group.tags)
                {
                    foreach (string rawTag in toReorderTags)
                    {
                        if (rawTag.Replace("_", " ") == templateTag && group.groupName != "contains")
                        {
                            newOutputList.Add(templateTag.Replace("_", " "));
                        }
                        else if (rawTag.Replace("_", " ").Contains(templateTag) && group.groupName == "contains")
                        {
                            newOutputList.Add(rawTag);
                        }
                    }
                }
                outputOrder.Add(newOutputList);
                foreach (string addedTag in newOutputList)
                {
                    toReorderTags.Remove(addedTag.Replace(" ", "_"));
                }
            }
            
            List<string> otherTags = new List<string>();
            foreach (string tag in toReorderTags)
            {
                otherTags.Add(tag.Replace("_", " "));
            }
            outputOrder.Add(otherTags);

            List<string> conjoinedOutputList = new List<string>();
            foreach (List<string> outputGroup in outputOrder)
            {
                conjoinedOutputList.AddRange(outputGroup);
            }
            
            foreach (string outputTag in conjoinedOutputList)
            {
                foreach (string containBLtag in tagTemplate.containBlacklist.tags)
                {
                    if (containBLtag.Trim() == "") { continue; }
                    if (outputTag.Contains(containBLtag))
                    {
                        tagTemplate.blacklist.tags.Add(outputTag);
                    }
                }
            }

            foreach (string removetag in tagTemplate.blacklist.tags)
            {
                conjoinedOutputList.Remove(removetag);
            }

            //string outputString = string.Join(", ", conjoinedOutputList).Replace("_", " ").Trim();
            return conjoinedOutputList;
        }

        public string GetTagSummary(TagTemplate tagTemplate)
        {
            int k = tags.Count;
            if (k > 13) { k = 13; }
            
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
                //return imageURL.Split(".")[3];
                string tempString = "";
                return imageURL.Substring(imageURL.LastIndexOf(".") + 1);
            } catch
            {
                Program.printColour("Something unexpected happened when trying to get the image extension", ConsoleColor.Red);
                Program.printColour("Trying to carry on", ConsoleColor.Red);
                return "jpg";
            }
            
        }
        public string GetTagString()
        {
            List<string> returnTags = new List<string>();
            foreach (string tag in tags)
            {
                returnTags.Add(tag.Replace("_", " "));
            }

            return string.Join(", ", returnTags);
        }

        public bool downloadImageFromPage(TagTemplate tagTemplate, bool fullSize = true)
        {
            string filename = tags.Count > 0 ? removeForbiddenChars(GetTagSummary(tagTemplate)) : "";
            string captionFilePath = $"Downloads\\{filename}.txt";

            if (!tagTemplate.scrapeTagsOnly)
            {
                WebClient client = new WebClient();
                string html = client.DownloadString(pageURL);
                tags = scrapeTagsFrompage(html, tagTemplate, tagTemplate.minimumTagUse);

                #region snip image
                string snipStart = string.Empty;
                int snipIndex = -1;

                snipIndex = html.IndexOf("<a href=\"https://img3.gelbooru.com/images/") + 9;
                html = html.Substring(snipIndex);

                snipIndex = html.IndexOf("\"");
                html = html.Substring(0, snipIndex);
                imageURL = html;
                #endregion

                string subfolder = "";
                captionFilePath = $"Downloads\\{subfolder}{filename}.txt";

                string imageExtension = GetImageExtension();

                if (File.Exists($"Downloads\\{subfolder}{filename}.{imageExtension}"))
                {
                    Program.printColour("File already exists", ConsoleColor.Green);
                    return false;
                }


                if (filename == "")
                {
                    filename = removeForbiddenChars(GetTagSummary(tagTemplate));
                    captionFilePath = $"Downloads\\{filename}.txt";
                }

                try
                {
                    client.DownloadFile(imageURL, $"Downloads\\{subfolder}{filename}.{imageExtension}");
                }
                catch (Exception ex)
                {
                    Program.printColour($"Failed to download image: {imageURL}", ConsoleColor.Red);
                    Program.printColour($"{ex.Message} ({ex.GetType()})", ConsoleColor.Red);
                    return false;
                }
            }

            File.CreateText(captionFilePath).Dispose();
            File.WriteAllText(captionFilePath, GetTagString());

            Program.printColour($"Saved image with tags '{GetTagSummary(tagTemplate)}...'");
            return true;
        }

        public List<string> scrapeTagsFrompage(string html, TagTemplate tagTemplate, int minimumTagUses = 1)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tagsNode = htmlDoc.DocumentNode.SelectNodes(@"/html/body/div[1]/section").ToList()[0].ChildNodes.ToList()[1];

            List<string> imageTags = new List<string>();
            foreach (var node in tagsNode.ChildNodes)
            {
                if (node.Name != "li") { continue; }
                if (node.Attributes.ToList().Count != 1) { continue; } 
                if (node.Attributes["class"] == null) { continue; }
                bool scrapeTag = false;
                if (tagTemplate.scrapeArtistTags && node.Attributes["class"].Value == "tag-type-artist") { scrapeTag = true; }
                else if (tagTemplate.scrapeCharacterTags && node.Attributes["class"].Value == "tag-type-character") { scrapeTag = true; }
                else if (tagTemplate.scrapeCopyrightTags && node.Attributes["class"].Value == "tag-type-copyright") { scrapeTag = true; }
                else if (node.Attributes["class"].Value == "tag-type-general") { scrapeTag = true; }

                if (scrapeTag)
                {
                    int tagUses = int.Parse(node.ChildNodes.ToList()[3].InnerText.Trim());
                    string tag = node.ChildNodes.ToList()[1].InnerText.Trim();
                    tag = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(tag.Trim())).Replace(" ", "_");

                    if (tagUses >= minimumTagUses)
                    {
                        imageTags.Add(tag);
                    }
                }
            }
            imageTags = orderTags(imageTags, tagTemplate);
            return imageTags;
        }

        public string removeForbiddenChars(string input)
        {
            return input.Replace("(", "").Replace(")", "").Replace(":", "").Replace("\"", "").Replace("/", "").Replace("\\", "").Replace("|", "").Replace("?", "").Replace("*", "").Replace(">", "").Replace("<", "");
        }
    }
}
