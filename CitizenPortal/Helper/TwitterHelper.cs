using CitizenPortal.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CitizenPortal.Helper
{
    public class TwitterHelper
    {
        public const string HashtagLink = "<span class='link'>#<a class='hashtag' href='https://twitter.com/search?q=%23{0}&src=hash'>{0}</a></span>";
        public const string SymbolLink = "<span class='link'>$<a class='symbol' href='https://twitter.com/search?q=%24{0}&src=ctag'>{0}</a></span>";
        public const string UrlLink = "<span class='link'><a href='{0}'>{1}</a></span>";
        public const string UserMentionLink = "<span class='link'>@<a href='https://twitter.com/{0}'>{0}</a></span>";

        private static List<FeedItem> _AllTweets = null;
        public static List<FeedItem> AllTweets
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigHelper.Config.TwitterAddress) || string.IsNullOrEmpty(ConfigHelper.Config.TwitterToken))
                {
                    _AllTweets = null;
                    return null;
                }

                if (_AllTweets != null)
                {
                    return _AllTweets;
                }

                List<FeedItem> allTweets = CacheHelper.Get(CacheHelper.Keys.AllTweets) as List<FeedItem>;
                if (allTweets != null)
                {
                    _AllTweets = allTweets;
                    return _AllTweets;
                }

                try
                {
                    WebRequest webRequest = HttpWebRequest.Create(ConfigHelper.Config.TwitterAddress + "&count=6");
                    webRequest.Method = "GET";
                    webRequest.Headers["Authorization"] = "Bearer " + ConfigHelper.Config.TwitterToken;

                    WebResponse webResponse = webRequest.GetResponse();
                    using (StreamReader responseStream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        allTweets = new List<FeedItem>();
                        foreach (JToken tweetObj in JArray.Parse(responseStream.ReadToEnd()))
                        {
                            FeedItem tweet = new FeedItem()
                            {
                                Text = tweetObj.SelectToken("text").ToString()
                            };

                            var entities = tweetObj["entities"];

                            // Transform hashtags
                            foreach (var hashtag in entities["hashtags"])
                            {
                                string hashtagLink = string.Format(TwitterHelper.HashtagLink, hashtag.SelectToken("text").ToString());
                                tweet.Text = tweet.Text.Replace("#" + hashtag["text"].ToString(), hashtagLink);
                            }

                            // Transform symbols
                            foreach (var symbol in entities["symbols"])
                            {
                                string symbolLink = string.Format(TwitterHelper.SymbolLink, symbol.SelectToken("text").ToString());
                                tweet.Text = tweet.Text.Replace("$" + symbol["text"].ToString(), symbolLink);
                            }

                            // Transform urls
                            foreach (var url in entities["urls"])
                            {
                                string urlLink = string.Format(TwitterHelper.UrlLink, url.SelectToken("url").ToString(), url.SelectToken("url").ToString());
                                tweet.Text = tweet.Text.Replace(url["url"].ToString(), urlLink);
                            }

                            // Transform user mentions
                            foreach (var userMention in entities["user_mentions"])
                            {
                                string userMentionLink = string.Format(TwitterHelper.UserMentionLink, userMention.SelectToken("screen_name").ToString());
                                tweet.Text = tweet.Text.Replace("@" + userMention["screen_name"].ToString(), userMentionLink);
                            }

                            allTweets.Add(tweet);
                        }
                    }

                    _AllTweets = allTweets;
                    CacheHelper.Put(CacheHelper.Keys.AllTweets, _AllTweets);

                    return _AllTweets;
                }
                catch (Exception)
                { }

                return null;
            }

            set
            {
                if (value == null)
                {
                    CacheHelper.Remove(CacheHelper.Keys.AllTweets);
                    _AllTweets = null;
                }
                else if (!value.Equals(_AllTweets))
                {
                    CacheHelper.Put(CacheHelper.Keys.AllTweets, value);
                    _AllTweets = value;
                }
            }
        }
    }
}