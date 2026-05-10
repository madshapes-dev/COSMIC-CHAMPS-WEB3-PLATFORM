using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Char;

namespace ThirdParty.Extensions
{
    public static class StringExtensions
    {
        /*private const string EmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" +
                                            "@" +
                                            @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
        // @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public static string ToSnakeCase (this string str)
        {
            if (string.IsNullOrEmpty (str))
                return string.Empty;

            var builder = new StringBuilder ();
            var newString = builder.Append (ToLower (str[0]));
            for (var i = 1; i < str.Length; i++)
            {
                var c = str[i];
                if (IsUpper (c))
                    builder.Append ('_');

                builder.Append (ToLower (c));
            }

            return newString.ToString ();
        }

        public static bool IsValidEmail (this string str) => Regex.IsMatch (str, EmailPattern);*/

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(
                    email,
                    @"(@)(.+)$",
                    DomainMapper,
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            } catch (RegexMatchTimeoutException e)
            {
                return false;
            } catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(250));
            } catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}