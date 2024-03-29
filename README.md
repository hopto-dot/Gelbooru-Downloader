## GelbooruDL

GelbooruDL is a tool that will download images from Gelbooru then create text files with the same name containing all the tags of that image.

#### ⚠　For now, advanced scraping customisability is only available by changing variables within the code ⚠

#### To download images:
* Enter the raw tags
* You may specify the url of a gelbooru results page
* Add `pages:[start page]-[end page]` to scrape from multiple pages. For example `pages:1-10`

Downloaded images will appear in a folder called `Downloads`. This is in the same folder as the executable.

#### When specifying raw tags
* Separate tags with a space
* Use underscores for spaces within the tag, the same as if you're searching on any booru website (for example `black_hair`)
* You may blacklist images with tags putting prepending tags with "-". For example; "-messy_hair" (this works on the website too)

## Planned functionality
- [X] Pages to download: Define how many pages to download
- [X] Page number: Define which page to download
- [X] Templates: define tags you want automatically being put at the beginning of captions
- [X] Ghost tags: define tags you don't mind being in images but not in the caption that gets saved
- [ ] Easy scraping customisability: currently, this customisability is only accessible by editing variables in the code.
- [ ] Pause time: define how long you want to pause between each image download (default: 0.5 seconds, to reduce risk of being ip banned)
