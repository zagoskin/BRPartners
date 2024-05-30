using System.Text.RegularExpressions;

namespace BRP.VendorManagement.API.Controllers.Trasformers;

public class ToKebabParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value is not string controllerName)
        {
            return null;
        }

        return controllerName.Trim() == string.Empty
            ? null
            : Regex.Replace(controllerName, "([a-z])([A-Z])", "$1-$2", RegexOptions.Compiled).ToLower(); // to kebab;
    }    
}
