using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ShellProgressBar;

namespace WritingComExporter
{
    class Program
    {
        private static string loginName;
        private static string storyURL;
        private static string storyName;
        private static string storyIncipit;
        private static string pathStory;
        private static List<OutlineMap> storyMap = new List<OutlineMap>();
        
        static void writeMsg(string msg)
        {
            Console.WriteLine(msg);
        }

        static void writeCont(string msg)
        {
            Console.Write(msg);
        }

        static int temporaryBypass(string url, IWebDriver driver, ProgressBar progress = null)
        {
            int counter = 0;
            
            do
            {
                counter++;

                if ((counter % 15) == 0)
                    if (progress == null)
                        writeCont(Environment.NewLine +
                                  "(***) Still waiting for Interactive Stories to become available...");
                    else
                        progress.ForegroundColor = ConsoleColor.Yellow;

                driver.Navigate().GoToUrl(url);
            } while (driver.Title == "One Moment Please...");

            if (progress != null)
                progress.ForegroundColor = ConsoleColor.Green;

            return 1;
        }

        static ProgressBar createProgressBar(int total, string msg)
        {
            var pbOptions = new ProgressBarOptions
            {
                ProgressCharacter = '█',
                ProgressBarOnBottom = true
            };

            return new ProgressBar(total, msg, pbOptions);
        }

        static string cleanString(string text)
        {
            Regex regEx = new Regex(@"(\\n|\\r)+");
            return regEx.Replace(text, "<br/>");
        }
        
        static void initializeSelenium(IWebDriver driver)
        {
            writeCont(">>> Please login to Writing.com using your account...");
            
            driver.Navigate().GoToUrl(Urls.LOGINURL);
            driver.Manage().Window.Maximize();

            while (driver.Url != Urls.AFTERLOGINURL) { }
            
            loginName = driver.FindElement(By.XPath(XPath.AFTERLOGIN_USERNAME)).Text;
            driver.Manage().Window.Minimize();
            writeMsg(" Logged in as " + loginName + "!");
        }

        static void loadStory(IWebDriver driver)
        {
            writeCont(">>> Please specify the main page of your Interactive Story: ");
            storyURL = Console.ReadLine();
            
            writeCont(">>> Searching for your story...");
            driver.Navigate().GoToUrl(storyURL);

            if (driver.FindElement(By.XPath(XPath.MAINPAGE_STORY_BEGIN)).Size.IsEmpty)
            {
                writeMsg("[!] Your URL is invalid.");
                loadStory(driver);
            }

            try
            {
                storyName = driver.FindElement(By.XPath(XPath.MAINPAGE_STORY_NAME)).Text;
            }
            catch (Exception e)
            {
                storyName = driver.FindElement(By.XPath(XPath.MAINPAGE_STORY_NAME_2)).Text;
            }

            storyIncipit = driver.FindElement(By.XPath(XPath.MAINPAGE_STORY_INCIPIT)).Text;
            
            writeMsg(" founded! Exporting " + storyName);
        }

        static void loadOutline(IWebDriver driver)
        {
            writeCont(">>> Loading outline...");
            temporaryBypass(storyURL + "/action/outline", driver);
            
            writeMsg(" loaded! Creating a map of all the pages...");
            
            ReadOnlyCollection<IWebElement> storyMapTMP;
            storyMapTMP = driver.FindElements(By.XPath(XPath.STORY_OUTLINE_MAP));

            var outline_progress = createProgressBar(storyMapTMP.Count, "Generating map...");

            foreach (var t in storyMapTMP)
            {
                if (t.Text == "(END)")
                {
                    outline_progress.Tick();
                    continue;
                }
                
                outline_progress.Tick("Adding to map: " + t.Text);

                var originalURL = t.GetAttribute("href");
                var mapURL = originalURL.Split("/");
                storyMap.Add(new OutlineMap(t.Text, mapURL[8], t.GetAttribute("href")));
            }
            
            Console.WriteLine(Environment.NewLine);
        }

        static void initializeFolder()
        {
            writeMsg(">>> Creating story folder on your Desktop...");
            
            pathStory = Path.Combine
                (Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WritingComStories", storyName);
            
            Directory.CreateDirectory(pathStory + "\\map");

            using (var streamWriter = File.CreateText(pathStory + "\\index.html"))
            {
                streamWriter.WriteLine("<h1>" + storyName + "</h1>");
                streamWriter.WriteLine("<span style=\"white-space: pre-line\">" + storyIncipit + "</span>");
                streamWriter.WriteLine("<br/><br/>" +
                                       "<a href='./map/" + storyMap[0].map + ".html'>Begin Story</a> | " +
                                       "<a href='./outline.html'>Story Outline</a>");
            }
            
            using (var streamWriter = File.CreateText(pathStory + "\\outline.html"))
            {
                writeMsg(">>> Creating index...");
                streamWriter.WriteLine("<h1>" + storyName + "</h1>");
                streamWriter.WriteLine("<h2>Story outline</h2>");
                streamWriter.WriteLine("<ul>");
                
                foreach (var element in storyMap)
                {
                    streamWriter.WriteLine("<li> <a href='./map/" + element.map + ".html'>" + element.map + ". " + element.title + "</a></li>");
                }
                
                streamWriter.WriteLine("</ul>");
            }
            
            Console.WriteLine(Environment.NewLine);
        }

        static void exportStory(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> chapterSelection;
            writeCont(">>> Exporting your story... This may take a while..!");
            var export_progress = createProgressBar(storyMap.Count, "Exporting your story...");

            foreach (var element in storyMap)
            {
                export_progress.Tick("Exporting: " + element.title);
                temporaryBypass(element.originalUrl, driver, export_progress);

                using (var streamWriter = File.CreateText(pathStory + "\\map\\" + element.map + ".html"))
                {
                    string chapterContent = driver.FindElement(By.XPath(XPath.STORY_CHAPTER_CONTENT)).Text;
                    if (chapterContent.Length == 0) chapterContent = driver.FindElement(By.XPath(XPath.STORY_CHAPTER_CONTENT_2)).Text;
                    
                    chapterSelection = driver.FindElements(By.CssSelector("[id ^= cc]"));
                    
                    streamWriter.WriteLine("<h1>" + storyName + "</h1>");
                    streamWriter.WriteLine("<h2>" + element.map + ". " + element.title + "</h2>");
                    streamWriter.WriteLine("<span style=\"white-space: pre-line\">" + chapterContent + "</span>");

                    if (chapterSelection.Count > 0)
                    {
                        streamWriter.WriteLine("<h3>What's next?</h3>");
                        streamWriter.WriteLine("<ul>");
                        
                        foreach (var menu in chapterSelection)
                        {
                            var mapElement = menu.GetAttribute("href").Split('/');
                            var tmp = storyMap.FindIndex(x => x.map.Contains(mapElement[8]));
                            
                            if (tmp > -1)
                                streamWriter.WriteLine("<li><a href='./" + mapElement[8] + ".html'>" + menu.Text + "</a></li>");
                            else
                                streamWriter.WriteLine("<li>" + menu.Text + " <i>(Not available)</i></li>");
                        }
                        
                        streamWriter.WriteLine("</ul>");
                    }
                    
                    else
                    {
                        streamWriter.WriteLine("<b>You've reached the end of the story!</b>");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            IWebDriver chromeDriver = new ChromeDriver();
            writeMsg(Environment.NewLine + "--- WRITING.com INTERACTIVE EXPORTER TOOL ---");
            
            initializeSelenium(chromeDriver);
            loadStory(chromeDriver);
            loadOutline(chromeDriver);
            initializeFolder();
            exportStory(chromeDriver);
            
            writeMsg("[!!!] Export complete!");
        }
    }
}