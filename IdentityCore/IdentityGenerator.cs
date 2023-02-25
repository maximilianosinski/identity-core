using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace IdentityCore;

public partial class IdentityGenerator
{
    public static async Task<Identity> GenerateCompleteAsync(GeneratorSettings? generatorSettings = null)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://randomuser.me/api/");
        if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase ?? $"Failed: API failure, {response.StatusCode}");
        var json = (JObject)JObject.Parse(await response.Content.ReadAsStringAsync())["results"]![0]!;
        Enum.TryParse((string)json["gender"]!, true, out Gender gender);
        var firstname = (string)json["name"]!["first"]!;
        var lastname = (string)json["name"]!["last"]!;
        
        var rgx = WebSafeRegex();
        firstname = rgx.Replace(firstname, "");
        lastname = rgx.Replace(lastname, "");
        
        var username = (string)json["login"]!["username"]!;
        var email = await GenerateTempMailAsync();
        var password = (string)json["login"]!["password"]! + (string)json["login"]!["salt"]!;
        var picture = (string)json["picture"]!["large"]!;
        var streetName = (string)json["location"]!["street"]!["name"]!;
        var streetNumber = (int)json["location"]!["street"]!["number"]!;
        var postcode = json["location"]!["postcode"]!.ToString();
        var city = (string)json["location"]!["city"]!;
        var country = (string)json["location"]!["country"]!;
        var birthDate = DateTime.Parse(json["dob"]!["date"]!.ToString().Replace("/", "-"));
        var userAgent = GenerateUserAgentAsync();

        if (generatorSettings is { PictureContentScope: { } })
        {
            picture = $"https://loremflickr.com/600/600/{generatorSettings.PictureContentScope}/all";
        }

        return new Identity
            (
                gender, 
                firstname, 
                lastname, username, 
                new Identity.Credentials(email, password), 
                picture, 
                new Identity.Location(streetName, streetNumber, postcode, city, country), 
                birthDate,
                userAgent
            );
    }

    public static string GenerateUserAgentAsync()
    {
        var random = new Random();
        var templates = new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/_CHROME-VERSION-ID_ Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/_CHROME-VERSION-ID_ Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_2_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/_CHROME-VERSION-ID_ Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:110.0) Gecko/_FIREFOX-VERSION-ID_ Firefox/110.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13.2; rv:110.0) Gecko/_FIREFOX-VERSION-ID_ Firefox/110.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/_CHROME-VERSION-ID_ Safari/537.36 Edg/_EDGE-VERSION-ID_",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_2_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/_CHROME-VERSION-ID_ Safari/537.36 Edg/_EDGE-VERSION-ID_"
        };

        var template = templates[random.Next(0, templates.Length)];
        var chromeMajor = -1;
        if (template.Contains("_CHROME-VERSION-ID_"))
        {
            chromeMajor = random.Next(98, 110);
            template = template.Replace("_CHROME-VERSION-ID_", $"{chromeMajor}.0.0.0");
        }

        if (template.Contains("_EDGE-VERSION-ID_"))
        {
            template = template.Replace("_EDGE-VERSION-ID_", $"{chromeMajor}.0.{random.Next(1480, 1587)}.{random.Next(35, 56)}");
        }

        if (template.Contains("_FIREFOX-VERSION-ID_"))
        {
            template = template.Replace("_FIREFOX-VERSION-ID_", random.Next(20000000, 21000000).ToString());
        }

        return template;
    }

    [GeneratedRegex("[^a-zA-Z0-9 -]")]
    private static partial Regex WebSafeRegex();

    public static async Task<string> GenerateTempMailAsync()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://www.1secmail.com/api/v1/?action=genRandomMailbox");
        if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase ?? $"Failed: API failure, {response.StatusCode}");
        var email = (string)JArray.Parse(await response.Content.ReadAsStringAsync())[0]!;
        
        return email;
    }
}