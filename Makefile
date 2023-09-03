.PHONY: run
run:
	@dotnet run --runtime linux-x64 --project OpalOPC -- opc.tcp://echo:53530

# These come from opcuaserve.com
.PHONY: run-opcuaserver.com-demo
run-opcuaserver.com-demo:
	@dotnet run --runtime linux-x64 --project OpalOPC --  opc.tcp://opcuaserver.com:48010 -vv

.PHONY: run-opcuaserver.com-lds
run-opcuaserver.com-lds:
	@dotnet run --runtime linux-x64 --project OpalOPC --  opc.tcp://opcuaserver.com:4840 -vv

.PHONY: run-opcuaserver.com-weather
run-opcuaserver.com-weather:
	@dotnet run --runtime linux-x64 --project OpalOPC --  opc.tcp://opcuaserver.com:48484 -vv

.PHONY: run-linux-mac-installer-test
run-linux-mac-installer-test:
	@cat OpalOPC/installers/install.sh | sudo ACCEPT_EULA=1 bash

.PHONY: run-linux-mac-installer
run-linux-mac-installer:
	@cat OpalOPC/installers/install.sh | sudo bash

.PHONY: build
build:
	@dotnet build

.PHONY: server
server:
	@php -S localhost:8000

.PHONY: test
test:
	@cd OpalOPC.Tests && dotnet test

.PHONY: publish-all
publish-all:
	@export DOTNET_CLI_ENABLE_PUBLISH_RELEASE_FOR_SOLUTIONS=1
	@ansible-playbook \
		--inventory ron, \
		deploy/playbooks/publish.yaml
