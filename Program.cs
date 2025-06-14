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
            // ���.NET 8.0����ʱ
            if (!IsDotNet8Installed())
            {
                var result = MessageBox.Show(
                    "��Ӧ�ó�����Ҫ.NET 8.0��ܡ��Ƿ�Ҫ�������ز���װ��",
                    "ȱ������ʱ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // ��.NET 8.0����ҳ��
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://builds.dotnet.microsoft.com/dotnet/Sdk/8.0.411/dotnet-sdk-8.0.411-win-x64.exe",
                        UseShellExecute = true
                    });
                }

                // �˳�Ӧ�ó���
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
                    // ��ȡVersionֵ
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