.PHONY: run
run:
	@dotnet run --project OpalOPC -- opc.tcp://echo:53530

# These come from opcuaserve.com
.PHONY: run-opcuaserver.com-demo
run-opcuaserver.com-demo:
	@dotnet run --project OpalOPC --  opc.tcp://opcuaserver.com:48010 -vv

.PHONY: run-opcuaserver.com-lds
run-opcuaserver.com-lds:
	@dotnet run --project OpalOPC --  opc.tcp://opcuaserver.com:4840 -vv

.PHONY: run-opcuaserver.com-weather
run-opcuaserver.com-weather:
	@dotnet run --project OpalOPC --  opc.tcp://opcuaserver.com:48484 -vv

.PHONY: build
build:
	@dotnet build

.PHONY: server
server:
	@php -S localhost:8000

.PHONY: test
test:
	@dotnet test