# OpalOPC

OPC UA security scanner

## Deploy

Build Windows, OSX and Linux binaries, sign them with GPG key,
create windows installer, and publish on the opalopc.com website
along with linux and mac installer. Update EULA at the same time.

```
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
make run-e2e-tests

# test install.sh
make run-linux-mac-installer
```
