using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal sealed class AdminOnlyPropertyValueConverter : PropertyValueConverterBase
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IPublishedContentTypeFactory _publishedContentTypeFactory;

        public AdminOnlyPropertyValueConverter(
            IDataTypeService dataTypeService,
            IPublishedContentTypeFactory publishedContentTypeFactory)
        {
            _dataTypeService = dataTypeService;
            _publishedContentTypeFactory = publishedContentTypeFactory;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias.InvariantEquals(AdminOnlyPropertyDataEditor.DataEditorAlias) == true;

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
            => GetInnerPropertyType(propertyType).ConvertInterToObject(owner, referenceCacheLevel, inter, preview);

        public override object? ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
            => GetInnerPropertyType(propertyType).ConvertInterToXPath(owner, referenceCacheLevel, inter, preview);

        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
            => GetInnerPropertyType(propertyType).ConvertSourceToInter(owner, source, preview);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
            => GetInnerPropertyType(propertyType).CacheLevel;

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
            => GetInnerPropertyType(propertyType).ModelClrType;

        private IPublishedPropertyType GetInnerPropertyType(IPublishedPropertyType propertyType)
        {
            if (propertyType is { ContentType: not null, DataType.Configuration: Dictionary<string, object> config })
            {
                var dataType = _dataTypeService.GetDataTypeFromConfig(config);
                if (dataType?.EditorAlias.InvariantEquals(AdminOnlyPropertyDataEditor.DataEditorAlias) == false)
                {
                    return _publishedContentTypeFactory.CreatePropertyType(
                        propertyType.ContentType,
                        propertyType.Alias,
                        dataType.Id,
                        ContentVariation.Nothing);
                }
            }

            throw new InvalidOperationException($"Data type not configured for the property: {propertyType.DataType.Id}");
        }
    }
}
