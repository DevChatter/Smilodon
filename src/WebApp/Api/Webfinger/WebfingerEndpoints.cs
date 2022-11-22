using Microsoft.AspNetCore.Mvc;

namespace Smilodon.WebApp.Api.Webfinger;

public static class WebfingerEndpoints
{
    public static RouteGroupBuilder MapWellKnownApi(this RouteGroupBuilder builder)
    {
        builder.MapGet("/host-meta", HostMeta);
        builder.MapGet("/webfinger", GetAllAccount);
        //?resource=acct:gargron@mastodon.social
        return builder;
    }

    public static async Task<IResult> HostMeta()
    {
        await Task.Delay(1); // TODO: Replace with our metadata
        return TypedResults.Ok();
    }

    public static async Task<IResult> GetAllAccount([FromQuery]string resource)
    {
        await Task.Delay(1); // TODO: Replace with DB Lookup
        return TypedResults.Ok(resource);
    }
}
