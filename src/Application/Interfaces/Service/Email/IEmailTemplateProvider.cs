namespace Application.Interfaces.Service.Email
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate();

        string GetTemplateByName(string name, string languageCode = "en");

        string GetButtonTemplate(string url, string text);

        string GetCodeAreaTemplate(string text);

        string GetLinkAreaTemplate(string text);
    }
}