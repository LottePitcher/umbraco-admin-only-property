using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal static class AdminOnlyPropertyHelpers
    {
        public static IDataType? GetInnerDataType(IDataTypeService dataTypeService, Dictionary<string, object> config)
        {
            if (!config.TryGetValue(AdminOnlyPropertyConfigurationEditor.DataTypeKey, out var dataTypeKeyObj)
                || dataTypeKeyObj is not string dataTypeKey)
            {
                return default;
            }

            // NOTE: For backwards-compatibility, the value could either be an `int` or `Udi`.
            // However the `_dataTypeService.GetDataType` doesn't accept a `Udi`, so we'll use the `Guid`.
            if (int.TryParse(dataTypeKey, out var id))
            {
                return dataTypeService.GetDataType(id);
            }

            if (UdiParser.TryParse<GuidUdi>(dataTypeKey, out GuidUdi? udi))
            {
                return dataTypeService.GetDataType(udi.Guid);
            }

            return default;
        }
    }
}
