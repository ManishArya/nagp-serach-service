namespace SearchWebApi.Utils
{
    public static class StringExtension
    {
        public static readonly string[] specialCharacters = {"+", "-", "$", "{", "}", "[", "]",
                "(", ")", ".", "*", "=", "?", "|", "~", ":", "&", "%", "/", "^", "!"};

        public static string EscapeCharacters(this string input)
        {
            foreach (var character in specialCharacters)
            {
                if (input.Contains(character))
                {
                    input = input.Replace(character, "\\" + character);
                }
            }

            return input;
        }

        public static string ReplaceUnwantedCharacter(this string input)
        {
            var toRet = input.Replace("[><]", "");
            return toRet;
        }
    }
}
