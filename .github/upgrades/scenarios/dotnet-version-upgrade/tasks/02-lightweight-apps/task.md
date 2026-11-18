# 02-lightweight-apps: Upgrade TfmExport and ReportsRefresh

Retarget `TfmExport` and `ReportsRefresh` to `net11.0`. `TfmExport` has no project dependencies; `ReportsRefresh` references `ConfidentialSettings`, which remains on its current target during the application phase. The assessment reports no API compatibility incidents for these applications, but flags package-version recommendations; verify actual .NET 11 package compatibility rather than applying a suggested downgrade without evidence.

**Done when**: Both applications target `net11.0`, their dependency references resolve, and their projects build successfully with relevant package changes applied.
