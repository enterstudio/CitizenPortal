using CitizenPortal.Helper;
using CitizenPortal.Models;
using Recaptcha;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace CitizenPortal.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/

        [AllowAnonymous]
        public ActionResult Index()
        {
            RecaptchaControlMvc.PublicKey = ConfigHelper.Config.RecaptchaPublicKey;
            RecaptchaControlMvc.PrivateKey = ConfigHelper.Config.RecaptchaPrivateKey;

            return View();
        }

        //
        // POST: /Contact/

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Index(Contact model, bool captchaValid, string captchaErrorMessage)
        {
            // Check captcha
            if (!captchaValid)
            {
                ModelState.AddModelError("captcha", CitizenPortal.Resources.Views.Contact.Index.FormErrorCaptcha);
            }

            // Check email format
            if (ModelState.IsValidField("Email") && !EmailHelper.IsValid(model.Email))
            {
                ModelState.AddModelError("Email", CitizenPortal.Resources.Views.Contact.Index.FormErrorEmail);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Send email
                    MailMessage email = new MailMessage();
                    email.From = new MailAddress(model.Email, model.Name);
                    email.To.Add(ConfigHelper.Config.ContactEmail);
                    email.Subject = "[Portail Citoyen] " + model.Subject;
                    email.Body = model.Message;

                    // Configure SMTP client
                    SmtpClient smtp = new SmtpClient();

                    smtp.EnableSsl = ConfigHelper.Config.SMTP_EnableSSL;

                    if (!string.IsNullOrEmpty(ConfigHelper.Config.SMTP_Host))
                    {
                        smtp.Host = ConfigHelper.Config.SMTP_Host;
                    }

                    int smtpPort;
                    if (!string.IsNullOrEmpty(ConfigHelper.Config.SMTP_Port) && int.TryParse(ConfigHelper.Config.SMTP_Port, out smtpPort) && smtpPort > 0)
                    {
                        smtp.Port = smtpPort;
                    }
                    
                    if (!string.IsNullOrEmpty(ConfigHelper.Config.SMTP_User))
                    {
                        smtp.Credentials = new NetworkCredential(ConfigHelper.Config.SMTP_User, ConfigHelper.Config.SMTP_Password);
                    }
                    
                    // Send email
                    smtp.Send(email);

                    // Clear form
                    model.Name = string.Empty;
                    model.Email = string.Empty;
                    model.Message = string.Empty;
                    ModelState.Clear();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("SMTP", CitizenPortal.Resources.Views.Contact.Index.FormErrorSendEmail);
                }
            }

            return View(model);
        }
    }
}
