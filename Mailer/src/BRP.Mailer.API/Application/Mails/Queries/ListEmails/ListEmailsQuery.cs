using BRP.Mailer.API.Domain;
using MediatR;

namespace BRP.Mailer.API.Application.Mails.Queries.ListEmails;

public record ListEmailsQuery() : IRequest<List<OutboxEmail>>;

