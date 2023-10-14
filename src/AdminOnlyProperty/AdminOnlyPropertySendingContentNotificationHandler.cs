using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.AdminOnlyProperty
{
    public sealed class AdminOnlyPropertySendingContentNotificationHandler
        : INotificationHandler<SendingContentNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IDataTypeService _dataTypeService;

        public AdminOnlyPropertySendingContentNotificationHandler(
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
            IDataTypeService dataTypeService)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _dataTypeService = dataTypeService;
        }

        public void Handle(SendingContentNotification notification)
        {
            var user = _backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;
            if (user != null)
            {
                foreach (var variant in notification.Content.Variants)
                {
                    var tabGroupCount = new Dictionary<string, int>();

                    // 'Tabs' property actually contains both 'Tabs' and 'Groups'
                    foreach (var tab in variant.Tabs)
                    {
                        // Keeps a count of the groups with tabs
                        if (string.IsNullOrWhiteSpace(tab.Alias) == false)
                        {
                            if (tab.Type.InvariantEquals("Tab") == true)
                            {
                                tabGroupCount.TryAdd(tab.Alias, 0);
                            }
                            else if (tab.Type.InvariantEquals("Group") == true)
                            {
                                var idx = tab.Alias.LastIndexOf('/');
                                if (idx > 0)
                                {
                                    var tabAlias = tab.Alias.Substring(0, idx);
                                    if (tabGroupCount.ContainsKey(tabAlias) == false)
                                    {
                                        tabGroupCount.Add(tabAlias, 1);
                                    }
                                    else
                                    {
                                        tabGroupCount[tabAlias]++;
                                    }
                                }
                            }
                        }

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

                                    // set the editor to the inner one since Umbraco uses this for the block list layout, and it must match
                                    prop.Editor = _dataTypeService.GetDataTypeFromConfig(config)?.EditorAlias ?? prop.Editor;
                                }

                                return allowed;
                            }
                            return true;
                        }).ToList();

                        if (tab.Properties.Any() == false)
                        {
                            // Decrement the group count for the tab
                            if (string.IsNullOrWhiteSpace(tab.Alias) == false &&
                                tab.Type.InvariantEquals("Group") == true)
                            {
                                var idx = tab.Alias.LastIndexOf('/');
                                if (idx > 0)
                                {
                                    var tabAlias = tab.Alias?.Substring(0, idx) ?? string.Empty;
                                    if (tabGroupCount.ContainsKey(tabAlias) == true)
                                    {
                                        tabGroupCount[tabAlias]--;
                                    }
                                }
                            }

                            // set Type as Empty so doesn't display
                            tab.Type = string.Empty;
                        }
                    }

                    if (tabGroupCount.Count > 0)
                    {
                        // if a Tab has only Groups and all the properties in those Groups
                        // are now hidden we should hide the Tab too
                        foreach (var tab in variant.Tabs)
                        {
                            // this Tab must have no properties, and no groups to show
                            // so set Type as Empty so doesn't display
                            if (tab?.Properties?.Any() == false &&
                                string.IsNullOrWhiteSpace(tab.Alias) == false &&
                                tabGroupCount.TryGetValue(tab.Alias, out var groupCount) == true &&
                                groupCount == 0)
                            {
                                tab.Type = string.Empty;
                            }
                        }
                    }
                }
            }
        }
    }
}
