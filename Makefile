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

# Run locally
.PHONY: run
run:
	@dotnet run --runtime linux-x64 --project OpalOPC -- opc.tcp://echo:53530

# Deployment
.PHONY: release
release:
	@export DOTNET_CLI_ENABLE_PUBLISH_RELEASE_FOR_SOLUTIONS=1
	@ansible-playbook \
		--vault-password-file "$(VAULT_PASSWORD_FILE)" \
		--inventory deploy/inventory.yaml \
		deploy/playbooks/release.yaml

.PHONY: setup-snap-builder
setup-snap-builder:
	@ansible-playbook \
		--vault-password-file "$(VAULT_PASSWORD_FILE)" \
		--inventory deploy/inventory.yaml \
		deploy/playbooks/setup-opalopc-snap-builder.yaml

.PHONY: docs
docs:
	@cd website && npx docusaurus start

.PHONY: deploy-website
deploy-website:
	@ansible-playbook \
		--inventory deploy/inventory.yaml \
		deploy/playbooks/deploy-website.yaml

.PHONY: list-website-deployments
list-website-deployments:
	@npx wrangler pages deployment list --project-name opalopc

.PHONY: deploy-api
deploy-api:
	@ansible-playbook \
		--vault-password-file "$(VAULT_PASSWORD_FILE)" \
		--inventory deploy/inventory.yaml \
		deploy/playbooks/deploy-api.yaml

.PHONY: setup-scanme
setup-scanme:
	@ansible-playbook \
		deploy/playbooks/setup-scanme.yaml \
		--inventory deploy/inventory.yaml \
		--vault-password-file "$(VAULT_PASSWORD_FILE)"

.PHONY: edit-vault
edit-vault:
	@ansible-vault \
		edit \
		./deploy/playbooks/vault.yaml \
		--vault-password-file "$(VAULT_PASSWORD_FILE)"

.PHONY: delete-tag
delete-tag:
	@ansible-playbook \
		deploy/playbooks/delete-tag.yaml

.PHONY: update-local-knownvulnerabilityapi-db
update-local-knownvulnerabilityapi-db:
	@ansible-playbook \
		deploy/playbooks/update-local-knownvulnerabilityapi-db.yaml

.PHONY: deploy-knownvulnerabilityapi-db
deploy-knownvulnerabilityapi-db:
	@ansible-playbook \
		deploy/playbooks/deploy-knownvulnerabilityapi-db.yaml \
		--inventory deploy/inventory.yaml \
		--vault-password-file "$(VAULT_PASSWORD_FILE)"
