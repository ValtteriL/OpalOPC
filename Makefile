VAULT_PASSWORD_FILE := "vault_password"

# E2E tests
.PHONY: run-e2e-tests
run-e2e-tests:
	@cd OpalOPC.Tests && dotnet test --filter Category=E2E

# Unit tests
.PHONY: run-unit-tests
run-unit-tests:
	@cd OpalOPC.Tests && dotnet test --filter Category!=E2E

# All tests
.PHONY: run-all-tests
run-all-tests: run-unit-tests run-e2e-tests

# Lint (fix format)
.PHONY: lint
lint:
	@dotnet format

# Lint (check format, dont make changes)
.PHONY: lint-check
lint-check:
	@dotnet format --verify-no-changes

# Build
.PHONY: build
build:
	@dotnet build

# Run locally
.PHONY: run
run:
	@dotnet run --runtime linux-x64 --project OpalOPC -- --accepteula opc.tcp://echo:53530

# Server for serving XSL on localhost, for development (with run)
.PHONY: server
server:
	@php -S localhost:8000

# Deployment
.PHONY: publish-all
publish-all:
	@export DOTNET_CLI_ENABLE_PUBLISH_RELEASE_FOR_SOLUTIONS=1
	@ansible-playbook \
		--vault-password-file "$(VAULT_PASSWORD_FILE)" \
		--inventory ron, \
		deploy/playbooks/publish.yaml

.PHONY: setup-snap-builder
setup-snap-builder:
	@ansible-playbook \
		--vault-password-file "$(VAULT_PASSWORD_FILE)" \
		--inventory deploy/inventory.yaml \
		deploy/playbooks/setup-opalopc-snap-builder.yaml
