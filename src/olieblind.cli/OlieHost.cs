using Microsoft.Extensions.DependencyInjection;

namespace olieblind.cli;

public class OlieHost
{
    public required IServiceScopeFactory ServiceScopeFactory { get; set; }
}
