using System.Security.Cryptography;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace AdminOnlyProperty.TestSite.v12.UserImport;

public class CreateEditorUser : INotificationHandler<UmbracoApplicationStartedNotification>
{
    private readonly IUserService _userService;
    private const string EditorEmail = "editor@editor.com";
    private const string EditorPassword = "password1234567890";

    public CreateEditorUser(IUserService userService) => _userService = userService;

    public void Handle(UmbracoApplicationStartedNotification notification)
    {
        IUser? editorExists = _userService.GetByEmail(EditorEmail);

        if (editorExists is not null)
        {
            return;
        }

        // Creating the user as Umbraco does in their tests:
        // https://github.com/umbraco/Umbraco-CMS/blob/8e609af90168a7c5e088b30193266733269457ec/tests/Umbraco.Tests.Integration/Umbraco.Infrastructure/Services/UserServiceTests.cs#L728
        HMACSHA1 hash = new()
        {
            Key = Encoding.Unicode.GetBytes(EditorPassword)
        };

        var encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(EditorPassword)));
        GlobalSettings globalSettings = new();

        User membershipUser = new(
            globalSettings,
            "Editor",
            EditorEmail,
            EditorEmail,
            encodedPassword);

        if(_userService.GetUserGroupByAlias(Umbraco.Cms.Core.Constants.Security.EditorGroupAlias)
           is not UserGroup editorUserGroup)
        {
            return;
        }

        membershipUser.AddGroup(editorUserGroup);

        _userService.Save(membershipUser);
    }
}
