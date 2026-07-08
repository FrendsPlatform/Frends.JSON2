# Changelog

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