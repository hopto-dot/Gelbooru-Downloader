## GelbooruDL

GelbooruDL is a tool that will download images from Gelbooru then create text files with the same name containing all the tags of that image.

<img src="https://github.com/hopto-dot/Gelbooru-Downloader/blob/master/GBDL.png?raw=true" width="700">

#### ⚠　For now, advanced scraping customisability is only available by changing variables within the code ⚠

#### To download images:
* Enter the raw tags
* You may specify the url of a gelbooru results page
* Add `pages:[start page]-[end page]` to scrape from multiple pages

Downloaded images will appear in a folder called `Downloads`, this is in the same folder as the exe.

#### When specifying raw tags
* Separate tags with a space
* Use underscores for spaces within the tag, the same as if you're searching on any booru website
* You may blacklist images with tags putting prepending tags with "-". For example; "-messy_hair" (this works on the website too)

## Planned functionality
- [X] Pages to download: Define how many pages to download
- [X] Page number: Define which page to download
- [X] Templates: define tags you want automatically being put at the beginning of captions
- [X] Ghost tags: define tags you don't mind being in images but not in the caption that gets saved
- [ ] Pause time: define how long you want to pause between each image download (default: 1 second, to reduce risk of being ip banned)
