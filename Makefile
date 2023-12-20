VAULT_PASSWORD_FILE := "vault_password"

# E2E tests
.PHONY: run-e2e-tests
run-e2e-tests: run-all-known verify-no-fail-on-echo-golf-india

.PHONY: run-all-known
run-all-known:
	@dotnet run \
        --accepteula \
		--runtime linux-x64 \
		--project OpalOPC -- \
		opc.tcp://echo:53530 \
		opc.tcp://golf:53530 \
		opc.tcp://india:53530 \
		opc.tcp://scanme.opalopc.com:53530 \
		opc.tcp://opcuaserver.com:48010 \
		opc.tcp://opcuaserver.com:4840 \
		opc.tcp://thisdoesnotexistsfafasfada:53530 \
		opc.tcp://google.com:443 \
		-vv \
		--output opalopc-report-all-known.html

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

# Run locally
.PHONY: verify-no-fail-on-echo-golf-india
verify-no-fail-on-echo-golf-india:
	@! dotnet run --runtime linux-x64 --project OpalOPC -- -vvv --accepteula opc.tcp://echo:53530 opc.tcp://golf:53530 opc.tcp://india:53530 | grep -i -e "fail"

# Server for serving XSL on localhost, for development (with run)
.PHONY: server
server:
	@php -S localhost:8000

# Unit tests
.PHONY: test
test:
	@cd OpalOPC.Tests && dotnet test

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
