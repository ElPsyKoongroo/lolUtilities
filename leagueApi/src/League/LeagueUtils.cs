namespace LeagueUtilities;

public partial class League
{
    internal static TimeSpan getTimeSpanBetween(int start, int end){
        return TimeSpan.FromSeconds( Random.Shared.Next(start * 10, end * 10) / 10.0 );
    }
    
    private static void CreateLogger()
    {
        //.Filter.ByExcluding(Matching.WithProperty<int>("Count", p => p < 5))
        const string formato = "[{Timestamp:HH:mm:ss.ffffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        var logFileFolder = Path.Combine(Environment.CurrentDirectory, "logs");
        var logFilePath = Path.Combine(logFileFolder, "log_.txt");

        Directory.CreateDirectory(logFileFolder);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFilePath,
                outputTemplate: formato,
                restrictedToMinimumLevel: LogEventLevel.Debug,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}