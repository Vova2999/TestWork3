namespace App;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Locator.Current.Locate<App>().Run();
    }
}