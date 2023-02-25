using Newtonsoft.Json.Linq;

namespace IdentityCore;

public class TempMail
{
    public static async Task<MailMessage[]> GetMessagesAsync(string email)
    {
        var emailParts = email.Split("@");
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://www.1secmail.com/api/v1/?action=getMessages&login={emailParts[0]}&domain={emailParts[1]}");
        if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase ?? $"Failed: API failure, {response.StatusCode}");
        var json = JArray.Parse(await response.Content.ReadAsStringAsync());
        var messages = new List<MailMessage>();
        foreach (var jToken in json)
        {
            var item = (JObject)jToken;
            var id = (string)item["id"]!;
            var readResponse = await httpClient.GetAsync($"https://www.1secmail.com/api/v1/?action=readMessage&login={emailParts[0]}&domain={emailParts[1]}&id={id}");
            if(!readResponse.IsSuccessStatusCode) continue;
            var mail = JObject.Parse(await readResponse.Content.ReadAsStringAsync());
            var from = (string)mail["from"]!;
            var subject = (string)mail["subject"]!;
            var date = DateTime.Parse((string)mail["date"]!);
            var body = (string)mail["body"]!;
            var textBody = (string)mail["textBody"]!;
            var htmlBody = (string)mail["htmlBody"]!;
            messages.Add(new MailMessage(id, from, subject, date, body, textBody, htmlBody));
        }

        return messages.ToArray();
    }
}