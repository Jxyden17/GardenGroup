namespace GardenGroup.Models.viewModels
{
    public class CreateUserViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string City { get; set; } = "";
        public string Role { get; set; } = "User";
        public string Password { get; set; } = "";

        public CreateUserViewModel(string firstName, string lastName, string email, string city, string role, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            City = city;
            Role = role;
            Password = password;
        }

        public CreateUserViewModel()
        {
        }
    }


}