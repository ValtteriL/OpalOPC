# OpalOPC

OPC UA security scanner

## Deployment

### Release applications

```bash
make release
```

This command creates MSIX and Snapd packages of OpalOPC.
The Snapd package is automatically uploaded to Snap store, but MSIX needs to be uploaded by hand to Microsoft Partner Portal.

### Update website

```bash
make deploy-website
```

## Development

Scan echo (on linux):

```bash
make eun
```

Run website on localhost with hot reload on changes:

```bash
make docs
```

Formatting:

```bash
# check
make lint-check

# make changes
make lint
```

## Testing

```bash
# unit tests
make run-unit-tests

# e2e tests (requires Windows)
make run-e2e-tests

# All tests
make run-all-tests
```
