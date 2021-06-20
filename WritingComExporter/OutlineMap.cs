namespace WritingComExporter
{
    public class OutlineMap
    {
        public OutlineMap(string title, string map, string originalUrl)
        {
            this.title = title;
            this.map = map;
            this.originalUrl = originalUrl;
        }

        public string title { get; set; }
        public string map { get; set; }
        public string originalUrl { get; set; }
    }
}