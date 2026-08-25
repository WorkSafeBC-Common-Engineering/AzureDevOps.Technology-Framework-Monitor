# Dockerfile Python Version Parsing Guide

## Goal

Extract the Python major.minor version used by a Dockerfile for Python EOL compliance analysis.

Store versions in the format:

```text
3.10
3.11
3.12
```

Ignore patch versions when evaluating Python lifecycle status.

---

## Primary Pattern

### Official Python Image

```dockerfile
FROM python:3.11
```

Extract:

```text
3.11
```

---

## Common Variants

### Slim Image

```dockerfile
FROM python:3.12-slim
```

Extract:

```text
3.12
```

### Alpine Image

```dockerfile
FROM python:3.11-alpine
```

Extract:

```text
3.11
```

### Debian / Bookworm Image

```dockerfile
FROM python:3.12-bookworm
```

Extract:

```text
3.12
```

### Full Version Specified

```dockerfile
FROM python:3.11.9
```

Extract:

```text
3.11
```

### Full Version With Variant

```dockerfile
FROM python:3.10.14-alpine
```

Extract:

```text
3.10
```

---

## Multi-Stage Builds

Dockerfiles may contain multiple FROM statements.

Example:

```dockerfile
FROM python:3.12 AS builder

# build steps

FROM python:3.12-slim
```

Extract:

```text
3.12
3.12
```

Recommendation:

- Collect all Python versions found.
- Report inconsistencies if different versions are used.

Example:

```dockerfile
FROM python:3.11 AS builder
FROM python:3.12-slim
```

Extract:

```text
3.11
3.12
```

Flag as inconsistent.

---

## ARG-Based Version Definitions

Example:

```dockerfile
ARG PYTHON_VERSION=3.12
FROM python:${PYTHON_VERSION}
```

Extract:

```text
3.12
```

Another example:

```dockerfile
ARG PYTHON_VERSION=3.11.8
FROM python:${PYTHON_VERSION}-slim
```

Extract:

```text
3.11
```

Recommended behavior:

1. Identify ARG declarations.
2. Resolve variable references.
3. Extract the resulting Python major.minor version.

---

## Ambiguous Cases

### Major Version Only

```dockerfile
FROM python:3
```

Extract:

```text
3
```

Recommendation:

Flag as ambiguous because a minor version is not specified.

---

## Non-Official Images

Examples:

```dockerfile
FROM mcr.microsoft.com/devcontainers/python:3.12
```

```dockerfile
FROM ghcr.io/company/python-runtime:3.11
```

These may contain Python version information, but parsing rules depend on the image naming convention.

Recommendation:

- Parse when the image tag clearly contains a Python version.
- Otherwise report that the runtime version could not be determined with confidence.

---

## Out of Scope (Optional)

A Dockerfile may install Python after the base image is loaded:

```dockerfile
FROM ubuntu:24.04

RUN apt-get install -y python3.11
```

A simple scanner may ignore this case.

An advanced scanner may inspect RUN commands and package installation statements.

---

## Recommended Scanner Output

For:

```dockerfile
ARG PYTHON_VERSION=3.11.8
FROM python:${PYTHON_VERSION}-slim
```

Output:

```text
Source: Dockerfile
Version Found: 3.11
Status: Supported
```

For:

```dockerfile
FROM python:3.11
FROM python:3.12-slim
```

Output:

```text
Source: Dockerfile
Versions Found:
- 3.11
- 3.12

Warning: Multiple Python versions detected.
```
