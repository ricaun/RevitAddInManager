using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

public class Installer
{
    const string APPDATA = "%AppDataFolder%";

    const string projectName = "Teste";
    const string outputName = "Teste";
    const string outputDir = "output";
    const string version = "1.0.0";

    private static void Main(string[] args)
    {
        string installationDir = @$"{APPDATA}\Autodesk\ApplicationPlugins\";

        var fileName = new StringBuilder().Append(outputName).Append("-").Append(version);
        //Additional suffixes for unique configurations add here

        var project = new Project
        {
            Name = projectName,
            OutDir = outputDir,
            Platform = Platform.x64,
            UI = WUI.WixUI_InstallDir,
            Version = new Version(version),
            OutFileName = fileName.ToString(),
            InstallScope = InstallScope.perUser,
            MajorUpgrade = MajorUpgrade.Default,
            GUID = new Guid("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
            //BannerImage = @"Installer\Resources\Icons\BannerImage.png",
            //BackgroundImage = @"Installer\Resources\Icons\BackgroundImage.png",
            ControlPanelInfo =
            {
                Manufacturer = Environment.UserName,
                //ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico"
            },
            Dirs = new Dir[] { CreateInstallDir(args[0]) }
        };
        project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
        project.BuildMsi();
    }

    private static InstallDir CreateInstallDir(string bundleFolder)
    {
        var installationDir = @"%ProgramDataFolder%\Autodesk\ApplicationPlugins\";

        var bundleFiles = new List<WixEntity>();

        var directoryInfo = new DirectoryInfo(bundleFolder);
        var directoryName = directoryInfo.Name;
        var files = new Files($@"{bundleFolder}\*.*");
        bundleFiles.Add(new Dir(directoryName, files));

        return new InstallDir(installationDir, bundleFiles.ToArray());
    }

    static WixEntity[] GenerateWixEntities(string[] args)
    {
        var versionStorages = new Dictionary<string, List<WixEntity>>();

        foreach (var directory in args)
        {
            Console.WriteLine(directory);
            var directoryInfo = new DirectoryInfo(directory);
            var fileVersion = directoryInfo.Name;
            var files = new Files($@"{directory}\*.*");
            if (versionStorages.ContainsKey(fileVersion))
                versionStorages[fileVersion].Add(files);
            else
                versionStorages.Add(fileVersion, new List<WixEntity> { files });

            var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            Console.WriteLine($"Added '{fileVersion}' version files: ");
            foreach (var assembly in assemblies) Console.WriteLine($"'{assembly}'");
        }

        return versionStorages.Select(storage => new Dir(storage.Key, storage.Value.ToArray())).Cast<WixEntity>().ToArray();
    }

}



