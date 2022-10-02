using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Community.AdminOnlyProperty
{
    internal class AdminOnlyPropertyComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ManifestFilters().Append<AdminOnlyPropertyManifestFilter>();
            builder.AddNotificationHandler<SendingContentNotification, AdminOnlyPropertySendingContentNotificationHandler>();
        }
    }

    internal class AdminOnlyPropertyManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var assembly = typeof(AdminOnlyPropertyManifestFilter).Assembly;

            manifests.Add(new PackageManifest
            {
                PackageName = AdminOnlyPropertyDataEditor.DataEditorName,
                Version = assembly.GetName().Version.ToString(3),
                AllowPackageTelemetry = true
            });
        }
    }
}
