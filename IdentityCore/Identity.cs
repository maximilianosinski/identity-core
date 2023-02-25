namespace IdentityCore;

public class Identity
{
    public Gender Gender { get; }
    public string Firstname { get; }
    public string Lastname { get; }
    public string Username { get; }
    public Credentials LoginCredentials { get; }
    public string PictureUrl { get; }
    public Location LocationInformation { get; }
    public DateTime BirthDate { get; }
    public string UserAgent { get; }

    public Identity(Gender gender, string firstname, string lastname, string username, Credentials loginCredentials, string pictureUrl, Location locationInformation, DateTime birthDate, string userAgent)
    {
        Gender = gender;
        Firstname = firstname;
        Lastname = lastname;
        Username = username;
        LoginCredentials = loginCredentials;
        PictureUrl = pictureUrl;
        LocationInformation = locationInformation;
        BirthDate = birthDate;
        UserAgent = userAgent;
    }
    
    public class Credentials
    {
        public string Email { get; }
        public string Password { get; }

        public Credentials(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class Location
    {
        public string StreetName { get; }
        public int StreetNumber { get; }
        public string Postcode { get; }
        public string City { get; }
        public string Country { get; }

        public Location(string streetName, int streetNumber, string postcode, string city, string country)
        {
            StreetName = streetName;
            StreetNumber = streetNumber;
            Postcode = postcode;
            City = city;
            Country = country;
        }
    }
}