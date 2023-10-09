namespace PlanePal.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendMail(string to, string subject, string body);
    }
}