using BRP.Mailer.API.Infrastructure;
using ErrorOr;
using MailKit.Net.Smtp;
using MediatR;
using MimeKit;
using MimeKit.Text;
using static BRP.Mailer.API.Application.EmailConstants;

namespace BRP.Mailer.API.Application.Mails.Commands.SendEmail;

internal sealed class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, ErrorOr<Success>>
{
    private readonly ILogger<SendEmailCommandHandler> _logger;

    public SendEmailCommandHandler(ILogger<SendEmailCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {        
        _logger.LogInformation(
            "[{Handler}] Attempting to send email to {To} from {From} with subject {Subject}",
            nameof(SendEmailCommandHandler),
            request.To,
            request.From,
            request.Subject);

        try 
        {
            await SendEmailAsync(request);
            return Result.Success;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send email to {To}", request.To);
            return Error.Unexpected(description: $"Failed to send email. {e}");
        }
    }

    public async Task SendEmailAsync(SendEmailCommand request)
    {
        using var client = new SmtpClient();

        client.Connect(Server, Port, useSsl: false);
        var message = new MimeMessage();
        var fromAddress = new MailboxAddress(request.From, request.From);
        message.From.Add(fromAddress);
        message.To.Add(new MailboxAddress(request.To, request.To));
        message.Subject = request.Subject;
        message.Sender = fromAddress;
        message.Body = new TextPart(request.IsHtmlBody ? TextFormat.Html : TextFormat.Plain)
        {
            Text = request.Body
        };

        await client.SendAsync(message);
        _logger.LogInformation("Email Sent to {To}", request.To);

        client.Disconnect(true);
    }
}
