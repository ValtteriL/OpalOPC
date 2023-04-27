.PHONY: run
run:
	@dotnet run opc.tcp://echo:53530/OPCUA/SimulationServer

.PHONY: build
build:
	@dotnet build
