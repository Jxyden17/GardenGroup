namespace GardenGroup.Models.viewModels
{
    public class DeleteUserViewModel
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Role { get; set; } = null!;

        public DeleteUserViewModel(string id, string firstName, string lastName, string email, string phoneNumber, string city, string role)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            City = city;
            Role = role;
        }

        public DeleteUserViewModel()
        {
        }
    }
}
