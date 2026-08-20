# Changelog

## [1.4.0] - 2026-08-05
### Updated
- Upgraded target framework from .NET 6 to .NET 8.
- Added `CancellationToken` support to the `QuerySingle` method.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options so that failures can be returned as a result instead of throwing an exception.
- The result now includes an `Error` property with error details when the task fails and `ThrowErrorOnFailure` is set to false.

## [1.3.0] - 2026-07-08
### Fixed
- Fixed issue where Options.ErrorWhenNotMatched did not throw an exception when a JSONPath filter expression (e.g. `[?(...)]`) matched no results.

## [1.2.0] - 2024-11-07
### Fixed
- Fixed issue with result dotnotation by changing the result Data object type to dynamic.

## [1.1.0] - 2024-08-20
### Updated
- Updated Newtonsoft.Json library to the latest version 13.0.3.

## [1.0.0] - 2023-02-21
### Added
- Initial implementation