# Changelog

## [1.1.0] - 2026-08-05
### Changed
- The task now targets .NET 8.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options: you can now choose whether the task throws an exception or returns a failed result when an error occurs, and optionally provide a custom error message.
- Added a `CancellationToken` parameter to support task cancellation.
- The `Result` object now includes an `Error` property with details when the task fails.

## [1.0.0] - 2023-06-15
### Added
- Initial implementation