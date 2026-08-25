# .python-version Parsing Guide

## Goal

Extract the Python version specified in a `.python-version` file for Python EOL compliance analysis.

---

## Purpose of the File

The `.python-version` file is commonly used by tools such as `pyenv` to select the Python interpreter version for a project.

Unlike `pyproject.toml`, `setup.cfg`, or `setup.py`, this file typically specifies the actual Python version to use rather than a compatibility range.

---

## Most Common Format

```text
3.12.4
```

Extract:

```text
3.12
```

For EOL evaluation, use the major.minor version.

---

## Major.Minor Format

```text
3.11
```

Extract:

```text
3.11
```

---

## Major Version Only

```text
3
```

Extract:

```text
3
```

Recommendation:

Flag as ambiguous because the minor version is not specified.

---

## Multiple Versions

Some tools allow multiple entries.

Example:

```text
3.12.4
3.11.9
```

Extract:

```text
3.12
3.11
```

Recommendation:

Report all discovered versions.

---

## Named Environments

Occasionally a virtual environment name may appear.

```text
my-project-env
```

or

```text
venv-3.11
```

Recommendation:

Attempt to parse a Python version if one is clearly embedded in the value; otherwise report that a Python version could not be determined.

---

## What To Parse

Parse the contents of the file and extract the first valid Python version encountered.

Examples:

```text
3.12.4      -> 3.12
3.11        -> 3.11
3           -> 3 (ambiguous)
```

Ignore whitespace and trailing newlines.

---

## Recommended Scanner Output

Input:

```text
3.12.4
```

Output:

```text
Source: .python-version
Version Found: 3.12
Status: Supported
```

Input:

```text
3
```

Output:

```text
Source: .python-version
Version Found: 3
Warning: Minor version not specified.
```
