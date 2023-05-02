# AzureDevOps.Technology-Framework-Monitor
Product to gather information about every project within an Azure DevOps organization. The product scans  every repository, analyses the projects within each repository, and saves data in a SQL Server database for further analysis and reporting.

101-Guide TFM Setup
Visual Studio: Download all the workloads except game development and android. (VS -> Get Tools and Features)

Download SQL Server Management Studio (SSMS)
https://aka.ms/ssmsfullsetup

Download SQL Server 2022 
https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x409&culture=en-us&country=us

Quick guide through the custom installation:
![image](https://user-images.githubusercontent.com/64446306/235730733-a4721bb9-0615-46b1-aca8-784e112187f6.png)

Click custom, install

Under installation tab, click New SQL Server

![image](https://user-images.githubusercontent.com/64446306/235723215-d745accc-ecd5-43c1-adb8-14d71d7d5a7f.png)

Install Rules: click next (Firewall may generate a warning) 

Installation Type: click Perform a new installation of SQL Server 2022, next

Edition: free edition, next

License Terms: Accepts terms, next

Disable Azure Extension, next

Enable Full-Text, Analysis Services, Database Engine Services, Master Data Services, and Integration Services, next

Default Instance, next (unless name’s already in use, create a new name)

Server configuration, next

Add Current user, next.

Add Current user, next. 

Install, close.

![image](https://user-images.githubusercontent.com/64446306/235724233-fbad621e-6f7d-4b4e-95da-441998107788.png)

