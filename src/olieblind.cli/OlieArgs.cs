namespace olieblind.cli;

public class OlieArgs
{
    public CommandsEnum Command { get; private set; }
    public int IntArg1 { get; private set; }
    public string StrArg1 { get; private set; } = string.Empty;

    public enum CommandsEnum
    {
        DayOneMaps,
        DeleteOldContent,
        DroughtMonitorVideo,
        EventsDatabase,
        EventsSpc,
        SatelliteInventory,
        SatelliteMarquee,
        SpcDayOneVideo,
        SpcDayTwoVideo,
        SpcDayThreeVideo,
        SpcMesos,
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
            "eventsspc" => ReadArgsForOptionalYear(args, CommandsEnum.EventsSpc),
            "satelliteinventory" => ReadArgsForOptionalYear(args, CommandsEnum.SatelliteInventory),
            "satellitemarquee" => ReadArgsForOptionalYear(args, CommandsEnum.SatelliteMarquee),
            "spcdayonevideo" => CommandsEnum.SpcDayOneVideo,
            "spcdaytwovideo" => CommandsEnum.SpcDayTwoVideo,
            "spcdaythreevideo" => CommandsEnum.SpcDayThreeVideo,
            "spcmesos" => ReadArgsForOptionalYear(args, CommandsEnum.SpcMesos),
            "listvoices" => CommandsEnum.ListVoices,
            "loadradars" => CommandsEnum.LoadRadars,
            _ => throw new ArgumentException($"Unknown command {command}")
        };
    }

    private CommandsEnum ReadArgsForEventsDatabase(string[] args)
    {
        const string usage = "Usage: dotnet olieblind.cli.dll eventsdatabase [year] [update]";

        if (args.Length != 3) throw new ArgumentException(usage);
        if (!int.TryParse(args[1], out var year)) throw new ArgumentException(usage);

        IntArg1 = year;
        StrArg1 = args[2];

        return CommandsEnum.EventsDatabase;
    }

    private CommandsEnum ReadArgsForOptionalYear(string[] args, CommandsEnum command)
    {
        const string usage = "Usage: dotnet olieblind.cli.dll [command_name] [year|blank for current year]";

        if (args.Length < 2)
        {
            IntArg1 = DateTime.UtcNow.Year;
        }
        else if (args.Length == 2)
        {
            if (!int.TryParse(args[1], out var year)) throw new ArgumentException(usage);
            IntArg1 = year;
        }
        else
        {
            throw new ArgumentException(usage);
        }

        return command;
    }
}