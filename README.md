OGDI DataLab Citizen Portal
========================

About
-----

The Citizen Portal is an intuitive and ergonomic website.
It is part of the Microsoft Open Data "in a box" solution and allows to display the datasets hosted by your [OGDI DataLab](https://github.com/openlab/OGDI-DataLab) instance.
It includes an administration panel to let any non-technical person to update the content, quickly and easily.


Microsoft Open Data solution
----------------------------

Microsoft has participated in the development of a solution to allow any organization to deploy a complete set of Open Data related applications.

This solution includes:

* [OGDI DataLab](https://github.com/openlab/OGDI-DataLab "Open Government Data Initiative")
* [ODAF OpenTurf](https://github.com/openlab/ODAF-OpenTurf "Open Data Application Framework")
* [ODPI](https://github.com/openlab/ODPI "Open Data Platform Installer")
* [Citizen Portal](https://github.com/alex-fournier/CitizenPortal)


Technical aspects
-----------------

The Citizen Portal is an ASP.NET MVC4 application and uses several services of Windows Azure:

* **Azure Web Sites**: The service that hosts the application.
* **Azure Storage**: The service that hosts the configuration of the application including the content and the settings.
* **Azure Active Directory**: The service that handles the access to the administration panel using a login/password scheme.


Usage
-----

1. In the *Web.config* file, replace **[StorageName]** and **[StorageKey]** by the same credentials of your OGDI DataLab configuration account.
2. Configure "Identity and Access" to enable the administration panel.

For a more detailed explanation, refer to the documentation in the *Documents* folder.
