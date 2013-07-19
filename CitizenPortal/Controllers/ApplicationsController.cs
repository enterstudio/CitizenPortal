using CitizenPortal.Helper;
using CitizenPortal.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CitizenPortal.Controllers
{
    public class ApplicationsController : Controller
    {
        //
        // GET: /Applications/

        public ActionResult Index()
        {
            Applications model = new Applications();

            if (ConfigHelper.Config.Applications != null && ConfigHelper.Config.Applications.Length > 0)
            {
                model.AllApplications = JsonConvert.DeserializeObject<List<Application>>(ConfigHelper.Config.Applications);
            }

            return View(model);
        }
    }
}
