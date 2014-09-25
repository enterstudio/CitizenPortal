using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace CitizenPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RefreshValidationSettings();
        }

        protected void Application_BeginRequest()
        {
            CultureInfo ci = new CultureInfo("en-US");

            if (!string.IsNullOrEmpty(Request.QueryString["lang"]))
            {
                ci = new CultureInfo(Request.QueryString["lang"]);
            }
            else if (Request.UserLanguages.Length > 0)
            {
                ci = new CultureInfo(Request.UserLanguages[0]);
            }

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }

        protected void RefreshValidationSettings()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Web.config";
                string metadataAddress = ConfigurationManager.AppSettings["FederationMetadataLocation"];
                ValidatingIssuerNameRegistry.WriteToConfig(metadataAddress, configPath);
            }
            // else { // See WebRole.cs file }
        }
    }
}