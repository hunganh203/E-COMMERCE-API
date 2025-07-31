using Application.Interfaces.Service.Email;
using System.Collections.Concurrent;
using System.Text;

namespace Cloud.Service.Email
{
    public class EmailTemplateProvider : IEmailTemplateProvider
    {
        private readonly ConcurrentDictionary<string, string> _defaultTemplates;

        private readonly string _nameSpace = "Cloud.Service.Email.EmailTemplates";

        public EmailTemplateProvider()
        {
            _defaultTemplates = new ConcurrentDictionary<string, string>();
        }

        public string GetDefaultTemplate()
        {
            var tenancyKey = "host";

            return _defaultTemplates.GetOrAdd(tenancyKey, key =>
            {
                var assembly = typeof(EmailTemplateProvider).Assembly;
                using var stream = assembly.GetManifestResourceStream("Cloud.Service.Email.EmailTemplates.default.html");
                byte[] bytes;

                using (var streamReader = new MemoryStream())
                {
                    stream?.CopyTo(streamReader);
                    bytes = streamReader.ToArray();
                }

                var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
                return template.Replace("{EMAIL_LOGO_URL}", "");
            });
        }

        public string GetTemplateByName(string name, string languageCode = "en")
        {
            return _defaultTemplates.GetOrAdd(name, key =>
            {
                var assembly = typeof(EmailTemplateProvider).Assembly;
                using var stream = assembly.GetManifestResourceStream($"{_nameSpace}.{languageCode}.{name}.html");
                byte[] bytes;

                using (var streamReader = new MemoryStream())
                {
                    stream?.CopyTo(streamReader);
                    bytes = streamReader.ToArray();
                }

                var template = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                return template;
            });
        }

        public string GetButtonTemplate(string url, string text)
        {
            var assembly = typeof(EmailTemplateProvider).Assembly;
            using var stream = assembly.GetManifestResourceStream("Cloud.Service.Email.EmailTemplates.button.html");
            byte[] bytes;

            using (var streamReader = new MemoryStream())
            {
                stream?.CopyTo(streamReader);
                bytes = streamReader.ToArray();
            }

            var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            template = template.Replace("{URL}", url);
            return template.Replace("{TEXT}", text);
        }

        public string GetCodeAreaTemplate(string text)
        {
            var assembly = typeof(EmailTemplateProvider).Assembly;
            using var stream = assembly.GetManifestResourceStream("Cloud.Service.Email.EmailTemplates.reset-code-area.html");
            byte[] bytes;

            using (var streamReader = new MemoryStream())
            {
                stream?.CopyTo(streamReader);
                bytes = streamReader.ToArray();
            }

            var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            return template.Replace("{TEXT}", text);
        }

        public string GetLinkAreaTemplate(string link)
        {
            var assembly = typeof(EmailTemplateProvider).Assembly;
            using var stream = assembly.GetManifestResourceStream("Cloud.Service.Email.EmailTemplates.link-area.html");
            byte[] bytes;

            using (var streamReader = new MemoryStream())
            {
                stream?.CopyTo(streamReader);
                bytes = streamReader.ToArray();
            }

            var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            return template.Replace("{LINK}", link);
        }
    }
}