using Umbraco.Cms.Core.Manifest;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal class AdminOnlyPropertyManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var assembly = typeof(AdminOnlyPropertyManifestFilter).Assembly;

            manifests.Add(new PackageManifest
            {
                PackageName = AdminOnlyPropertyDataEditor.DataEditorName,
                Version = assembly.GetName()?.Version?.ToString(3) ?? "1.0.0",
                AllowPackageTelemetry = true
            });
        }
    }
}
