using CitizenPortal.Helper;
using CitizenPortal.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CitizenPortal.Controllers
{
    public class _SharedController : Controller
    {
        //
        // GET: /_Shared/_Aside

        public ActionResult _Aside()
        {
            Aside aside = new Aside();

            // Twitter
            aside.Tweets = TwitterHelper.AllTweets;

            // Actu
            aside.News = NewsHelper.AllNews;

            // Last data
            if (DataHelper.AllDatasets != null)
            {
                aside.LastData =
                    (from d in DataHelper.AllDatasets
                     orderby d.ReleaseDate descending
                     select new FeedItem
                     {
                         Title = d.Name,
                         Link = string.Format("/Data/Show/{0}/{1}", d.Catalog.Alias, d.EntitySet),
                         Date = DateTime.Parse(d.ReleaseDate).ToString("dd/MM/yyyy")
                     }).Take(3).ToList();
            }

            return View(aside);
        }
    }
}
