# 04-shared-libraries: Add validated .NET 11 targets to shared libraries

Upgrade the 27 shared libraries in dependency order while retaining `net10.0` compatibility for consumers not yet migrated. The dependency graph ranges from foundation libraries (`Parameters`, `ProjectData`, and `ConfidentialSettings`) through parsing, storage, and scanner integration libraries. The solution assessment reports 90 source-incompatible API findings and 36 behavioral-change findings overall; address source changes inline, check behavior-sensitive `Uri`, XML serialization, configuration, and HTTP content use, and verify package compatibility against the actual target frameworks. Migrate Unity container usage to built-in DI where the projects actively use it.

**Done when**: Every shared library has a compiling `net11.0` target and retains a working `net10.0` target during transition; project references resolve at both TFMs; no source-incompatible API findings remain for the upgraded targets.
