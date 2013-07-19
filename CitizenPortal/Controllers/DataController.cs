using CitizenPortal.Helper;
using CitizenPortal.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Recaptcha;
using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace CitizenPortal.Controllers
{
    public class DataController : Controller
    {
        private readonly int _resultPerPage = 20;

        //
        // GET: /Data/

        public ViewResult Index(string sortOrder, string catalogFilter, string categoryFilter, string keywordFilter, int? page)
        {
            Data data = new Data();
            int pageNumber = (page ?? 1);

            if (DataHelper.AllDatasets == null)
            {
                return View(data);
            }

            data.Datasets = (from d in DataHelper.AllDatasets
                             where (string.IsNullOrEmpty(catalogFilter) || (!string.IsNullOrEmpty(d.Catalog.Alias) && d.Catalog.Alias.ToLower() == catalogFilter.ToLower()))
                             && (string.IsNullOrEmpty(categoryFilter) || (!string.IsNullOrEmpty(d.Category) && d.Category.ToLower() == categoryFilter.ToLower()))
                             && (string.IsNullOrEmpty(keywordFilter) || (d.KeywordsList != null && d.KeywordsList.Contains(keywordFilter.ToLower())))
                             select d).Select(d =>
                             {
                                d.Rating = DataHelper.AllRates.Where(r => r.ItemKey == string.Format("{0}||{1}", d.Catalog.Alias, d.EntitySet)).Sum(r => r.RateValue);
                                return d;
                            });

            data.Catalogs = from d in data.Datasets
                            group d by d.Catalog into g
                            select new Data.DatasetProperty { Value = g.Key.Alias, Description = g.Key.Description, Counter = g.Count() };

            data.Categories = from d in data.Datasets
                              group d by d.Category into g
                              select new Data.DatasetProperty { Value = g.Key, Counter = g.Count() };
            
            data.Keywords = from d in data.Datasets
                            where d.KeywordsList != null
                            from e in d.KeywordsList
                            group d by e into g
                            select new Data.DatasetProperty { Value = g.Key, Counter = g.Count() };

            // Order datasets
            switch (sortOrder)
            {
                case "Date":
                    data.Datasets = data.Datasets.OrderBy(d => d.ReleaseDate).ToList();
                    break;
                case "Date desc":
                    data.Datasets = data.Datasets.OrderByDescending(d => d.ReleaseDate).ToList();
                    break;
                case "Name desc":
                    data.Datasets = data.Datasets.OrderByDescending(d => d.Name).ToList();
                    break;
                default:
                    data.Datasets = data.Datasets.OrderBy(d => d.Name).ToList();
                    break;
            }

            // Set pagination information
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPage = Math.Ceiling((double)data.Datasets.Count() / (double)_resultPerPage);

            // Set sorting variables
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = (string.IsNullOrEmpty(sortOrder) ? "Name desc" : string.Empty);
            ViewBag.DateSortParam = (sortOrder == "Date" ? "Date desc" : "Date");

            // Set filtering variables
            ViewBag.CatalogFilter = catalogFilter;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.KeywordFilter = keywordFilter;

            // Filter and order the displayed data
            data.Datasets = data.Datasets.Skip((pageNumber - 1) * _resultPerPage).Take(_resultPerPage).ToList();
            data.Categories = data.Categories.OrderBy(d => d.Value);
            data.Keywords = data.Keywords.OrderBy(d => d.Value);

            return View(data);
        }

        //
        // GET: /Data/Show

        public ActionResult Show(string catalogName, string datasetName)
        {
            RecaptchaControlMvc.PublicKey = ConfigHelper.Config.RecaptchaPublicKey;
            RecaptchaControlMvc.PrivateKey = ConfigHelper.Config.RecaptchaPrivateKey;

            try
            {
                Dataset dataset = (from d in DataHelper.AllDatasets
                                   where (d.Catalog.Alias == catalogName) && (d.EntitySet == datasetName)
                                   select d).Select(d => {
                                       d.Rating = DataHelper.AllRates.Where(r => r.ItemKey == string.Format("{0}||{1}", d.Catalog.Alias, d.EntitySet)).Sum(r => r.RateValue);
                                       return d;
                                   }).First();

                // Set current rate
                ViewBag.UserRating = (from r in DataHelper.AllRates
                                      where (r.User == Request.UserHostAddress) && (r.ItemKey == string.Format("{0}||{1}", catalogName, datasetName))
                                      select r.RateValue).FirstOrDefault();

                // Set DataService URL
                ViewBag.DataServiceUrl = ConfigHelper.Config.DataServiceUrl;

                // Set language
                ViewBag.Language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                ViewBag.LanguageFull = Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName.Replace('-', '_');
                ViewBag.LanguageFullISO = Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName;

                // Set specific title
                ViewBag.Title = datasetName;

                return View(dataset);
            }
            catch (Exception)
            {
                ViewBag.Error = CitizenPortal.Resources.Views.Data.Show.DatasetNotFound;
            }

            return View();
        }

        //
        // POST: /Data/AddComment

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult AddComment(CommentEntity model, bool captchaValid, string captchaErrorMessage)
        {
            string error = CitizenPortal.Resources.Views.Data.Comment.FormError;

            if (!ModelState.IsValid)
            {
                error = CitizenPortal.Resources.Views.Data.Comment.FormErrorFields;
            }
            else if (!captchaValid)
            {
                error = CitizenPortal.Resources.Views.Data.Comment.FormErrorCaptcha;
            }
            else
            {
                try
                {
                    // Update Azure Table
                    AzureHelper.GetCloudTable("Comments").Execute(TableOperation.Insert(model));

                    return Json(model);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }

            model.Error = error;
            return Json(model);
        }

        //
        // POST: /Data/AddRate

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddRate(RateEntity model)
        {
            try
            {
                // Update model
                model.User = Request.UserHostAddress;

                // Update Cloud Table
                AzureHelper.GetCloudTable("RateEntry").Execute(TableOperation.Insert(model));

                // Renew rates
                DataHelper.AllRates = null;

                return Json(model);
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            return Json(model);
        }

        //
        // POST: /Data/RemoveRate

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveRate(RateEntity model)
        {
            try
            {
                // Update Cloud Table
                CloudTable cloudTable = AzureHelper.GetCloudTable("RateEntry");
                TableQuery<RateEntity> query = new TableQuery<RateEntity>();
                foreach (RateEntity entity in cloudTable.ExecuteQuery(query).Where(r => r.User == Request.UserHostAddress))
                {
                    cloudTable.Execute(TableOperation.Delete(entity));
                }

                // Renew rates
                DataHelper.AllRates = null;

                return Json(model);
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            return Json(model);
        }
    }
}
