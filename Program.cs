using Microsoft.Win32;
using System.Diagnostics;

namespace NET_2018K
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 检查.NET 8.0运行时
            if (!IsDotNet8Installed())
            {
                var result = MessageBox.Show(
                    "此应用程序需要.NET 8.0框架。是否要立即下载并安装？",
                    "缺少运行时",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // 打开.NET 8.0下载页面
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://builds.dotnet.microsoft.com/dotnet/Sdk/8.0.411/dotnet-sdk-8.0.411-win-x64.exe",
                        UseShellExecute = true
                    });
                }

                // 退出应用程序
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frMain());
        }
        static bool IsDotNet8Installed()
        {
            const string subkey = @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost";

            using (var hostKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(subkey))
            {
                if (hostKey != null)
                {
                    // 读取Version值
                    var versionValue = hostKey.GetValue("Version") as string;

                    if (!string.IsNullOrEmpty(versionValue) && versionValue.StartsWith("8."))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}