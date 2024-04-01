# OpalOPC

OPC UA security scanner

## Deployment

To deploy means to release applications, and if necessary, to deploy website and API.

### Release applications

```bash
make release
```

This command creates MSIX and Snapd packages of OpalOPC.
The Snapd package is automatically uploaded to Snap store, but need to be promoted to latest/stable by hand in Snapd listing page. MSIX needs to be uploaded by hand to Microsoft Partner Portal.

### Deploy website

Deploy Docusaurus website to Cloudflare Pages.

```bash
make deploy-website
```

Deploy Known Vulnerability API.

### Deploy API

```bash
make update-local-knownvulnerabilityapi-db # create local CPE database using latest data
make deploy-api

# deploy the local CPE database to API
make deploy-knownvulnerabilityapi-db
```


### Setup scanme.opalopc.com

Setup scanme on a VPS. Only needs to be run once.

```bash
make setup-scanme
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

## Misc

### Edit Ansible vault
    
```bash
make edit-vault
```
