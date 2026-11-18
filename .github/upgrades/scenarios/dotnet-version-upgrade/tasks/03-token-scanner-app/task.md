# 03-token-scanner-app: Upgrade TfmScanWithToken

Retarget `TfmScanWithToken` to `net11.0` after its smaller application dependencies have been checked. It references 19 projects and the assessment identifies six source-incompatible API findings for this application, plus package recommendations. Resolve the API and package issues inline, preserve the pre-existing project-file edits, and validate the application against its current library targets.

**Done when**: `TfmScanWithToken` targets `net11.0`, all 19 project references resolve, and the application builds with its reported API and package issues addressed.
