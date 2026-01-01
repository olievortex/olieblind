namespace olieblind.cli;

public class OlieArgs
{
    public CommandsEnum Command { get; private set; }
    public int IntArg1 { get; private set; }

    public enum CommandsEnum
    {
        DayOneMaps,
        DeleteOldContent,
        DroughtMonitorVideo,
        EventsDatabase,
        SpcDayOneVideo,
        SpcDayTwoVideo,
        SpcDayThreeVideo,
        ListVoices,
        LoadRadars
    }

    public OlieArgs(string[] args)
    {
        if (args.Length == 0) throw new ArgumentException("The command name is missing");

        var command = args[0].ToLower();

        Command = command switch
        {
            "dayonemaps" => CommandsEnum.DayOneMaps,
            "deleteoldcontent" => CommandsEnum.DeleteOldContent,
            "droughtmonitorvideo" => CommandsEnum.DroughtMonitorVideo,
            "eventsdatabase" => ReadArgsForEventsDatabase(args),
            "spcdayonevideo" => CommandsEnum.SpcDayOneVideo,
            "spcdaytwovideo" => CommandsEnum.SpcDayTwoVideo,
            "spcdaythreevideo" => CommandsEnum.SpcDayThreeVideo,
            "listvoices" => CommandsEnum.ListVoices,
            "loadradars" => CommandsEnum.LoadRadars,
            _ => throw new ArgumentException($"Unknown command {command}")
        };
    }

    private CommandsEnum ReadArgsForEventsDatabase(string[] args)
    {
        const string usage = "Usage: dotnet olieblind.cli.dll eventsdatabase [year]";

        if (args.Length == 1) throw new ArgumentException(usage);
        if (!int.TryParse(args[1], out var year)) throw new ArgumentException(usage);

        IntArg1 = year;

        return CommandsEnum.EventsDatabase;
    }
}