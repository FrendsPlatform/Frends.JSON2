# Changelog

## [1.3.0] - 2026-07-31
### Changed
- Added a `CancellationToken` parameter so that Frends can cancel the task when needed.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options. When `ThrowErrorOnFailure` is set to false, task failures are returned as a result object (with `Success = false` and an `Error` containing the message and exception) instead of raising an exception. The default behaviour (throwing on error) is unchanged.
- The result object now includes an `Error` property that is populated when `ThrowErrorOnFailure` is false.
- Updated target framework from .NET 6 to .NET 8.

## [1.2.0] - 2026-07-08
### Fixed
- Fixed issue where Options.ErrorWhenNotMatched did not throw an exception when a JSONPath filter expression (e.g. `[?(...)]`) matched no results.

## [1.1.0] - 2024-08-20
### Updated
- Updated Newtonsoft.Json library to the latest version 13.0.3.

## [1.0.0] - 2023-02-21
### Added
- Initial implementation