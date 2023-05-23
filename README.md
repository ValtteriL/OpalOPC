# OpalOPC

OPC UA security scanner

## Deploy

Build Windows, OSX and Linux binaries, sign them with GPG key, and publish on the opalopc.com website

```
export VERSION=<version>
make publish-all
```

## Develop
```
make run
make server
```

## Test

```
# unit test
make test

# e2e tests
make run-opcuaserver.com-demo
make run-opcuaserver.com-lds
make run-opcuaserver.com-weather
```

