namespace IdentityCore;

public class MailMessage
{
    public string Id { get; }
    public string From { get; }
    public string Subject { get; }
    public DateTime Date { get; }
    public string Body { get; }
    public string TextBody { get; }
    public string HtmlBody { get; }

    public MailMessage(string id, string from, string subject, DateTime date, string body, string textBody, string htmlBody)
    {
        Id = id;
        From = from;
        Subject = subject;
        Date = date;
        Body = body;
        TextBody = textBody;
        HtmlBody = htmlBody;
    }
}