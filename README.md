## GelbooruDL

GelbooruDL is a tool that will download images from Gelbooru then create text files with the same name containing all the tags of that image.

#### To download images:
* Enter the raw tags
* You may specify the url of a gelbooru results page

Downloaded images will appear in a folder called `Downloads`, this is in the same folder as the exe.

#### When specifying raw tags
* Separate tags with a space
* Use underscores for spaces within the tag, the same as if you're searching on any booru website
* You may blacklist images with tags putting prepending tags with "-". For example; "-messy_hair" (this works on the website too)

## Planned functionality
- [ ] Pages to download: Define how many pages to download
- [ ] Page number: Define which page to download
- [ ] Templates: define tags you want automatically being put at the beginning of captions
- [ ] Ghost tags: define tags you don't mind being in images but not in the caption that gets saved
- [ ] Pause time: define how long you want to pause between each image download (default: 1 second, to reduce risk of being ip banned)
