
# E2E tests
.PHONY: run-e2e-tests
run-e2e-tests: run-linux-mac-installer-test run-all-known test-report-structure

.PHONY: run-linux-mac-installer-test
run-linux-mac-installer-test:
	@cat OpalOPC/installers/install.sh | sudo ACCEPT_EULA=1 bash

## Run against all known targets, verify result is consistent
oldreport = test-resources/opalopc-report-all-known.xml
tempreport = /tmp/opalopc-report-all-known.xml

.PHONY: run-all-known
run-all-known:
	@dotnet run \
		--runtime linux-x64 \
		--project OpalOPC -- \
		opc.tcp://echo:53530 \
		opc.tcp://golf:53530 \
		opc.tcp://india:53530 \
		opc.tcp://opcuaserver.com:48010 \
		opc.tcp://opcuaserver.com:4840 \
		opc.tcp://thisdoesnotexistsfafasfada:53530 \
		opc.tcp://google.com:443 \
		-vv \
		--output opalopc-report-all-known.xml
	@grep -v -e 'StartTime' -e 'EndTime' -e 'RunStatus' opalopc-report-all-known.xml > $(tempreport)
	@grep -v -e 'StartTime' -e 'EndTime' -e 'RunStatus' $(tempreport) | cmp $(tempreport)
	@rm -v $(tempreport)

## Verify reports are valid according to dtd
.PHONY: test-report-structure
test-report-structure:
	@xmllint --noout --valid opalopc-report-*

# Install locally
run-linux-mac-installer:
	@cat OpalOPC/installers/install.sh | sudo bash

# Build
.PHONY: build
build:
	@dotnet build

# Run locally
.PHONY: run
run:
	@dotnet run --runtime linux-x64 --project OpalOPC -- opc.tcp://echo:53530

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
		--inventory ron, \
		deploy/playbooks/publish.yaml


# Metrics
VAULT_PASSWORD_FILE := "deploy/vault_password"

.PHONY: daily-metrics
daily-metrics:
	@ansible-playbook \
		--vault-password-file $(VAULT_PASSWORD_FILE) \
		-e @deploy/playbooks/vault/dailymetrics-vault.yml \
		deploy/playbooks/dailymetrics.yaml
