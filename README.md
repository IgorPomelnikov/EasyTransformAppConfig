# EasyTransformAppConfig
This program creates App.config, App.Release.config, App.Debug.config, than change &lt;your-project-name>.csproj file writing configurations for app configs. Before changing it creates backup file in the same folder &lt;your-project-name>_backup.csproj just in case. 

How to use:
1. Create new project.
2. Close your IDE.
3. Run EasyTransformConfig.exe in the folder with &ltyour-project-name>.csproj.
4. Start the solution.
5. If everytihg is OK you can delete EasyTransformConfig.exe and &lt;your-project-name>_backup.csproj files. If it is not - I am so sorry, it didn't work. Delete your new created .csproj file and rename backup file.

Inspired by https://stackoverflow.com/questions/3004210/app-config-transformation-for-projects-which-are-not-web-projects-in-visual-stud
