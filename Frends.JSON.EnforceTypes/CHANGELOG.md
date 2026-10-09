# Changelog

## [2.1.0] - 2026-10-09
### Changed
- Port task to supported repository

## [1.1.0] - 2026-07-20
### Changed
- The task now targets .NET 8.
- Added an **Options** parameter to the task. You can now choose whether the task throws an error on failure (`ThrowErrorOnFailure`) or returns a result with error details (`ErrorMessageOnFailure`).
- The result now includes a `Success` flag and an `Error` property (containing message and exception details) so you can check the outcome without relying on exceptions.
- The input parameter has been renamed from `EnforceTypesInput` to `Input` to align with Frends task conventions.

## [1.0.1] - 2022-11-02
### Updated
- Updated dependency for Newtonsoft.Json from 13.0.1 to 12.0.1

## [1.0.0] - 2022-04-05
### Added
- Initial implementation
