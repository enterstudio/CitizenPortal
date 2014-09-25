using System;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Web.Mvc;
using CitizenPortal.Helper;
using CitizenPortal.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace CitizenPortal.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;

            string fname = (cp.FindFirst(ClaimTypes.GivenName).Value != "FirstName" ? cp.FindFirst(ClaimTypes.GivenName).Value : string.Empty);
            string lname = (cp.FindFirst(ClaimTypes.Surname).Value != "LastName" ? cp.FindFirst(ClaimTypes.Surname).Value : string.Empty);

            if (!string.IsNullOrEmpty(fname) || !string.IsNullOrEmpty(lname))
            {
                ViewBag.AdminFullName = string.Format("{0} {1}", fname, lname);
            }

            ViewBag.fname = cp.FindFirst(ClaimTypes.GivenName).Value;
            ViewBag.lname = cp.FindFirst(ClaimTypes.Surname).Value;

            return View(ConfigHelper.Config);
        }

        //
        // GET: /Admin/SignOut

        public void SignOut()
        {
            WsFederationConfiguration fc = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;

            string wreply = System.Web.HttpContext.Current.Request.UrlReferrer.ToString(); 

            SignOutRequestMessage soMessage = new SignOutRequestMessage(new Uri(fc.Issuer), wreply);
            soMessage.SetParameter("wtrealm", fc.Realm);

            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            Response.Redirect(soMessage.WriteQueryString());
        }

        //
        // POST: /Admin

        [HttpPost]
        public ActionResult Index(ConfigEntity model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update model
                    model.AdminIp = Request.UserHostAddress;

                    // Handle blob files
                    foreach (string inputName in Request.Files)
                    {
                        var file = Request.Files[inputName];
                        if (file.ContentLength > 0)
                        {
                            string fileFullName = Path.GetFileName(file.FileName);

                            CloudBlockBlob blob = AzureHelper.GetCloudBlob("CitizenPortal", fileFullName);
                            blob.UploadFromStream(file.InputStream);

                            PropertyInfo prop = model.GetType().GetProperty(inputName + "Url");
                            prop.SetValue(model, blob.Uri.ToString());
                        }
                    }

                    // Update Azure Table
                    AzureHelper.GetCloudTable("CitizenPortal").Execute(TableOperation.InsertOrReplace(model));

                    // Renew cache
                    CacheHelper.ClearAll();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("error", ex.Message);
                }
            }

            return View(model);
        }
    }
}
