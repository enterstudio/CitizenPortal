To use the citizen portal:

1) In Web.config, replace [StorageName], [StorageKey], [AADName] and [DNSName] 
	- [StorageName] -> Storage Name in Azure
	- [StorageKey]	-> Storage primary Key 
	- [AADName] 	-> Domain in Azure Active Directory  *****.onmicrosoft.com
	- [DNSName] 	-> ID of the cloud service in Azure (This field and AAd Setting must be the same) 
2) If some references are invalid, just delete and re-add them to the project.
3) When deploying, you must configure "Identity and Access" to enable the administration panel.
