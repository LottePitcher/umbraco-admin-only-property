using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal sealed class AdminOnlyPropertyConfigurationEditor : ConfigurationEditor
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditors;

        internal readonly string[] DefaultUserGroups = new[] { "admin" };
        internal const string UserGroups = "userGroups";

        public AdminOnlyPropertyConfigurationEditor(
            IDataTypeService dataTypeService,
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            IUserService userService,
            PropertyEditorCollection propertyEditors)
            : base()
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;

            var groups = userService
               .GetAllUserGroups()
               .Select(x => new
               {
                   label = x.Name,
                   value = x.Alias,
               });

            _ = DefaultConfiguration.TryAdd(UserGroups, DefaultUserGroups);

            Fields.Add(new ConfigurationField
            {
                Key = UserGroups,
                Name = "User groups",
                Description = "Select as many user groups as you like!",
                View = "checkboxlist",
                Config = new Dictionary<string, object>
                {
                    { "prevalues", groups },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = "dataType",
                Name = localizedTextService.Localize("adminOnlyProperty", "labelDataType") ?? "Data type",
                Description = localizedTextService.Localize("adminOnlyProperty", "descriptionDataType") ?? "The data type to wrap.",
                View = "treepicker",
                Config = new Dictionary<string, object>
                {
                    {"multiPicker", false},
                    {"entityType", nameof(DataType)},
                    {"type", UmbConstants.Applications.Settings},
                    {"treeAlias", UmbConstants.Trees.DataTypes},
                    {"idType", "id"}
                }
            });
        }

        public override IDictionary<string, object> ToValueEditor(object? configuration)
        {
            if (configuration is Dictionary<string, object> config &&
                config.TryGetValue("dataType", out var obj1) == true &&
                obj1 is string str1 &&
                int.TryParse(str1, out var id) == true)
            {
                if (config.ContainsKey(UserGroups) == false)
                {
                    config.Add(UserGroups, DefaultUserGroups);
                }

                var dataType = _dataTypeService.GetDataType(id);
                if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor) == true)
                {
                    var cacheKey = $"__aopConfig";
                    var config2 = dataEditor.GetConfigurationEditor().ToValueEditor(dataType.Configuration);

                    if (config2?.ContainsKey(cacheKey) == false)
                    {
                        config2.Add(cacheKey, config);
                    }

                    return config2!;
                }
            }

            return base.ToValueEditor(configuration);
        }
    }
}
