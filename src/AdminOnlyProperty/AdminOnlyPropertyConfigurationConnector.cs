using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal sealed class AdminOnlyPropertyConfigurationConnector : IDataTypeConfigurationConnector
    {
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        
        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
        {
            return ToArtifact(dataType, dependencies);
        }

        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies)
        {
            if (dataType.Configuration is Dictionary<string, object> config &&
                config.TryGetValue(AdminOnlyPropertyConfigurationEditor.DataTypeKey, out var obj1) == true &&
                obj1 is string str1 &&
                UdiParser.TryParse<GuidUdi>(str1, out var udi) == true)
            {
                dependencies.Add(new ArtifactDependency(udi, false, ArtifactDependencyMode.Match));
            }

            return ConfigurationEditor.ToDatabase(dataType.Configuration, _configurationEditorJsonSerializer);
        }
        
        public object? FromArtifact(IDataType dataType, string? configuration, IContextCache contextCache)
        {
            return FromArtifact(dataType, configuration);
        }

        public object? FromArtifact(IDataType dataType, string? configuration)
        {
            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();
            return dataTypeConfigurationEditor?.FromDatabase(configuration, _configurationEditorJsonSerializer);
        }

        public IEnumerable<string> PropertyEditorAliases => new[] { AdminOnlyPropertyDataEditor.DataEditorAlias };

        public AdminOnlyPropertyConfigurationConnector(IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }
    }
}
