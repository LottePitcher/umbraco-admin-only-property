using Newtonsoft.Json.Linq;
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
            var user = _backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;
            if (user != null)
            {
                foreach (var variant in notification.Content.Variants)
                {
                    // 'Tabs' property actually contains both 'Tabs' and 'Groups'
                    foreach (var tab in variant.Tabs)
                    {
                        // Tabs might have only Groups, no properties themselves
                        if (tab?.Properties == null || tab.Properties.Any() == false)
                        {
                            continue;
                        }

                        // remove any Admin Only properties for which the user does not have the appropriate access
                        tab.Properties = tab.Properties.Where(prop =>
                        {
                            var cacheKey = $"__aopConfig";
                            if (prop?.PropertyEditor?.Alias.InvariantEquals(AdminOnlyPropertyDataEditor.DataEditorAlias) == true &&
                                prop?.ConfigNullable.TryGetValue(cacheKey, out var tmp1) == true &&
                                tmp1 is Dictionary<string, object> config &&
                                config.TryGetValue(AdminOnlyPropertyConfigurationEditor.UserGroupsKey, out var tmp2) == true &&
                                tmp2 is JArray array1 &&
                                array1.Count > 0)
                            {
                                prop.ConfigNullable.Remove(cacheKey);

                                var allowedGroups = array1.ToObject<string[]>();

                                var allowed = user.Groups.Any(x => allowedGroups?.Contains(x.Alias) == true);
                                if (allowed)
                                {
                                    // data type might be configured to show the indicator on the label
                                    // option is a checkbox/toggle so will be set to '1' if the indicator should be shown
                                    if (config.TryGetValue(AdminOnlyPropertyConfigurationEditor.IndicatorKey, out var tmp3) == true &&
                                        tmp3.ToString() == "1")
                                    {
                                        prop.Label = "🔓 " + prop.Label;
                                    }
                                }
                                return allowed;
                            }

                            return true;

                        }).ToList();

                        if (tab.Properties.Any() == false)
                        {
                            // set Type as Empty so doesn't display
                            tab.Type = string.Empty;
                        }
                    }
                }
            }
        }
    }
}
