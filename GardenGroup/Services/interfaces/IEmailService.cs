
namespace GardenGroup.Services.interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBOdy);
    }
}
