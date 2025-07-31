namespace Application.Interfaces.Service.Email
{
    public interface IEmailSender
    {
        void Send(string from, string to, string subject, string html);

        Task SendAsync(string from, string to, string subject, string html);
    }
}