using Microsoft.AspNetCore.Mvc;

namespace Smilodon.WebApp.Api.Admin;

public static class WebfingerEndpoints
{
    public static RouteGroupBuilder MapWellKnownApi(this RouteGroupBuilder builder)
    {
        builder.MapGet("/webfinger", GetAllAccount);
        //?resource=acct:gargron@mastodon.social
        return builder;
    }

    public static async Task<IResult> GetAllAccount([FromQuery]string resource)
    {
        await Task.Delay(1); // TODO: Replace with DB Lookup
        return TypedResults.Ok(resource);
    }
}
