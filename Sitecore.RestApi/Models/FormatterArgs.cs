using System.Reflection;

namespace Sitecore.RestApi.Models
{
    public class FormatterArgs: System.EventArgs
    {
        public object Source { get; set; }
        public MethodInfo Method { get; set; }
    }
}