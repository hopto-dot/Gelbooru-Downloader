using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GelbooruDL.Classes
{
    public class TagTemplate
    {
        public string templateName = ""; // this does nothing
        public List<TagGroup> template = new List<TagGroup>();
        public TagGroup blacklist = new TagGroup();
        public TagGroup containBlacklist = new TagGroup();
        
        public bool scrapeArtistTags = true;
        public bool scrapeCharacterTags = true;
        public bool scrapeCopyrightTags = false;

        public bool scrapeTagsOnly = false; // scrape tags only and don't download images (only create caption files)

        public int minimumTagUse = 100; // the minimum global frequency required for a tag to be included in captions
        public int minimumTagTotal = 23; // the minimum number of tags an image must have to be downloaded

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
            template.Add(new TagGroup("first group", "1girl, pov, sex, pronebone, 1boy, leg grab, ankle grab, restrained, excessive cum, deep penetration, loli"));
            // tags that contain these keywords will always be moved to the beginning of but captions but after the tags in the list above (because it is Add()'ed to `template` second)
            template.Add(new TagGroup("contains", "straddle, doggystyle, position, breast, cum, holding, another's, behind, grab, hug, ass, pussy, light, polka, striped, nude, testicles, white , black, aqua, red , blue , purple, pink, green, blonde, grey, brown, orange, gradient hair, yellow, medium, tail, bun, streaked, blur, depth of field"));
            // images with these tags will be downloaded but the tags won't appear in the captions. Maybe I should name this tag group "hide".
            // to stop tags from being searched at all use a hypen before a tag when running the program, for example "Enter tags or url: -1boy"
            containBlacklist = new TagGroup("blacklist", "absurd, artist, bad , twitter, pixiv, request, translation, shaped, pupils, commentary, relationship, netorare, fat mons, third-party edit, tagme, commission");
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
