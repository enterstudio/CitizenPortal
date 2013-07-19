using System;
using System.Web;
using System.Web.Caching;

namespace CitizenPortal.Helper
{
    public class CacheHelper
    {
        private static string Prefix = "CitizenPortal_";

        public class Keys
        {
            public static string Config = "Config";
            public static string AllDatasets = "AllDatasets";
            public static string AllRates = "AllRates";
            public static string AllTweets = "AllTweets";
            public static string AllNews = "AllNews";
        }

        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(Prefix + key);
        }

        public static void Put(string key, object value)
        {
            HttpRuntime.Cache.Insert(
                Prefix + key,
                value,
                null,
                DateTime.Now.AddMinutes(60),
                Cache.NoSlidingExpiration,
                (key == Keys.AllDatasets ? CacheItemPriority.High : CacheItemPriority.Default),
                null);
        }

        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(Prefix + key);
        }

        public static void ClearAll()
        {
            ConfigHelper.Config = null;
            DataHelper.AllDatasets = null;
            DataHelper.AllRates = null;
            NewsHelper.AllNews = null;
            TwitterHelper.AllTweets = null;
        }
    }
}
