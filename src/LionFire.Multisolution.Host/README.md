## Nuget Upgrader tool

I wrote my own Nuget fixer since it was a huge time waster to run into VS2022's Package Manager refusing to update things. 

I just wanted to force 500+ projects to upgrade by manually editing multiple Directory.packages files and let the chips fall where they may. 
Usually everything is fine.

## Status

I haven't prepped this for public consumption, so YMMV.

This has some bugs:
- some problems around thinking downgrading to preview releases is still an upgrade

But it saves me a lot of time.

## How it works

### Config

1. Copy `document.sample.json` somewhere on your disk, and edit it to your liking.
2. Edit appsettings.json to point to this file in Documents:MostRecent[]

### Upgrading your Nuget packages
Start it, go to the Web UI, click the right arrow buttons to upgrade.  Done. 

No fuss. No checking whether the stars are aligned enough for you to go through with the upgrade.
