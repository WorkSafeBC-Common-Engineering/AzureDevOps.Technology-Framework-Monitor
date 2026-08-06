# Dockerfile Python Version Parsing Plan

## Summary of Guide

The Dockerfile parsing guide requires:

1. Parse Python versions from `FROM` image tags.
2. Normalize extracted versions to `major.minor` for lifecycle analysis (or `major` when only major is present).
3. Support common image variants like `-slim`, `-alpine`, and `-bookworm`.
4. Handle multi-stage Dockerfiles and flag inconsistent version usage.
5. Resolve `ARG`-based version substitutions used in `FROM` tags.
6. Parse non-official Python images when Python version is clearly inferable.
7. Ignore `RUN apt-get install python...` style runtime installs for now.

## Step-by-Step Plan

1. **Parser behavior alignment**
   1. Parse every valid `FROM` instruction, including alias and multi-stage patterns.
   2. Resolve `ARG` variables used in Python image tags.
   3. Extract and normalize versions to `major.minor` (or `major` only where applicable).

2. **Version selection and consistency logic**
   1. Collect all valid Python version candidates from the Dockerfile.
   2. Select the lowest normalized version as the canonical result.
   3. Set `PythonInconsistentVersion=true` only when normalized versions differ.

3. **Unit test coverage against guide scenarios**
   1. Official image tags and variants (`3.11`, `3.12-slim`, `3.11-alpine`, `3.12-bookworm`).
   2. Patch-version normalization (`3.11.9`, `3.10.14-alpine` -> `3.11`, `3.10`).
   3. Multi-stage same-version and mixed-version inconsistency cases.
   4. ARG-based resolution (`${PYTHON_VERSION}`, `$PYTHON_VERSION`, suffix forms).
   5. Major-only tags, non-Python images, and non-official image cases.

4. **Acceptance criteria**
   1. `PythonVersionDockerfile` stores normalized value as required by guide.
   2. `PythonMajorVersionDockerfile` matches the normalized lifecycle value.
   3. `PythonInconsistentVersion` appears only on conflicting normalized versions.
   4. Dockerfile parser tests pass in `PythonVersionUnitTests`.
