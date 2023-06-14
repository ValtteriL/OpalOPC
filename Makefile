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

.PHONY: build
build:
	@dotnet build

.PHONY: server
server:
	@php -S localhost:8000

.PHONY: test
test:
	@dotnet test

.PHONY: publish-all
publish-all:
	@export DOTNET_CLI_ENABLE_PUBLISH_RELEASE_FOR_SOLUTIONS=1
	@ansible-playbook \
		--inventory ron, \
		deploy/playbooks/publish.yaml
