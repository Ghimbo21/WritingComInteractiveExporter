# Writing.com Interactive Exporter
A tool to export Interactive stories from Writing.com, made in C# with .net Core 5.

# How it works
This tool use Selenium (Chrome Web Driver) to download the pages and export all the Interactive Story inside a folder on your Desktop. You only need to specify your login information (via the official website) and the Interactive Story you want to export. Then you can grab a tea and wait for it to finish!

# What about the cooldown?
Writing.com have a cooldown for all the free members that doesn't let you to read all the story at once. This tool reload the page everytime the error shows up, so it may take a long time if you don't have a paid membership, expecially with long interactive stories.
