using System.Reflection;

namespace Sitecore.RestApi.Models
{
    public class Formatter: System.EventArgs
    {
        public object Source { get; set; }
        public MethodInfo Method { get; set; }
    }
}