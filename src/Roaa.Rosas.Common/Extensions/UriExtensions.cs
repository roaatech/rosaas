using System.Text;

namespace Roaa.Rosas.Common.Extensions
{
    public static class UriExtensions
    {
        public static async Task<string> LoadContentAsync(this Uri template)
        {
            string str = "";
            using (HttpClient client = new())
            {
                using var response = await client.GetAsync(template);
                str = await response.Content.ReadAsStringAsync();
            }
            return str;
        }

        public static string ReplaceVariablesInTemplate(this string template, Dictionary<string, string> templateVariabes)
        {
            StringBuilder strBuilder = new(template);
            foreach (var variable in templateVariabes)
            {
                strBuilder = strBuilder.Replace(variable.Key, variable.Value);
            }

            return strBuilder.ToString();
        }
    }
}
