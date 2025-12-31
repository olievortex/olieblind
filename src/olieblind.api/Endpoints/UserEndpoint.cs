using Microsoft.AspNetCore.Http.HttpResults;
using olieblind.lib.CookieConsent;
using olieblind.lib.Models;

namespace olieblind.api.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/api/user/cookieConsent", ReadVideoPageDefault);
    }

    public static async Task<Results<Ok, BadRequest<ErrorModel>>> ReadVideoPageDefault(UserCookieConsentModel model, HttpContext context, ICookieConsentBusiness business, CancellationToken ct)
    {
        var sourceIp = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var userAgent = context.Request.Headers.UserAgent.ToString();

        if (!await business.LogUserCookieConsentAsync(model.CookieValue, userAgent, sourceIp, ct))
            return TypedResults.BadRequest(new ErrorModel { Message = "Can't parse the cookie value" });

        return TypedResults.Ok();
    }
}
