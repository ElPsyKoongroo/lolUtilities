namespace LeagueUtilities;

public partial class League
{
    public static TimeSpan getTimeSpanBetween(int start, int end){
        return TimeSpan.FromSeconds( ( Random.Shared.Next(start * 10, end * 10) ) / 10.0 );
    }
    
    private static void CreateLogger()
    {
        //.Filter.ByExcluding(Matching.WithProperty<int>("Count", p => p < 5))
        const string formato = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        var logFilePath = Path.Combine(Environment.CurrentDirectory, "logs", "log.txt");
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: logFilePath,
                outputTemplate: formato,
                restrictedToMinimumLevel: LogEventLevel.Debug,
                rollingInterval: RollingInterval.Day)
            .WriteTo.Console(
                outputTemplate: formato,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();
    }
}