using System.Reflection;

namespace Sitecore.RestApi.Models
{
    public class FormatterArgs: System.EventArgs
    {
        public System.Type Type { get; set; }
        public MethodInfo Method { get; set; }
    }
}