# Admin Only Property for Umbraco

This adds a new data type 'wrapper' to the Umbraco backoffice that allows you to hide a document type property from any user that is not in the Administrators group.

## Installation

Add the package to an existing Umbraco website (v.10.2 or later) from nuget:

`dotnet add package Umbraco.Community.AdminOnlyProperty`

## Configuration

After installing the package you should be able to add a new data type using the 'Admin Only Property'.

Specify which the underlying data type to use. This is an example of an textarea that should visible to (and editable by) Administrators only:

<img width="689" alt="image" src="https://user-images.githubusercontent.com/4716542/191495770-705798bc-14d7-4c33-9e79-80cbee22d2ce.png">
