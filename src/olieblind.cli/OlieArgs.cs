namespace olieblind.cli;

public class OlieArgs
{
    public CommandsEnum Command { get; private set; }

    public enum CommandsEnum
    {
        DayOneMaps,
        DeleteOldContent,
        DroughtMonitorVideo,
        SpcDayOneVideo,
        SpcDayTwoVideo,
        SpcDayThreeVideo,
        ListVoices
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
            "spcdayonevideo" => CommandsEnum.SpcDayOneVideo,
            "spcdaytwovideo" => CommandsEnum.SpcDayTwoVideo,
            "spcdaythreevideo" => CommandsEnum.SpcDayThreeVideo,
            "listvoices" => CommandsEnum.ListVoices,
            _ => throw new ArgumentException($"Unknown command {command}")
        };
    }
}