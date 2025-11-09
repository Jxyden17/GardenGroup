namespace GardenGroup.Models.viewModels
{
    public class LoginViewModel
    {
        public string email { get; set; }
        public string Password { get; set; }

        public LoginViewModel()
        {
        }

        public LoginViewModel(string Email, string password)
        {
            email = Email;
            Password = password;
        }
    }
}