# Admin Only Property Editor for Umbraco

[![Downloads](https://img.shields.io/nuget/dt/Our.Umbraco.Community.AdminOnlyProperty?color=cc9900)](https://www.nuget.org/packages/Our.Umbraco.Community.AdminOnlyProperty/)
[![NuGet](https://img.shields.io/nuget/vpre/Our.Umbraco.Community.AdminOnlyProperty?color=0273B3)](https://www.nuget.org/packages/Our.Umbraco.Community.AdminOnlyProperty)
[![GitHub license](https://img.shields.io/github/license/lottepitcher/umbraco-admin-only-property?color=8AB803)](../LICENSE)

This package adds a new data type 'wrapper' to the Umbraco backoffice that allows you to hide a document type property from any user that is not in the specified user group(s).

## Installation

Add the package to an existing Umbraco website (v10.2 or later) from Nuget:

`dotnet add package Our.Umbraco.Community.AdminOnlyProperty`

## Configuration

After installing the package, add a new data type using the 'Admin Only Property' property editor.

Select which user group(s) can see the property (default is 'Administrators'), and which underlying data type to use.

In the following example a textstring property will only be visible to (and therefore only editable by) Administrators. Unauthorised content editors will have no idea that the property exists.

<img width="750" alt="Data type config" src="https://github.com/LottePitcher/umbraco-admin-only-property/blob/develop/docs/screenshots/data-type-config.png">

If you would like authorised content editors to see an indicator that the property is hidden from some users then tick 'Show indicator?'. When this is ticked an unlocked padlock emoji will be displayed alongside the label, for example:

<img width="750" alt="Document type indicator" src="https://github.com/LottePitcher/umbraco-admin-only-property/blob/develop/docs/screenshots/indicator-textstring.png">

## Known Issues

The package does not currently support being used to 'wrap' the following data types:
- Block list - see issue [#19](https://github.com/LottePitcher/umbraco-admin-only-property/issues/19)

## Contributing

Contributions to this package are most welcome! Please read the [Contributing Guidelines](CONTRIBUTING.md).

## Acknowledgements

### Logo

The package logo uses the [Shield](https://thenounproject.com/icon/shield-5206781/) (by [Kholifah](https://thenounproject.com/vinadbumi/)) icon from the [Noun Project](https://thenounproject.com), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).