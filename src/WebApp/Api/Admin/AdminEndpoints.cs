namespace Smilodon.WebApp.Api.Admin;

public static class AdminEndpointsV1
{
    public static RouteGroupBuilder MapAdminApiV1(this RouteGroupBuilder builder)
    {
        builder.MapGet("/accounts", GetAllAccounts);
        builder.MapGet("/accounts/{id}", GetAccount);
        builder.MapPost("/accounts/{id}/action", ActOnAccount);
        
        return builder;
    }

    public static async Task<IResult> GetAllAccounts()
    {
        List<string> accounts = new List<string> { "@Brendoneus", "@DevChatter" };
        await Task.Delay(1); // TODO: Replace with DB Lookup
        return TypedResults.Ok(accounts);
    }

    public static async Task<IResult> GetAccount(int id)
    {
        await Task.Delay(1); // TODO: Replace with DB Lookup
        return TypedResults.Ok("@Brendoneus");
    }

    public static async Task<IResult> ActOnAccount(int id)
    {
        await Task.Delay(1); // TODO: Replace with DB write
        return TypedResults.Ok();
    }
}
