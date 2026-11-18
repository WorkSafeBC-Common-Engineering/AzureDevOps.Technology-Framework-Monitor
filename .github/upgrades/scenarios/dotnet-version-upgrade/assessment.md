# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v11.0.

Detailed findings live alongside this file in `assessment/`. This page is the index: read it first, then open only the documents you need.

## Table of Contents

- [Executive Summary](#executive-summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Detailed Reports](#detailed-reports)
  - [Projects Relationship Graph](assessment/project-graph.md)
  - [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
  - [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)
  - [Project Details](#project-details)

## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 32 | All require upgrade |
| Total NuGet Packages | 92 | 8 need upgrade |
| Total Code Files | 173 |  |
| Total Code Files with Incidents | 53 |  |
| Total Lines of Code | 15212 |  |
| Total Number of Issues | 178 |  |
| Proposed Target Framework | net11.0 |  |
| Estimated LOC to modify | 126+ | at least 0.8% of codebase |

### Projects Compatibility

| Difficulty | Projects | Percentage |
| :--- | :---: | :---: |
| 🟢 Low | 32 | 100.0% |
| ***Total Projects*** | ***32*** | ***100%*** |

This repository has 32 projects. The per-project compatibility table is in [the project inventory](assessment/projects/index.md).

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 84 | 91.3% |
| ⚠️ Incompatible | 1 | 1.1% |
| 🔄 Upgrade Recommended | 7 | 7.6% |
| ***Total NuGet Packages*** | ***92*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 90 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 36 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 15940 |  |
| ***Total APIs Analyzed*** | ***16066*** |  |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| Legacy Configuration System | 87 | 69.0% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Configuration.ConfigurationManager | 26 | 20.6% | Source Incompatible |
| M:System.Configuration.ConfigurationManager.GetSection(System.String) | 18 | 14.3% | Source Incompatible |
| T:System.Xml.Serialization.XmlSerializer | 16 | 12.7% | Behavioral Change |
| T:System.Uri | 13 | 10.3% | Behavioral Change |
| T:System.Configuration.Configuration | 8 | 6.3% | Source Incompatible |
| P:System.Configuration.ConfigurationManager.AppSettings | 7 | 5.6% | Source Incompatible |
| T:System.Configuration.ConfigurationSection | 4 | 3.2% | Source Incompatible |
| M:System.String.Join(System.String,System.ReadOnlySpan{System.String}) | 3 | 2.4% | Source Incompatible |
| M:System.Uri.#ctor(System.String) | 3 | 2.4% | Behavioral Change |
| P:System.Configuration.Configuration.FilePath | 3 | 2.4% | Source Incompatible |

The table above is the top 10. See [the complete list](assessment/api-issues/most-frequent-api-issues.md) for every affected API.

## Detailed Reports

- [Projects Relationship Graph](assessment/project-graph.md)
- [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
- [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)

### Project Details

- [All 32 projects](assessment/projects/index.md)

