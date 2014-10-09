using System;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.RestApi.Helpers;

namespace Sitecore.RestApi.Models
{
    public class ItemProfileRepository: GenericRepository
    {
        public ItemProfile Get(string profileName)
        {
            var rootId = SettingsHelper.GetItemProfilesRootId();
            var itemProfilesRoot = Db.GetItem(rootId);

            if (itemProfilesRoot == null) return null;

            var item =
                itemProfilesRoot.Children.SingleOrDefault(
                    n => n["Name"].Equals(profileName, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                throw new Exception(
                    string.Format(
                        "The item profile '{0}' with id {1} could not be found. Please check the setting value for Restful.ItemProfilesRootId.",
                        profileName, rootId));
            }

            var itemProperties = new MultilistField(item.Fields["Item Properties"]);
            var fieldProperties = new MultilistField(item.Fields["Field Properties"]);
            var hiddenFields = new MultilistField(item.Fields["Hidden Fields"]);
            var showFields = new CheckboxField(item.Fields["Show Fields"]);
            var forceCamelCase = new CheckboxField(item.Fields["CamelCase Name"]);
            var valueFormatters = new MultilistField(item.Fields["Value Formatters"]);

            var itemProfile = new ItemProfile
                                    {
                                        Name = item["Name"],
                                        ItemPropertyNames = itemProperties.GetItems().Select(n => n["Name"]),
                                        FieldPropertyNames = fieldProperties.GetItems().Select(n => n["Name"]),
                                        HiddenFieldNames = hiddenFields.GetItems().Select(n => n["Name"]),
                                        ShowFields = showFields.Checked,
                                        CamelCaseName = forceCamelCase.Checked,
                                        ValueFormatters = valueFormatters.GetItems().Select(n => System.Type.GetType(n["Type"])).Where(n => n != null)
                                    };

            return itemProfile;
        }
    }
}