using Sitecore.Data.Fields;

namespace Sitecore.RestApi.Formatters
{
    public interface IFieldValueFormatter
    {
        void FormatValue(Field source); 
    }
}