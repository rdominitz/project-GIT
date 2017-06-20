using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    class InputTester
    {
        public static bool isValidInput(List<string> input)
        {
            List<string> matches = input.Where(s => s != null && !s.Equals("null") && !s.Equals("")).ToList();
            return input.Count == matches.Count;
        }

        public static bool isLegalPassword(string password)
        {
            return isValidInput(new List<string>() { password }) && 5 <= password.Length && password.Length <= 15 && password.All(c => Char.IsLetterOrDigit(c));
        }

        public static bool isLegalEmail(string email)
        {
            return email != null && Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase);
        }
    }
}
