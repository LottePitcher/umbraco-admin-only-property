# Admin Only Property for Umbraco

[![GitHub license](https://img.shields.io/github/license/lottepitcher/umbraco-admin-only-property)](LICENSE)
[![NuGet](https://img.shields.io/nuget/vpre/Our.Umbraco.Community.AdminOnlyProperty.svg)](https://www.nuget.org/packages/Our.Umbraco.Community.AdminOnlyProperty)

This package adds a new data type 'wrapper' to the Umbraco backoffice that allows you to hide a document type property from any user that is not in the Administrators group.

## Installation

Add the package to an existing Umbraco website (v10.2 or later) from Nuget:

`dotnet add package Our.Umbraco.Community.AdminOnlyProperty`

## Configuration

After installing the package, add a new data type using the 'Admin Only Property' property editor.

Then specify which the underlying data type to use. This is an example of a textarea: 

<img width="689" alt="image" src="https://user-images.githubusercontent.com/4716542/191495770-705798bc-14d7-4c33-9e79-80cbee22d2ce.png">

The property will only be visible (and therefore editable by) Administrators only.

## Acknowledgements

### Logo

The package logo uses the [Shield](https://thenounproject.com/icon/shield-5206781/) (by [Kholifah](https://thenounproject.com/vinadbumi/)) icon from the [Noun Project](https://thenounproject.com), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).