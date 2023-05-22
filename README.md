# HOW TO USE THE TECHNOLOGY FRAMEWORK MONITOR WITH VISUAL STUDIO .NET COMMUNITY VERSION 2019 or 2022


## Introduction
The Technology Framework Monitor (TFM) is an open-source project developed by the WorkSafeBC (WSBC) Common Engineering (Ce) team; who are part of the Enterprise Development and Operations (EDO) group within the Innovation and Technology division (I&T). The TFM product will scan through and gather information on every project within an Azure Developer Operations organization, or repository and analyze the files within, then save the data gathered into a Microsoft Standard Query Language (SQL) server database.  

The primary goal of the TFM product is to perform a scan which will iterate through projects with various file extensions, obtain a file list, and store information into the SQL database. The file list information collected can contain which .NET version, .nuget package versions, npm package versions and much more depending on the configuration settings. The data being published is stored in the SQL server database for analysis and reporting purposes. 

### Before You Begin
To start using the TFM you must have the following equipment and software installed:

A Windows 10 or 11 Operating System (OS).
Microsoft Visual Studio .Net 2019 or 2022.
Git. 
Microsoft SQL Server 2022 Developer Edition.
Microsoft SQL Server Management Studio (SSMS).
A Microsoft Azure Organization account.


#### Programming Language(s)

To use the TFM you must be familiar with the following programming language: 
Microsoft C#


## Instructions

### Configuration

#### Microsoft SQL Server 2022 Developer Edition:

1. Open your SQL Server Installer for Developer Edition: [Start] > [Programs] > [SQL Server 2022].

2. Click the Custom tab, then click Install.

3. After the installation is complete, Click on the Installation tab. 

5. Then click New SQL Server standalone installation or add features to an existing installation.

6. The Install Rules: Once the operation is completed and you have passed most checks, Click next (Firewall may generate a warning).

7. The Installation Type: Click Perform a new installation of SQL Server 2022. 

8. Click next after the operation is completed.  

9. The Edition: Select the free edition with developer selected in the drop down menu, then click next. 

10. The License Terms: Click on the check-box and select Accept terms and conditions, then click next.

11. The Azure Extension for SQL Server: Disable the Azure Extension, then click next. 

12. The Feature Selection: Select the following features; Enable Full-Text, Analysis Services, Database Engine Services, Master Data Services, and Integration Services, then click next.

13. The Instance Configuration: Click Default Instance, next (unless you have a named instance already in use, then create a new named instance).

14. The Server configuration: next.

15. The Database Engine Configuration: Click the “Add Current User” button, then click next.

16. The Analysis Services Configuration: Click the “Add Current User” button, then click next.

17. Click Install, then click close.

----------------------------------------------------------------------------------------------------------------------------------------

#### Microsoft SQL Server Management Studio (SSMS):

18. Open your SSMS (SQL Server Management Studio) program. 

19. Click connect the connect plug in the Databases tab to the left.

----------------------------------------------------------------------------------------------------------------------------------------
#### Microsoft Visual Studio .Net 2019 or 2022: 

20. To Clone the TFM product, Open Visual Studio 2019 or 2022.

21. Click Clone a repository.

22. Enter the TFM repository Url.  

23. Click clone.

24. Open the ProjectScanner.sln once it is cloned (should be in the path you cloned the project to), (AzureDevOps.Technology-Framework-Monitor\IT.TFM).

25. Expand the Data Sources folder, Right-click ProjectScannerDB, then right click to select further options, find and click publish.

26. A window will pop up and click edit, then under the browse tab, select the same database you created during the SQL Server setup.

27. Name your database name. The database will be transferred to your created server.

28. Click publish, and it will close the window. 

29. From the Solution Explorer, expand Executables, expand TfmScanWithToken, then open “App.config”.

30. In the appSettings section of the XML file, replace the OrgName and OrgUrl with the provided keys. You will  need a personal access token and two keys to get started when using the TFM.

31. In the connectionString section, change the data source value to your database name.

32. Solution Explorer, right-click TfmScanWithToken, then click properties.

33. Debug tab, in the command-line arguments, paste your Personal Access Token (PAT) key.   

34. In the start menu, search then click system environment variables.

35. In your user variables, click on new.

36. For the Variable name, enter tokenVariable.

37. For the Variable value, enter your PAT.

38. Go back to Solution Explorer, right-click TfmScanWithToken, click “Set as Startup Project”, then click start at the top.

39. A terminal window should pop up. 

40. Once it completes the scan, it will say “Press any key to exit”.

## URL Links   
### Microsoft Visual Studio .NET Community Download

https://visualstudio.microsoft.com/downloads/

### Microsoft SQL Server Management Studio Download

https://aka.ms/ssmsfullsetup	

### Microsoft SQL Server 2022 Developer Edition Download

https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x409&culture=en-us&country=us 

### WorkSafeBC Common Engineering Github TFM Repository

https://github.com/WorkSafeBC-Common-Engineering/AzureDevOps.Technology-Framework-Monitor

### Git Download

https://git-scm.com/downloads 

### Microsoft Azure
https://azure.microsoft.com/en-us/ 

## Latest Updates

### Author(s)

BCIT Computer Systems Technology Industry Sponsored Student Project (ISSP) Team #5:

    Daniel Chellapan
    Samuel Tjahadi


WorkSafeBC Ce I&T Division:

    I&T Manager - Willy Schaub
    EDO Delivery Software Developer - Andreas Mertens
    EDO Common Engineering Practice Lead - Martin Lacey

## Changes: 

#### BCIT Computer Systems Technology Industry Sponsored Student Project (ISSP) Team #5:

##### May 2

9:50 AM - Started the development for a ReadMe.md through a shared Google Document link for collaboration with the BCIT and WorkSafeBC Ce I&T team through Google Drive. (Daniel C) 

10:16 AM - Added the first draft of configuration instructions to launch the TFM product.  (Samuel T) 

10:46 AM - Added first draft of images for the SQL Developer Edition 2022 Installation, publishing to the SSMS Database, and the TFM Scanner product. (Samuel T)

##### May 3

8:20 AM - Added the PAT instructions (Samuel T) 

##### May 4

1:03 PM - Added the first draft of the Introduction. (Daniel C) 

4:35 PM - Added the first draft of the Before You Begin, Product Requirements, title and heading formatting. (Daniel C)

##### May 17

4:55 PM - Added Instructions, headings titles, second draft of the ReadMe.md, edited the configuration instructions, added the URL links section with a Table Of Contents section. (Daniel C) 

##### May 18

11:10 AM - Made edits to the introduction and configuration settings. (Daniel C) 

3:05 PM - Edited SQL 2022 Developer Edition images, and configuration settings. (Daniel C) 

##### May 21

12:23pm - Added Latest Updates Section, re-sized all images to have it more balanced within the readMe document, and formatting. (Daniel C) 