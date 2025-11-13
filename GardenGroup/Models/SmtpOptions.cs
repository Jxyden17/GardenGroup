namespace GardenGroup.Models
{
    public class SmtpOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;

        public SmtpOptions(string host, int port, bool enableSsl, string username, string password, string fromAddress)
        {
            Host = host;
            Port = port;
            EnableSsl = enableSsl;
            Username = username;
            Password = password;
            FromAddress = fromAddress;
        }

        public SmtpOptions()
        {
        }
    }
}
