using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
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

        private readonly string[] _defaultUserGroups = new[] { "admin" };

        private const string LocalizationAreaKey = "adminOnlyProperty";

        internal const string DataTypeKey = "dataType";
        internal const string IndicatorKey = "indicator";
        internal const string UserGroupsKey = "userGroups";

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

            _ = DefaultConfiguration.TryAdd(UserGroupsKey, _defaultUserGroups);

            Fields.Add(new ConfigurationField
            {
                Key = UserGroupsKey,
                Name = localizedTextService.Localize(LocalizationAreaKey, "labelUserGroups") ?? "User groups",
                Description = localizedTextService.Localize(LocalizationAreaKey, "descriptionUserGroups") ?? "Select as many user groups as you like!",
                View = "checkboxlist",
                Config = new Dictionary<string, object>
                {
                    { "prevalues", groups },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = DataTypeKey,
                Name = localizedTextService.Localize(LocalizationAreaKey, "labelDataType") ?? "Data type",
                Description = localizedTextService.Localize(LocalizationAreaKey, "descriptionDataType") ?? "The data type to wrap",
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

            Fields.Add(new ConfigurationField
            {
                Key = IndicatorKey,
                Name = localizedTextService.Localize(LocalizationAreaKey, "labelIndicator") ?? "Show indicator?",
                Description = localizedTextService.Localize(LocalizationAreaKey, "descriptionIndicator") ?? "Add '🔓' to the label to indicate the property is restricted to some users",
                View = "boolean"
            });
        }

        public override IDictionary<string, object> ToValueEditor(object? configuration)
        {
            if (configuration is Dictionary<string, object> config)
            {
                if (config.ContainsKey(UserGroupsKey) == false)
                {
                    config.Add(UserGroupsKey, JArray.FromObject(_defaultUserGroups));
                }

                var dataType = AdminOnlyPropertyHelpers.GetInnerDataType(_dataTypeService, config);
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

        public override object? FromConfigurationEditor(IDictionary<string, object?>? editorValues, object? configuration)
        {
            if (editorValues?.TryGetValue(DataTypeKey, out var value) == true && int.TryParse(value?.ToString(), out var id))
            {
                var dataType = _dataTypeService.GetDataType(id);
                if (dataType != null)
                {
                    editorValues[DataTypeKey] = dataType.GetUdi().ToString();
                }
            }

            return base.FromConfigurationEditor(editorValues, configuration);
        }

        public override IDictionary<string, object> ToConfigurationEditor(object? configuration)
        {
            var editorValues = base.ToConfigurationEditor(configuration);

            if (editorValues.TryGetValue(DataTypeKey, out var value) == true && UdiParser.TryParse<GuidUdi>(value.ToString(), out var udi))
            {
                var dataType = _dataTypeService.GetDataType(udi.Guid);
                if (dataType != null)
                {
                    editorValues[DataTypeKey] = dataType.Id.ToString();
                }
            }

            return editorValues;
        }
    }
}
