# setup.cfg Python Version Parsing Guide

## Goal

Extract Python version compatibility information from a `setup.cfg` file for Python EOL compliance analysis.

---

## Primary Location

Look for the `python_requires` setting in the `[options]` section.

Example:

```ini
[options]
python_requires = >=3.11
```

Extract:

```text
>=3.11
```

---

## Common Examples

### Minimum Version

```ini
[options]
python_requires = >=3.10
```

### Version Range

```ini
[options]
python_requires = >=3.10,<4
```

### Exact Version

```ini
[options]
python_requires = ==3.11
```

### Wildcard Version

```ini
[options]
python_requires = ==3.11.*
```

### Excluded Versions

```ini
[options]
python_requires = >=3.10,!=3.11.0,<3.14
```

---

## What To Parse

Parse the value of:

```ini
[options]
python_requires = ...
```

Do not attempt to derive Python versions from dependency lists.

---

## Multiple Occurrences

Normally there should be only one `python_requires` declaration.

If multiple declarations are found, report an inconsistency.

---

## Recommended Output

Input:

```ini
[options]
python_requires = >=3.10,<4
```

Output:

```text
Source: setup.cfg
Constraint: >=3.10,<4
```
