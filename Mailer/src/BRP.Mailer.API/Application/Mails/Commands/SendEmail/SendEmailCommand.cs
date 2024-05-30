using ErrorOr;
using MediatR;

namespace BRP.Mailer.API.Application.Mails.Commands.SendEmail;

public record SendEmailCommand(
    string To, 
    string From, 
    string Subject, 
    string Body, 
    bool IsHtmlBody = false) : IRequest<ErrorOr<Success>>;
