using System.Collections.Generic;

namespace CitizenPortal.Models
{
    public class FeedItem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
    }

    public class Aside
    {
        public List<FeedItem> Tweets { get; set; }
        public List<FeedItem> News { get; set; }
        public List<FeedItem> LastData { get; set; }
    }
}
