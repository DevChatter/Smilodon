using Smilodon.Domain.Models;
using Smilodon.Domain.Persistence;

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

    public static async Task<IResult> GetAllAccounts(IRepository<Account> repository, CancellationToken cancellationToken)
    {
        List<Account> accounts = await repository.ListAsync(cancellationToken);
        return TypedResults.Ok(accounts);
    }

    public static async Task<IResult> GetAccount(long id, IRepository<Account> repository, CancellationToken cancellationToken)
    {
        Account? account = await repository.GetByIdAsync(id, cancellationToken);
        return account is null ? TypedResults.NotFound(new {}) : TypedResults.Ok(account);
    }

    public static async Task<IResult> ActOnAccount(int id)
    {
        await Task.Delay(1); // TODO: Replace with DB write
        return TypedResults.Ok();
    }
}
