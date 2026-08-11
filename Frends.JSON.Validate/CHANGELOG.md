# Changelog

## [2.0.0] - 2026-08-11
### Changed
- The task now targets .NET 8.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options: you can now choose whether the task throws an exception or returns a failed result when an error occurs, and optionally provide a custom error message.
- Added a `CancellationToken` parameter to support task cancellation.
- The `Result` object now includes an `Error` property with details when the task fails.
- [Breaking Change] Renamed the `ThrowOnInvalidJson` option to `FailOnInvalidJson` to better reflect its purpose: it controls whether invalid JSON is treated as an error at all, independent of whether that error is thrown or returned (which is controlled by `ThrowErrorOnFailure`). Update any existing task configurations to use `FailOnInvalidJson`.

## [1.0.0] - 2023-06-15
### Added
- Initial implementation