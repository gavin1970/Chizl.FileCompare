# Security Policy

## Supported Versions

All library are always set to target netstandard2.0 and netstandard2.1.  This means it works for .netstandard, net6.0+, .NetFamework4.6+.

Versions are based on release date/time of build<br/>
Examlpe: 5.08.22.1140<br/>
Year "5" = (2025-2020)<br/>
Month "08"<br/>
Day "22"<br/>
Minutes "1140" = since midnight, 7PM UTC<br/>

This script is added to the prebuild event for desktop demos, then copy the info for netstandard libraries.
```csharp
if $(ConfigurationName) == Release (
  C:\Code\~VBS\setVersionByDate.vbs "$(ProjectDir)" T
)
```

| Version | Supported          |
| ------- | ------------------ |
| 5.8.x   | :white_check_mark: |
| < 5.8   | :x:                |

## Reporting a Vulnerability

Until which time I get a issue process up and working, open a discussion and I can work from there.
