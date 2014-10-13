using System.Reflection;
using Sitecore.Data.Fields;

namespace Sitecore.RestApi.Formatters
{
    public class DateFieldFormatter : Formatter, IFieldValueFormatter
    {
        public void FormatValue(object source, PropertyInfo property)
        {
            var regEx = new System.Text.RegularExpressions.Regex("[0-9][0-9][0-9][0-9][0-1][0-9][0-9][0-9]T[0-2][0-3][0-5][0-9][0-5][0-9]");
            var value = property.GetValue(source).ToString();

            if (!regEx.IsMatch(value)) return;

            Set(new DateField(source as Field).DateTime);
        }
    }
}