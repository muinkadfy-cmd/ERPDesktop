namespace ERPDesktop.Shared.Paths;

public static class AppPaths
{
    public static string DataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ERPDesktop", "Data");

    public static string LogsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ERPDesktop", "Logs");

    public static string DatabaseFilePath => Path.Combine(DataDirectory, "erpdesktop.db");
}
