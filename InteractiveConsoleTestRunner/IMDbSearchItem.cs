namespace InteractiveConsoleTestRunner
{
    public class IMDbSearchItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string IMDbId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}