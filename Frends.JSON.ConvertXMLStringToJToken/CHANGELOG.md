# Changelog

## [2.0.0] - 2026-05-27
### Breaking
- The `XSD` parameter has moved from the **Input** tab to the **Options** tab and is now only used (and shown) when `TypeCorrection` is `Schema`.
  - **Why this is breaking:** the XSD is no longer bound where existing process references expect it. A migration (`migration.json`) ships with the package to copy `Input.XSD` → `Options.XSD` and set `TypeCorrection = Schema` automatically, but **tenants that do not apply migration.json will not migrate automatically**. On those tenants the old XSD value is dropped, so XSD-driven array mapping (added in 1.2.0) silently stops working.
  - **Upgrade path (manual, for tenants without migration support):** after updating each affected process step, open the **Options** tab, set `TypeCorrection = Schema`, and paste your XSD into `Options.XSD`. This restores the previous array-mapping behaviour. Steps that never used an XSD need no changes — leave `TypeCorrection = None`.
### Added
- Added `Options` parameter with `TypeCorrection` (`None`/`Attributes`/`Schema`) to convert numeric and boolean XML values into native JSON types, via inline `xsi:type` attributes (`Attributes`) or the supplied XSD (`Schema`). Default is `None`.
### Changed
- In `Schema` mode the XSD now drives both array mapping and value typing. `Schema` mode without an XSD is a graceful no-op (returns string values) rather than an error.

## [1.2.0] - 2026-05-07
### Added
- Added optional XSD input support to ensure consistent array/object mapping 

## [1.1.0] - 2024-08-20
### Updated
- Updated Newtonsoft.Json library to the latest version 13.0.3.

## [1.0.1] - 2023-11-28
### Fixed
- Fixed Return Jtoken type to dynamic.

## [1.0.0] - 2023-02-13
### Added
- Initial implementation
