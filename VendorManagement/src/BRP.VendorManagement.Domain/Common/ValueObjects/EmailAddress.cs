using System.Globalization;
using System.Text.RegularExpressions;
using BRP.VendorManagement.Domain.Common.Models;

namespace BRP.VendorManagement.Domain.Common.ValueObjects;
public sealed class EmailAddress : ValueObject
{
    public string Value { get; } = null!;

    public EmailAddress(string value)
    {
        if (!IsValidEmail(value))
        {
            throw new ArgumentException("Invalid email address", nameof(value));
        }
        Value = value.Trim().ToLower();
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {            
            email = Regex.Replace(
                email, @"(@)(.+)$",
                DomainMapper,
                RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(200));
            
            static string DomainMapper(Match match)
            {                
                var idn = new IdnMapping();                
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
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
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
