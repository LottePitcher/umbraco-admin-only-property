using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace Umbraco.Community.AdminOnlyProperty
{
    public sealed class AdminOnlyPropertySendingContentNotificationHandler
        : INotificationHandler<SendingContentNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public AdminOnlyPropertySendingContentNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        public void Handle(SendingContentNotification notification)
        {
            var user = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;

            foreach (var variant in notification.Content.Variants)
            {
                foreach (var tab in variant.Tabs)
                {
                    tab.Properties = tab.Properties.Where(prop =>
                    {
                        var cacheKey = $"__aopConfig";
                        if (prop.PropertyEditor.Alias.InvariantEquals(AdminOnlyPropertyDataEditor.DataEditorAlias) == true &&
                            prop.Config.TryGetValue(cacheKey, out var tmp1) == true &&
                            tmp1 is Dictionary<string, object> config 
                            )
                        {
                            prop.Config.Remove(cacheKey);

                            return user.IsAdmin();
                        }

                        return true;

                    }).ToList();

                    if (tab.Properties.Any() == false)
                    {
                        tab.Type = string.Empty;
                    }
                }

            }
        }
    }
}