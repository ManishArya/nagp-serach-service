namespace SearchWebApi.Utils
{
    public static class StringExtension
    {
        public static readonly String[] specialCharacters = {"+", "-", "$", "{", "}", "[", "]",
                "(", ")", ".", "*", "=", "?", "|", "~", ":", "&", "%"
                , "/", "^", "!"};
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
            string toRet = "";
            if (!string.IsNullOrEmpty(input))
            {
                toRet = input.Replace("[><]", "");
            }
            return toRet;
        }
    }
}
