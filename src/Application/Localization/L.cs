using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Application.Localization
{
    public class L
    {
        public static string T(string key, string lang = "en")
        {
            try
            {
                var info = new CultureInfo(lang);
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
                var rs = new ResourceManager("Application.Localization.SourceFiles.Resource", Assembly.GetExecutingAssembly());
                return rs.GetString(key) ?? "";
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return "";
            }
        }
    }
}