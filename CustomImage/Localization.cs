using Language;
using System.Reflection;
using Newtonsoft.Json;
namespace CustomImage
{
    public static class Localization
    {
        private static string ToIdentifer(this LanguageCode code) => code.ToString().ToLower().Replace('_', '-');
        public static string CurrLang => Language.Language.CurrentLanguage().ToIdentifer();
        public static Dictionary<string, string> Translation = JudgeandWrite();
        private static Dictionary<string, string> JudgeandWrite()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach(string res in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(t=>t.EndsWith("json")))
            { 
                using(Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream(res))
                {
                    using StreamReader reader = new(stream) ;
                    string content=reader.ReadToEnd();
                    dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(content)!;
                }
            }
            return dic;
        }
        public static string Localize(this string key)
        {
            if(CurrLang.Equals("zh"))
            {
                if(Translation.TryGetValue(key, out string? value))
                {
                    return value;
                }

            }
            return key;
        }
    }
    
}
