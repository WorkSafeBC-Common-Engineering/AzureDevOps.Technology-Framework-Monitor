AzureDevOps.Technology-Framework-Monitor
Product to gather information about every project within an Azure DevOps organization. The product scans every repository, analyses the projects within each repository, and saves data in a SQL Server database for further analysis and reporting.

Requirements:
You’ll need a personal access token and 2 keys to use TFM.

Visual Studio: Download all the non-game development packages/features. 

Download SQL Server Management Studio (SSMS)

https://aka.ms/ssmsfullsetup

Download SQL Server 2022 

https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x409&culture=en-us&country=us

1. Open SQL Server installer. Click Custom, Install.

![image](https://user-images.githubusercontent.com/64446306/235730733-a4721bb9-0615-46b1-aca8-784e112187f6.png)

2. Under the installation tab, New SQL Server.

![image](https://user-images.githubusercontent.com/64446306/235723215-d745accc-ecd5-43c1-adb8-14d71d7d5a7f.png)

3. Install Rules: click next (Firewall may generate a warning)

4. Installation Type: click Perform a new installation of SQL Server 2022, next

5. Edition: free edition, next

6. License Terms: Accepts terms, next

7. Azure Extension for SQL Server: Disable Azure Extension, next

8. Feature Selection: Select the following features: Enable Full-Text, Analysis Services, Database Engine Services, Master Data Services, and Integration Services, next

9. Instance Configuration: Default Instance, next (unless name’s already in use, create a new name)

![image](https://user-images.githubusercontent.com/64446306/236272591-75d14fd0-d7f8-4b2f-bcff-365e599bac78.png)

10. Server configuration: next

11. Database Engine Configuration: click the “Add Current User” button, next.

12. Analysis Services Configuration: click the “Add Current User” button, next.

13. Install, close.

14. Open SSMS (SQL Server Management Studio)

15. Connect your server.

![image](https://user-images.githubusercontent.com/64446306/236272730-960c1d1a-22ec-419c-aeb8-b3ba8fa96a09.png)

Below is what it should look like in the object explorer

![image](https://user-images.githubusercontent.com/64446306/236272870-15685d29-2e46-4254-abb0-0178602a46e0.png)

16. Go to https://github.com/WorkSafeBC-Common-Engineering, and clone their TFM repo using Visual Studio.

17. Open ProjectScanner.sln (AzureDevOps.Technology-Framework-Monitor\IT.TFM)

![image](https://user-images.githubusercontent.com/64446306/236270599-e7919285-f16e-4f57-b652-cc4a82898461.png)

18. Expand the Data Sources folder, Right-click ProjectScannerDB, and publish.

![image](https://user-images.githubusercontent.com/64446306/236270420-82d8de58-d2ef-4b30-bcbe-80ba2bcb298f.png)

19. A window will pop up and click edit, then under the browse tab, select the same database you created during the SQL Server setup.

![image](https://user-images.githubusercontent.com/64446306/236271560-59c45a82-f713-4cb1-aa82-6d767629b635.png)

20. Name your database name. The database will be transferred to your created server.

![image](https://user-images.githubusercontent.com/64446306/236271634-d004d2b8-d121-431b-a54b-6761a037e3a2.png)

21. Click publish, and it will close the window. You will get these terminal messages.

![image](https://user-images.githubusercontent.com/64446306/236271735-873d6e00-a0ba-476c-a9e0-2bbf375b99e5.png)

22. From the Solution Explorer, expand Executables, expand TfmScanWithToken, and open “App.config”.

![image](https://user-images.githubusercontent.com/64446306/236271780-df1128bf-0d51-4fc9-a1b0-5c4322a47baa.png)

23. In the appSettings section of the XML file, replace OrgName and OrgUrl with the given keys.

![image](https://user-images.githubusercontent.com/64446306/236271835-d634a496-5956-4e21-b3dd-9e1e9676d729.png)

24. In the connectionString section, change the data source value to your database name.

![image](https://user-images.githubusercontent.com/64446306/236271913-237b8c4e-0605-4e8e-bdc7-3f25bd2b26ba.png)

25. Solution Explorer, right-click TfmScanWithToken, and click properties.

26. Debug tab, in the command-line arguments, paste your given personal access token (PAT) key.

![image](https://user-images.githubusercontent.com/64446306/236271971-9f42d900-9c48-43d6-9262-07f0eed45b87.png)

27. Go to system environment variables in your OS and click Environment Variables

![image](https://user-images.githubusercontent.com/64446306/236272102-1e679447-ff0d-4de8-a9c6-1c8ddec9bac7.png)
![image](https://user-images.githubusercontent.com/64446306/236272151-468487be-88d7-457a-b8fc-97244ab9f46e.png)

28. In your user variables, create a new user variable.

29. For the Variable name, enter tokenVariable.

30. For the Variable value, enter your personal access token.

![image](https://user-images.githubusercontent.com/64446306/236272234-1776dc94-3fa0-4724-9e46-a06a10477d87.png)

31. Go back to Solution Explorer, right-click TfmScanWithToken, click “Set as Startup Project”, then press start at the top.

![image](https://user-images.githubusercontent.com/64446306/236272381-6225c4ac-410e-4294-9495-401b6b72d51f.png)

32. A console window will pop up and your Scanner should be outputting terminal messages.

![image](https://user-images.githubusercontent.com/64446306/236272433-fc7370f5-fcc3-491b-a755-7dbebb3be70d.png)

33. Once it completes the scan, it will say “Press any key to exit”.

![image](https://user-images.githubusercontent.com/64446306/236272481-d0df026d-6e67-44ee-9094-aa1b721e5cec.png)

34. Go to your server in SSMS, your database will be populated with the scanned files.
