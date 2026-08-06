# Changelog

## [1.2.0] - 2026-08-04
### Changed
- The task now targets .NET 8.
- Added error handling options: you can now choose whether the task throws an error on failure or returns a result with `Success = false` and error details in the `Error` property.
- The result now includes an `Error` property with details when the task fails.

## [1.1.0] - 2024-08-20
### Updated
- Updated Newtonsoft.Json library to the latest version 13.0.3.

## [1.0.1] - 2024-02-16
### Fixed
- Fixed dotnotation on Task Jtoken result property by changing the type to dynamic.

## [1.0.0] - 2023-02-20
### Added
- Initial implementation