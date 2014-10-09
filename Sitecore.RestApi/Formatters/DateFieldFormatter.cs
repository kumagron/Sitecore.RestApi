using Sitecore.Data.Fields;

namespace Sitecore.RestApi.Formatters
{
    public class DateFieldFormatter : ValueFormatter, IFieldValueFormatter
    {
        public void FormatValue(Field field)
        {
            var regEx = new System.Text.RegularExpressions.Regex("[0-9][0-9][0-9][0-9][0-1][0-9][0-9][0-9]T[0-2][0-3][0-5][0-9][0-5][0-9]");

            if (!regEx.IsMatch(field.Value)) return;

            SetValue(new DateField(field).DateTime);
        }
    }
}