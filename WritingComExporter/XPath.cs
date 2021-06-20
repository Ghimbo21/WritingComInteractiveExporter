namespace WritingComExporter
{
    public class XPath
    {
        public const string AFTERLOGIN_USERNAME =
            "/html/body/div[15]/div[1]/div/div[3]/div/div[6]/div[3]/div[2]/div[2]/div[1]/div/table/tbody/tr[1]/td[2]/b";

        public const string MAINPAGE_STORY_BEGIN =
            "//*[@id=\"Content_Column_Inside\"]/div[6]/div[3]/div[2]/div/a";
        
        public const string MAINPAGE_STORY_NAME =
            "//*[@id=\"Content_Column_Inside\"]/div[4]/table/tbody/tr/td[2]/div[2]/h1/a";

        public const string MAINPAGE_STORY_NAME_2 =
            "//*[@id=\"Content_Column_Inside\"]/div[4]/table/tbody/tr/td[2]/div[3]/h1/a";
        
        public const string MAINPAGE_STORY_INCIPIT =
            "//*[@id=\"Content_Column_Inside\"]/div[6]/div[2]/table/tbody/tr/td";

        public const string STORY_OUTLINE_MAP =
            "//*[@id=\"Content_Column_Inside\"]/div[6]/div[2]/pre/b//*";

        public const string STORY_CHAPTER_CONTENT =
            "//*[@id=\"Content_Column_Inside\"]/div[5]/table/tbody/tr/td[1]/div/table/tbody/tr[1]/td/table/tbody/tr/td[1]/div[2]/div[2]/div";
        
        public const string STORY_CHAPTER_CONTENT_2 =
            "//*[@id=\"Content_Column_Inside\"]/div[5]/table/tbody/tr/td[1]/div/table/tbody/tr[1]/td/table/tbody/tr/td[1]/div/div[3]";

        public const string STORY_CHAPTER_CHOICES =
            "//*[@id=\"Content_Column_Inside\"]/div[5]/table/tbody/tr/td[1]/div/table/tbody/tr[1]/td/table/tbody/tr/td[1]/div/div[4]/table/tbody/tr/td/div/div[1]/p//*[not(contains(., '*'))]";
               //*[@id="Content_Column_Inside"]/div[5]/table/tbody/tr/td[1]/div/table/tbody/tr[1]/td/table/tbody/tr/td[1]/div[2]/div[6]/table/tbody/tr/td/div[1]/div[1]/p[1]
    }
}