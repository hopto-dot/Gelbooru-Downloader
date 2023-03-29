using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GelbooruDL.Classes
{
    public class TagTemplate
    {
        public string templateName = ""; //this does nothing
        public List<TagGroup> template = new List<TagGroup>();
        public TagGroup blacklist = new TagGroup();
        public TagGroup containBlacklist = new TagGroup();
        
        public bool scrapeArtistTags = false;
        public bool scrapeCharacterTags = false;
        public bool scrapeCopyrightTags = false;

        public bool scrapeTagsOnly = false; // scrape tags only (create caption files) and don't download images

        public int minimumTagUse = 200; // the minimum global frequency for a tag to be used in a caption
        public int minimumTagTotal = 27; // the minimum number of tags an image must have to be downloaded

        public void addGroup(TagGroup Group)
        {
            template.Add(Group);
        }
        
        public TagTemplate(string templateName, int minimumTagUse = 100)
        {
            this.templateName = templateName;
            this.minimumTagUse = minimumTagUse;
        }

        public TagTemplate(int minimumTagUse = 100)
        {
            // this doesn't do anything at the moment
            templateName = "Default";
            // these tags will always be moved to the beginning of captions (because it is Add()'ed to `template` first)
            template.Add(new TagGroup("first group", "1girl, 1boy"));
            // tags that contain these keywords will always be moved to the beginning of but captions but after the tags in the list above (because it is Add()'ed to `template` second)
            template.Add(new TagGroup("contains", "polka, striped, white , black, aqua, red , blue , purple, pink, green, blonde, grey, brown, orange, gradient hair, yellow"));
            // images with these tags will be downloaded but the tags won't appear in the captions
            // to stop tags from being searched at all use a hypen before a tag when running the program. For example "Enter tags or url: -1boy"
            containBlacklist = new TagGroup("blacklist", "blur, photo, depth of field, absurd, highres, lowres, artist, bad , twitter, pixiv, request, translation, shaped, pupils, commentary, multiple boys, multiple girls, relationship, netorare, fat mons, third-party edit, tagme, commission");
        }
    }

    public class TagGroup
    {
        public string groupName = ""; // if named "contains", tags that contains the listed keywords will be picked up. "blacklist" is another special name
        public List<string> tags = new List<string>();

        public TagGroup(string GroupName, string TagString)
        {
            groupName = GroupName;

            tags = TagString.Replace(", ", ",").Replace("_", "").Split(",").ToList();
        }

        public TagGroup()
        {

        }
    }
}
