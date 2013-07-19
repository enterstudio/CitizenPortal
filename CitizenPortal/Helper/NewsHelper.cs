using CitizenPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace CitizenPortal.Helper
{
    public class NewsHelper
    {
        private static List<FeedItem> _AllNews = null;
        public static List<FeedItem> AllNews
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigHelper.Config.RssAddress))
                {
                    _AllNews = null;
                    return null;
                }

                if (_AllNews != null)
                {
                    return _AllNews;
                }

                List<FeedItem> allNews = CacheHelper.Get(CacheHelper.Keys.AllNews) as List<FeedItem>;
                if (allNews != null)
                {
                    _AllNews = allNews;
                    return _AllNews;
                }

                try
                {
                    using (XmlReader xmlReader = XmlReader.Create(ConfigHelper.Config.RssAddress))
                    {
                        allNews = new List<FeedItem>();
                        foreach (SyndicationItem newsObj in SyndicationFeed.Load(xmlReader).Items.ToList().Take(2))
                        {
                            FeedItem news = new FeedItem()
                            {
                                Title = (newsObj.Title.Text.Length > 55 ? newsObj.Title.Text.Substring(0, 55) + "..." : newsObj.Title.Text),
                                Text = (newsObj.Summary.Text.Length > 110 ? newsObj.Summary.Text.Substring(0, 110) + "..." : newsObj.Summary.Text),
                                Link = newsObj.Links[0].Uri.ToString()
                            };

                            allNews.Add(news);
                        }
                    }

                    _AllNews = allNews;
                    CacheHelper.Put(CacheHelper.Keys.AllNews, _AllNews);

                    return _AllNews;
                }
                catch (Exception)
                { }

                return null;
            }

            set
            {
                if (value == null)
                {
                    CacheHelper.Remove(CacheHelper.Keys.AllNews);
                    _AllNews = null;
                }
                else if (!value.Equals(_AllNews))
                {
                    CacheHelper.Put(CacheHelper.Keys.AllNews, value);
                    _AllNews = value;
                }
            }
        }
    }
}