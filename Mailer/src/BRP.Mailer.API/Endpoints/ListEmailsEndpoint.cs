using BRP.Mailer.API.Application.Mails.Queries.ListEmails;
using BRP.Mailer.API.Mappings;
using BRPartners.Shared.Responses.Mailer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BRP.Mailer.API.Endpoints;

internal static class ListEmailsEndpoint
{
    public static void MapListEmailsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("/api/emails", ListEmailsAsync)
            .WithName("ListEmails")
            .Produces(StatusCodes.Status200OK, typeof(ListEmailsResponse));
    }

    private static async Task<IResult> ListEmailsAsync(
        [FromServices] ISender sender, 
        CancellationToken token)
    {
        var query = new ListEmailsQuery();

        var result = await sender.Send(query, token);
        var response = new ListEmailsResponse
        {
            Items = result.ConvertAll(e => e.ToResponse())
        };
        return Results.Ok(response);
    }
}
