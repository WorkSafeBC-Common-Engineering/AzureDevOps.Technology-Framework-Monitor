# setup.py Python Version Parsing Guide

## Goal

Extract Python version compatibility information from a `setup.py` file for Python EOL compliance analysis.

---

## Primary Location

Look for the `python_requires` argument passed to `setup()`.

Example:

```python
from setuptools import setup

setup(
    name="myproject",
    python_requires=">=3.11",
)
```

Extract:

```text
>=3.11
```

---

## Common Examples

### Minimum Version

```python
setup(
    python_requires=">=3.10"
)
```

### Version Range

```python
setup(
    python_requires=">=3.10,<4"
)
```

### Exact Version

```python
setup(
    python_requires="==3.11"
)
```

### Wildcard Version

```python
setup(
    python_requires="==3.11.*"
)
```

---

## Dynamic Values

Sometimes the value is assigned to a variable.

```python
PYTHON_REQUIRES = ">=3.11"

setup(
    python_requires=PYTHON_REQUIRES
)
```

A simple scanner may only parse string literals.

An advanced scanner may resolve variables.

---

## What To Parse

Parse:

```python
setup(..., python_requires=..., ...)
```

Ignore unrelated version values such as:

```python
version="1.2.3"
```

which refers to the package version, not the Python version.

---

## Recommended Output

Input:

```python
setup(
    python_requires=">=3.10,<4"
)
```

Output:

```text
Source: setup.py
Constraint: >=3.10,<4
```
