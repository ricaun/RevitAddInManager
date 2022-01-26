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
    private static void Main(string[] args)
    {
        string bundleFolder;
        string projectName = "Installer";
        string projectVersion = "1.0.0";
        string outputInstaller = "Output";
        Guid guid = new Guid("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");

        if (args.Length >= 1)
        {
            bundleFolder = args[0];
            if (args.Length >= 2) projectName = args[1];
            if (args.Length >= 3) projectVersion = args[2];
            if (args.Length >= 4) outputInstaller = args[3];
            projectVersion = BuildMsiBundle(bundleFolder, projectName, projectVersion, outputInstaller, guid);
        }
        else
        {
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("Installer.exe bundleFolder projectName projectVersion outputInstaller");
            Console.WriteLine("---------------------------------------------------------------------");
        }
    }

    /// <summary>
    /// Create Msi Installation
    /// </summary>
    /// <param name="bundleFolder"></param>
    /// <param name="projectName"></param>
    /// <param name="projectVersion"></param>
    /// <param name="outputInstaller"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    private static string BuildMsiBundle(string bundleFolder, string projectName, string projectVersion, string outputInstaller, Guid guid)
    {
        // Remove Revision 
        projectVersion = Version.Parse(projectVersion).ToString(3);

        var fileName = $"{projectName} {projectVersion}";

        var project = new Project
        {
            Name = projectName,
            OutDir = outputInstaller,
            Platform = Platform.x64,
            UI = WUI.WixUI_InstallDir,
            Version = new Version(projectVersion),
            OutFileName = fileName.ToString(),
            InstallScope = InstallScope.perMachine,
            MajorUpgrade = MajorUpgrade.Default,
            GUID = guid,
            //BannerImage = @"Installer\Resources\Icons\BannerImage.png",
            //BackgroundImage = @"Installer\Resources\Icons\BackgroundImage.png",
            ControlPanelInfo =
                {
                    Manufacturer = Environment.UserName,
                    //ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico"
                },
            Dirs = new Dir[] { CreateInstallDir(bundleFolder) }
        };
        project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.VerifyReadyDlg);
        return project.BuildMsi();
    }

    /// <summary>
    /// Create InstallDir
    /// </summary>
    /// <param name="bundleFolder"></param>
    /// <returns></returns>
    private static InstallDir CreateInstallDir(string bundleFolder)
    {
        var installationDir = @"%CommonAppDataFolder%\Autodesk\ApplicationPlugins\";

        var bundleFiles = new List<WixEntity>();

        var directoryInfo = new DirectoryInfo(bundleFolder);
        var directoryName = directoryInfo.Name;
        var files = new Files($@"{bundleFolder}\*.*");
        bundleFiles.Add(new Dir(directoryName, files));

        return new InstallDir(installationDir, bundleFiles.ToArray());
    }
}



