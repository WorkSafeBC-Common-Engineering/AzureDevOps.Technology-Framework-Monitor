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

9. After you select all the features, click Next.
10. Instance Configuration Step: Click **Default Instance**, **Next** (unless you have a named instance already in use, then create a new named instance).
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/0ed208df-e168-4155-8b95-eb76a2760bdb"></p>

11. Server configuration Step: Click **Next**.
12. Database Engine Configuration Step: Click the “**Add Current User**” button, click **Next**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/49309a6e-f767-444b-a0e6-88e07880f02f"></p>

13. Analysis Services Configuration Step: Click the “**Add Current User**” button, click **Next**.
14. Click **Install**, then click **Close**.

### Configurating SQL Server Management Studio

15. Open your SSMS (SQL Server Management Studio) program.

16. Enter your Server name (created from the SQL Server setup)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/05064e60-099f-4ca2-b3a1-36f5c999f1d5"></p>

17. Click **Connect**. (Shows your server at the sidebar)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/a15d17b2-3047-42b8-b1f8-8fa5c14e0562"></p>

### Cloning the TFM Tool

18. To Clone the TFM product, Open Visual Studio 2019 or 2022.
19. Click Clone a repository.
20. Enter the TFM repository Url (https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor).
21. Click clone.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/08027afa-5751-4a76-8be0-2aa315005d1e"></p>


### Configuration for ProjectScanner

22. Open the ProjectScanner.sln once it is cloned (should be in the path you cloned the project to) in AzureDevOps.Technology-Framework-Monitor\IT.TFM.
23. Expand the **Data Sources** folder, right-click ProjectScannerDB, then right-click to select further options, find and click publish.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/fd0ee2fe-30ef-43de-b3a7-95c82e16d349"></p>

24. A window will pop up and click edit, then under the browse tab, select the same server you created during the SQL Server setup.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/8a687942-b0fd-4409-847b-a16476ac995d"></p>

25. Name your database name. The database will be transferred to your created server.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/e8338d00-fa97-46d2-8cb6-2160a7f2e761"></p>

26. Click publish, and it will close the window. 
27. From the Solution Explorer, expand Executables, TfmScanWithToken, then open “App.config”.
28. In the appSettings section of the XML file within the TfmScanWithToken folder, replace the OrgName with your Azure DevOps Organization name and OrgUrl with your Azure DevOps Organization link (truncate the https:// prefix).
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/09493ce0-6757-4775-9806-7d35fe97ea7c"></p>
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/8ccf5680-f845-4d01-a452-cb583536ed01"></p>

29. In the connectionString section, change the data source value to your database name.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/8ae50bba-f457-4dbd-b52c-dc22a7c0c720"></p>
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/f86c85d2-24b6-45f3-ba44-61a0ae48158d"></p>

### Adding your Personal Access token (PAT) in System environment variable
30. In your system variables, click on new.
31. For the Variable name, enter **TFM_AdToken** and for the Variable value, enter your Personal Access token (PAT). (You will need a PAT from your Azure DevOps Organization)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/1628b215-9861-41e0-b8a5-5875e59b79ac"></p>

32. Restart your computer/laptop to apply the changes.

### Running the ProjectScanner

33. Go back to Solution Explorer, right-click TfmScanWithToken, click **Set as Startup Project**, then click **Start** at the top. A terminal window should pop up.
34. Once it completes the scan, it will say “Press any key to exit”.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/febe2542-93b2-4fef-beba-a31c3c0ccb2f"></p>

### Analyzing Data

35. Open a new Microsoft Excel sheet
36. Under the Data tab, click Get Data, From Database, From SQL Server Database
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/8da14e38-eb86-4e8c-a555-7a12ce5a748f"></p>

37. Enter your server and database name in the fields of the SQL Server database pop-up prompt, and click OK.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/abe07801-ff73-4787-99a5-f77043e27f17"></p>

38. Click **Use my current credentials**, click **Connect**, then click **OK** for the Encryption prompt.

39. Select **Full Scan**, and click **Load**. (Your Excel sheet will be populated with data)
40. Under Table Design, click Summarize with PivotTable, click New Worksheet, then click OK.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/dcbfe2c0-9fed-4ba3-96f2-f7b84641998e"></p>

41. A Field List should appear on the side with a series of fields to sort from. (If it is not shown, click Field List under PivotTable Analyze. With this tool, you can analyze all your repositories by choosing what fields you want to see. 
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/73f0f35b-265c-4a7b-bd87-ee09e6ec46b6"></p>

42. Click any field and it will appear under the Rows for ordering (rearrangeable to cater to your preferences). From here you can analyze all your projects to, for example, check package versions to update them to a newer version.

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
