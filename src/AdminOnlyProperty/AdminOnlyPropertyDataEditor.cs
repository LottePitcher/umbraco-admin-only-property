using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal sealed class AdminOnlyPropertyDataEditor : IDataEditor
    {
        internal const string DataEditorAlias = "Umbraco.Community.AdminOnlyProperty";
        internal const string DataEditorName = "Admin Only Property";
        internal const string DataEditorViewPath = "readonlyvalue";
        internal const string DataEditorIcon = "icon-shield";

        private readonly IDataTypeService _dataTypeService;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IUserService _userService;
        private readonly PropertyEditorCollection _propertyEditors;

        public AdminOnlyPropertyDataEditor(
            IDataTypeService dataTypeService,
            IIOHelper ioHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IUserService userService,
            PropertyEditorCollection propertyEditors)
        {
            _dataTypeService = dataTypeService;
            _ioHelper = ioHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _userService = userService;
            _propertyEditors = propertyEditors;
        }

        public string Alias => DataEditorAlias;

        public EditorType Type => EditorType.PropertyValue;

        public string Name => DataEditorName;

        public string Icon => DataEditorIcon;

        public string Group => UmbConstants.PropertyEditors.Groups.Common;

        public bool IsDeprecated => false;

        public IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object>();

        public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor()
        {
            return new AdminOnlyPropertyConfigurationEditor(
                _dataTypeService,
                _ioHelper,
                _localizedTextService,
                _userService,
                _propertyEditors);
        }

        public IDataValueEditor GetValueEditor()
        {
            return new DataValueEditor(
                 _localizedTextService,
                 _shortStringHelper,
                 _jsonSerializer)
            {
                ValueType = ValueTypes.Json,
                View = DataEditorViewPath,
            };
        }

        public IDataValueEditor GetValueEditor(object? configuration)
        {
            if (configuration is Dictionary<string, object> config)
            {
                var dataType = _dataTypeService.GetDataTypeFromConfig(config);
                if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor) == true)
                {
                    return dataEditor.GetValueEditor(dataType.Configuration);
                }
            }

            return GetValueEditor();
        }
    }
}
