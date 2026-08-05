# Changelog

## [1.3.0] - 2026-08-05
### Changed
- The task now targets .NET 8.
- The task now supports configurable error handling: a new **Options** parameter lets you choose whether failures throw an exception (default) or return a result with `Success = false` and error details in the `Error` property.
- The result object now includes an `Error` property with the error message and exception details when the task fails without throwing.

## [1.2.0] - 2024-11-26
### Changed
- Removed old Handlebars DLL from the libs directory and replaced it with the NuGet package Handlebars.Net version 2.1.6.

## [1.1.0] - 2024-08-20
### Updated
- Updated Newtonsoft.Json library to the latest version 13.0.3.

## [1.0.1] - 2023-03-10
### Changed
- Local reference for Handlebars to fix Frends import issue.

## [1.0.0] - 2023-02-21
### Added
- Initial implementation