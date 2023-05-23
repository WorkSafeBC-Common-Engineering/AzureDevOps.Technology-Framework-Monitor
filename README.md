## HOW TO USE THE TECHNOLOGY FRAMEWORK MONITOR WITH VISUAL STUDIO .NET COMMUNITY VERSION 2019 or 2022

### Introduction

The Technology Framework Monitor (TFM) is an open-source project developed by the WorkSafeBC (WSBC) Common Engineering (Ce) team; who are part of the Enterprise Development and Operations (EDO) group within the Innovation and Technology division (I&T). The TFM product will scan through and gather information on every project within an Azure Developer Operations organization, or repository, analyze the files within, and then save the data gathered into a Microsoft Standard Query Language (SQL) server database.  
Some features of the TFM product are to perform a scan which will iterate through projects with various file extensions, obtain a file list, and store information in the SQL database. The file list information collected can contain which .NET version, .nuget package versions, npm package versions and much more depending on the configuration settings. The data being published is stored in the SQL server database for analysis and reporting purposes. 

## Before You Begin
To start using the TFM you must have the following equipment and software installed:

1. A personal computer (PC) or laptop.
2. A Windows 10 or 11 Operating System (OS).
3. Microsoft Visual Studio .Net 2019 or 2022.
4. Git. 
5. Microsoft SQL Server 2022 Developer Edition.
6. Microsoft SQL Server Management Studio (SSMS).
7. A Microsoft Azure Organization account.
8. Microsoft Excel

### Programming Language(s)

To use the TFM you must be familiar with the following programming language: 
Microsoft C#

## Instructions

### Configuration for SQL Server 2022

1. Open your SQL Server Installer for Developer Edition. Click **Custom**, then **Install** after selecting a download path.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/6de04259-4e2d-4e92-85b7-8246a7e960d2"></p>

2. Under the **Installation** tab, click **New SQL Server standalone installation or add features to an existing installation**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/5e0f0975-f66c-4356-b945-194ddc28a24b"></p>

3. Install Rules Step: Once the operation is completed and you have passed most checks, click **Next** (Firewall may generate a warning).
4. Installation Type Step: Click **Perform a new installation of SQL Server 2022**, then **Next**.
5. Edition Step: Select the free edition with **Developer** selected in the drop-down menu, then click **Next**.
6. License Terms Step: Click accept terms and conditions, then click **Next**.
7. Azure Extension for SQL Server Step: **Uncheck Azure Extension**, then click **Next**.
8. Feature Selection Step: Select the following features, then click **Next**.
	a. Enable Full-Text
	b. Analysis Services
	c. Database Engine Services
	d. Master Data Services
	e. Integration Services
After you select all the features, click Next.
Instance Configuration Step: Click **Default Instance**, **Next** (unless you have a named instance already in use, then create a new named instance).
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/0ed208df-e168-4155-8b95-eb76a2760bdb"></p>

Server configuration Step: Click **Next**.
Database Engine Configuration Step: Click the “**Add Current User**” button, click **Next**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/49309a6e-f767-444b-a0e6-88e07880f02f"></p>

Analysis Services Configuration Step: Click the “**Add Current User**” button, click **Next**.
Click **Install**, then click **Close**.

### Configurating SQL Server Management Studio

Open your SSMS (SQL Server Management Studio) program.
Enter your Server name (created from the SQL Server setup)
Click **Connect**. (Shows your server at the sidebar)
<p align="center"><img width="750" img src="https://user-images.githubusercontent.com/64446306/236272730-960c1d1a-22ec-419c-aeb8-b3ba8fa96a09.png"></p>

### Cloning the TFM Tool

To Clone the TFM product, Open Visual Studio 2019 or 2022.
Click Clone a Repository.
Enter the TFM repository Url (https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor).
Click clone.

### Configuration for ProjectScanner

Open the ProjectScanner.sln once it is cloned (should be in the path you cloned the project to), (AzureDevOps.Technology-Framework-Monitor\IT.TFM).
Expand the Data Sources folder, Right-click ProjectScannerDB, then right-click to select further options, find and click publish.
A window will pop up and click edit, then under the browse tab, select the same database you created during the SQL Server setup.
Name your database name. The database will be transferred to your created server.
Click publish, and it will close the window. 
From the Solution Explorer, expand Executables, TfmScanWithToken, then open “App.config”.
In the appSettings section of the XML file within the TfmScanWithToken folder, replace the OrgName with your Azure DevOps Organization name and OrgUrl with your Azure DevOps Organization link (truncate the https:// prefix).
In the connectionString section, change the data source value to your database name.
Solution Explorer, right-click TfmScanWithToken, then click properties.
In the start menu, search then click system environment variables:
In your user variables, click on new.
For the Variable name, enter your tokenVariable.
For the Variable value, enter your Personal Access token (PAT). (You will need a PAT from your Azure DevOps Organization)
Restart your computer/laptop to apply the changes.

### Running the ProjectScanner and Analyzing Data

Go back to Solution Explorer, right-click TfmScanWithToken, click “Set as Startup Project”, then click Start at the top. A terminal window should pop up.
Once it completes the scan, it will say “Press any key to exit”.
Open a new Microsoft Excel sheet
Under the Data tab, click Get Data, From Database, From SQL Server Database
Enter your server and database name in the fields of the SQL Server database pop-up prompt, and click OK.
Select Full Scan, and click Load. (Your Excel sheet will be populated with data)
Under Table Design, click Summarize with PivotTable, click New Worksheet, then click OK.
A Field List should appear on the side with a series of fields to sort from. (If it is not shown, click Field List under PivotTable Analyze. With this tool, you can analyze all your repositories by choosing what fields you want to see. 
Click any field and it will appear under the Rows for ordering. (Rearrangeable to cater to your preferences)


### URL Links

Microsoft Visual Studio .NET Community Download
https://visualstudio.microsoft.com/downloads/

Microsoft SQL Server Management Studio Download
https://aka.ms/ssmsfullsetup

Microsoft SQL Server 2022 Developer Edition Download
https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x409&culture=en-us&country=us

WorkSafeBC Common Engineering Github TFM Repository
https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor

Git Download
https://git-scm.com/downloads

Microsoft Azure
https://azure.microsoft.com/en-us

### Latest Updates:

BCIT Industry Sponsored Student Project Team 5

May 9: Updated 22 projects in ProjectScanner Repository to Microsoft .NET 6 Framework in a separate branch called update-branch (Samuel)

May 16: Merged update-branch into master and dealt with conflicts. (Samuel)

May 19: Added placeholders for users to add their Azure DevOps Organization name and URL, SQL server name, and database name and updated .gitignore to protect sensitive user details (Samuel)

## Authors
### BCIT Computer Systems Technology Industry Sponsored Student Project (ISSP) Team #5:
Daniel Chellapan

Samuel Tjahjadi

### WorkSafeBC Ce I&T Division:
I&T Manager - Willy Schaub

EDO Delivery Software Developer - Andreas Mertens

EDO Common Engineering Practice Lead - Martin Lacey
