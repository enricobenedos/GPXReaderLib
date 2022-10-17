
# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.2] - 2022-10-17
### Fixed 

- Exception when some `ele` tags are missing from GPX file

## [2.0.1] - 2021-01-12
### Fixed 

- `README.md` and `CHANGELOG.md` location
- Broken NuGet publish action

## [2.0.0] - 2021-01-11
### Migrated to .NET Standard 2.0 

### Added 

- `GetElevationGain()` in order to obtain elevation gain
- Tests suite/project
- `IGpxReader` interface

### Breaking changes

- Class and method names. Please check the sample in `README.md`

## [1.6.1] - 2020-10-13
### Fixed

- README badges (#3)

### Removed

- Unused action ".NET CORE"

## [1.6.0] - 2020-10-12
### Added

- Missing license file
- New GitHub action to automatically deploy package to Nuget 

## [1.5.0] - 2020-07-25
### Added

- Github actions CI

## [1.4.1] - 2020-07-25
### Fixed

- Exception\wrong value when two latitude\longitude points are equal
- Exception when `Math.Acos()` receive values >1 or <-1. This can happen when running this library on different OS (error reported on [Microsoft Github](https://github.com/dotnet/runtime/issues/39838))

## [1.4.0] - 2020-03-18
### Added

- Now Altimetry object contains also the elevation related kilometer

## [1.3.1] - 2020-03-13
### Fixed

- Updated README.md to show hot to use new implemented method on v1.3.0

## [1.3.0] - 2020-03-13
### Added

- GetGPXAltimetry() method to retrieve a complete altimetry of the trace

## [1.2.0] - 2019-02-01
### Added

- Method that returns a list of latitude and longitudes

### Updated

- README.md

## [1.1.0] - 2019-12-30
### Added

- Distance calculation in kilometers
- Code snippet to use the library

## [1.0.0] - 2019-12-28
### Added

- First implemetation