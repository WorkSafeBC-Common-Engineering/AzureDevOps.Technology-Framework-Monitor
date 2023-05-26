## <p align="center">HOW TO USE THE TECHNOLOGY FRAMEWORK MONITOR WITH VISUAL STUDIO .NET COMMUNITY VERSION 2019 or 2022</p>

## Introduction

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
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/1a18e81d-6601-4f88-a5ef-6e89fc137a48"></p>

2. Under the **Installation** tab, click **New SQL Server standalone installation or add features to an existing installation**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/0ab9c3b8-6a0b-4f9a-bafc-ceca4fd605aa"></p>

3. Install Rules Step: Once the operation is completed and you have passed most checks, click **Next** (Firewall may generate a warning).
4. Installation Type Step: Click **Perform a new installation of SQL Server 2022**, then **Next**.
5. Edition Step: Select the free edition with **Developer** selected in the drop-down menu, then click **Next**.
6. License Terms Step: Click accept terms and conditions, then click **Next**.
7. Azure Extension for SQL Server Step: **Uncheck Azure Extension**, then click **Next**.
8. Feature Selection Step: **Select** the following features:

	a. Enable Full-Text

	b. Analysis Services

	c. Database Engine Services

	d. Master Data Services

	e. Integration Services
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/b9793401-270f-45c5-ab01-e582ebd10ebe"></p>

9. After you select all the features, click **Next**.
10. Instance Configuration Step: Click **Default Instance**, **Next** (unless you have a named instance already in use, then create a new named instance).
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/b612611c-8531-4cd3-b617-4da6ce039b57"></p>

11. Server configuration Step: Click **Next**.
12. Database Engine Configuration Step: Click the “**Add Current User**” button, click **Next**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/955e499b-275b-43a1-87aa-876ab7a1b8b9"></p>

13. Analysis Services Configuration Step: Click the “**Add Current User**” button, click **Next**.
14. Click **Install**, then click **Close**.

### Configurating SQL Server Management Studio

15. **Open SSMS** (SQL Server Management Studio) program.

16. Enter your **Server name** (created from the SQL Server setup)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/a948fb88-d54b-4411-8fe0-b1ae8d723daf"></p>

17. Click **Connect**. (Shows your server at the sidebar below)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/c96a65fe-9354-49bc-9a85-95b1e66f5abc"></p>

### Cloning the TFM Tool

18. To clone the TFM product, **open Visual Studio 2019 or 2022**.
19. Click **Clone a repository**.
20. Enter the **TFM repository Url** (https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor).
21. Click **Clone**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/03123224-cb33-423e-8f6a-6d10e3438f3c"></p>


### Configuration for ProjectScanner

22. **Open the ProjectScanner.sln** once it is cloned (should be in the path you cloned the project to) in AzureDevOps.Technology-Framework-Monitor\IT.TFM.
23. **Expand the Data Sources** folder, **right-click ProjectScannerDB**, and **click publish**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/2f218d45-af23-4a9b-ba3d-1f6585083b8d"></p>

24. A window will pop up and **click edit**, then under the **browse tab**, **select** the same **server** you created during the SQL Server setup.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/4b214ea2-54e2-40d4-a5e9-ab6c4f21f1ba"></p>

25. **Name your database**. The database will be transferred to your created server as below.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/08f0344f-b3bd-4244-9a1e-604542d8cc67"></p>
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/b78fd557-8afd-4d88-9e87-a6df86107082"></p>

26. **Click publish**, and it will close the window. 
27. From the Solution Explorer, **expand Executables, TfmScanWithToken, then open “App.config**”.

28. In the connectionString section, **replace** the **SERVER_PLACEHOLDER** to **your server name** and **DATABASE_PLACEHOLDER with your database name**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/4d9a1690-d6de-4167-abb4-e1347daadd82"></p>
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/b13e1184-1761-4af3-ac92-83173f7a3a29"></p>

29. In the appSettings section, **replace the ORG_NAME_PLACEHOLDER** with **your Azure DevOps Organization name** and **ORG_URL_PLACEHOLDER** with **your Azure DevOps Organization link without the http:// prefix**. 
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/210ae623-d860-40ca-96b0-a9075e135718"></p>
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/0ec34a59-1e99-49e9-ba55-489c15288e8a"></p>

### <p align="center"> **(ensure to truncate the https:// prefix as above)**. </p>
### Adding your Personal Access token (PAT) in System environment variable
30. In your system variables, click on **new**.
31. For the Variable name, enter **TFM_AdToken** and for the Variable value, enter your Personal Access token (PAT). (You will need a PAT from your Azure DevOps Organization)
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/3af45fd0-8e4b-43d7-9a73-1e0588175881"></p>

32. **Restart** your computer/laptop to apply the changes.

### Running the ProjectScanner

33. Go back to Solution Explorer, right-click TfmScanWithToken, click **Set as Startup Project**, then click **Start** at the top. A terminal window should pop up.
34. Once it completes the scan, it will say “Press any key to exit” and you may safely **exit the program**.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/9b0605bb-9719-47aa-a385-2f917309b72b"></p>

### Analyzing Data

35. **Open a new Microsoft Excel** sheet
36. Under the **Data tab**, click **Get Data**, **From Database**, **From SQL Server Database**
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/d9cc5503-1562-465d-b9b7-35ac4ae90b58"></p>

37. **Enter your server and database name** in the fields of the SQL Server database pop-up prompt, and **click OK.**
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/c5508174-99e3-4ff6-a573-35f271279e4f"></p>

38. Click **Use my current credentials**, click **Connect**, then click **OK** for the Encryption prompt.

39. Select **Full Scan**, and click **Load**. (Your Excel sheet will be populated with data)
40. Under Table Design, click Summarize with PivotTable, click New Worksheet, then click OK.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/c3934ce4-8951-47d6-83e2-e86b5f6c817b"></p>

41. A Field List should appear on the side with a series of fields to sort from. (If it is not shown, click Field List under PivotTable Analyze. With this tool, you can analyze all your repositories by choosing what fields you want to see. 
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/b2222146-0463-43c9-bb3d-4438da868c71"></p>

### You're done!

42. **Click any field** and it will appear under the Rows for ordering (rearrangeable to cater to your preferences). From here you can analyze all your projects to, for example, check package versions to update them to a newer version.

Below is a sample of using fields Package Version and Project Name to sort projects by Package version.
<p align="center"><img width="750" img src="https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor/assets/64446306/4e046991-e766-4b38-8f15-414f9b3a87b2"></p>


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

May 7: Upgraded Newtonsoft.Json to 13.0.3, and fixed issue with broken NuGet link (Andreas)

May 9: Updated all 22 projects in ProjectScanner.sln to Microsoft .NET 6 Framework in a new branch called update-branch (Samuel)

May 10: Updating the REST API, so that it can manage the cached connections better. Also managing the URLs better for the two formats. (Andreas)

May 16: Merged update-branch into master and dealt with conflicts. (Samuel)

May 19: Added placeholders for users to add their Azure DevOps Organization name and URL, SQL server name, and database name and updated .gitignore to protect sensitive user details (Samuel)

May 19: Removed need for PAT on command line. Just need to set the environment variable. (Andreas)

## Authors
### BCIT Computer Systems Technology Industry Sponsored Student Project (ISSP) Team #5:
Daniel Chellapan

Samuel Tjahjadi

### WorkSafeBC Ce I&T Division:
I&T Manager - Willy Schaub

EDO Delivery Software Developer - Andreas Mertens

EDO Common Engineering Practice Lead - Martin Lacey
