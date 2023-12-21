using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;

namespace AdminOnlyProperty.TestSite.v13.UserImport;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<UmbracoApplicationStartedNotification, CreateEditorUser>();
    }
}
