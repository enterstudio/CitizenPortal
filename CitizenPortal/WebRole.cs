using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Web.Administration;

namespace CitizenPortal
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // See the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            RoleEnvironment.Changing += RoleEnvironmentChanging;


            //Comment following code to debug the cloud service on local emulator

            #region Copy the config in web.config
            using (var server = new ServerManager())
            {
                string siteNameFromServiceModel = "Web";
                string siteName = string.Format("{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);
                string configFilePath = server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath + "\\Web.config";
                XElement element = XElement.Load(configFilePath);

                string strSetting;

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("connectionString"))))
                {
                    var v = from appSetting in element.Element("connectionStrings").Elements("add")
                            where "WindowsAzureStorage" == appSetting.Attribute("name").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("connectionString").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("bingCredential"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "bingCredential" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("FederationMetadataLocation"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "FederationMetadataLocation" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("audienceUri"))))
                {
                    element.Element("system.identityModel").Element("identityConfiguration").Element("audienceUris").Element("add").Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("trustedIssuerName"))))
                {
                    element.Element("system.identityModel").Element("identityConfiguration").Element("issuerNameRegistry").Element("authority").Attribute("name").Value = strSetting;
                    element.Element("system.identityModel").Element("identityConfiguration").Element("issuerNameRegistry").Element("authority").Element("validIssuers").Element("add").Attribute("name").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("issuer"))))
                    element.Element("system.identityModel.services").Element("federationConfiguration").Element("wsFederation").Attribute("issuer").Value = strSetting;
                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("realm"))))
                    element.Element("system.identityModel.services").Element("federationConfiguration").Element("wsFederation").Attribute("realm").Value = strSetting;

                element.Save(configFilePath);

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("FederationMetadataLocation"))))
                    ValidatingIssuerNameRegistry.WriteToConfig(strSetting, configFilePath);

                server.CommitChanges();
            }
            #endregion

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}