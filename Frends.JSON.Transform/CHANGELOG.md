# Changelog

## [1.2.0] - 2026-10-09
### Changed
- Port task to supported repository

## [1.1.0] - 2026-07-20
### Changed
- The task now targets .NET 8.
- Added an **Options** parameter that lets you control error handling: set *Throw error on failure* to `false` to receive errors as a result value instead of an exception. The result now includes `Success` and `Error` fields.
- Added cancellation support so the task can be stopped when running in a Frends process.

## [1.0.2] - 2022-11-2
### Fixed
- Updated dependency Newtonsoft.Json from 13.0.1 to 12.0.1
- Change JUST.NETcore to JUST.NET version 4.2.0

## [1.0.1] - 2022-10-31
### Fixed
- Changed the Result JToken to be dynamic to enable dot notation
- Changed input parameter field default DataFormatString to Json
- Updated documentation
- Fixed tests

## [1.0.0] - 2022-04-06
### Added
- Initial implementation
