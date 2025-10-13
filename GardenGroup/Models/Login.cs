namespace GardenGroup.Models
{
    public class Login
    {
        public string email { get; set; }
        public string Password { get; set; }

        public Login()
        {
        }

        public Login(string Email, string password)
        {
            email = Email;
            Password = password;
        }
    }
}