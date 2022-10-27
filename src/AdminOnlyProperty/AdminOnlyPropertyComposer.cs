using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
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
}
