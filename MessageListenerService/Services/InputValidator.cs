using System.Text.RegularExpressions;

namespace MessageListenerService.Services
{
    public class InputValidator
    {
        private readonly Regex Pattern = new(@"^(?<key>[A-Za-z0-9_]+):(?<value>.+)$", RegexOptions.Compiled);

        public bool TryValidate(string input, string expectedKey, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(input))
                return false;

            var match = Pattern.Match(input);
            if (!match.Success)
                return false;

            var key = match.Groups["key"].Value;
            if (!key.Equals(expectedKey, StringComparison.OrdinalIgnoreCase))
                return false;

            value = match.Groups["value"].Value;
            return true;
        }
    }
}
