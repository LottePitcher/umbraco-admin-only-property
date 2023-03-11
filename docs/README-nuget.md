## Admin Only Property for Umbraco

[![Downloads](https://img.shields.io/nuget/dt/Our.Umbraco.Community.AdminOnlyProperty?color=cc9900)](https://www.nuget.org/packages/Our.Umbraco.Community.AdminOnlyProperty/)
[![NuGet](https://img.shields.io/nuget/vpre/Our.Umbraco.Community.AdminOnlyProperty?color=0273B3)](https://www.nuget.org/packages/Our.Umbraco.Community.AdminOnlyProperty)
[![GitHub license](https://img.shields.io/github/license/lottepitcher/umbraco-admin-only-property?color=8AB803)](https://github.com/LottePitcher/umbraco-admin-only-property/blob/develop/LICENSE)

A data type 'wrapper' for the Umbraco backoffice that allows you to hide a property from any user that is not in the specified user group(s).

After installing the package, add a new data type using the 'Admin Only Property' property editor.

Then specify which user group(s) can see the property, and which underlying data type to use.

Unauthorised content editors will have no idea that the property exists.