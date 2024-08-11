---
title: CLI Command Reference
description: The CLI Command Reference shows the exhaustive set of command line arguments to the CLI version of OpalOPC, and a couple of usage examples.
sidebar_position: 5
keywords: [reference, cli]
slug: usage-cli
---

```bash
Opal OPC 5.0.0 ( https://opalopc.com )
Usage: opalopc [Options] [Target ...]
  -i, --input-file=VALUE     input targets from list of discovery uris
  -o, --output=VALUE         base name for output reports
  -v                         increase verbosity (can be specified up to 2 times)
  -h, --help                 show this message and exit
  -l, --login-credential=VALUE
                             username:password for user authentication
  -b, --brute-force-credential=VALUE
                             username:password for brute force attack
  -L, --login-credential-file=VALUE
                             import list of username:password for
                               authentication from file
  -B, --brute-force-credential-file=VALUE
                             import list of username:password for brute force
                               attack from file
  -c, --user-certificate-and-privatekey=VALUE
                             path-to-certificate:path-to-privatekey for user
                               authentication
  -a, --application-certificate-and-privatekey=VALUE
                             path-to-certificate:path-to-privatekey for
                               application authentication
  -d, --discovery            discover targets on network through mDNS and exit
      --version              show version and exit
```

## Positional arguments

#### Target

Specify one or more target URIs.

If target URI points to a server, all its endpoints will be scanned.

If target URI points to a Local Discovery Server (LDS) or Global Discovery Server (GDS), all endpoints listed in them will be scanned.

Currently only ocp.tcp scheme is supported.

![OPC URL format](/img/opc-ua-uri-format.png)

## Flags

#### -i, --input-file=VALUE

Specify a file to read OPC UA Discovery URIs from. The file shall have one URI per line.
The special value `-` causes URIs to be read from stdin.

#### -o, --output=VALUE

Provide a base filename for output reports. If not specified, OpalOPC will generate it automatically.
The base name is used to create an HTML report (`.html`) and a SARIF scanning report (`.sarif`).

#### -v

Increase output verbosity. Can be specified multiple times to further increase it.
Currently the maximum verbosity is reached by using this flag 2 times.

#### -h, --help

Show help message and exit.

#### -l, --login-credetial=VALUE

Specify a single `username:password` for user authentication. Can be specified multiple times to add multiple credentials.

#### -b, --brute-force-credential=VALUE

Specify a single `username:password` for user authentication brute force attack. Can be specified multiple times to add multiple credentials.

#### -L, --login-credential-file=VALUE

Specify a file to read multiple `username:password` from for user authentication. The file shall have one credential per line.

#### -B, --brute-force-credential-file=VALUE

Specify a file to read multiple `username:password` from for user authentication brute force attack. The file shall have one credential per line.

#### -c, --user-certificate-and-privatekey=VALUE

Specify `path-to-certificate:path-to-privatekey` for user authentication. The files shall be in PEM format. Can be specified multiple times to add multiple credentials.

#### -a, --application-certificate-and-privatekey=VALUE

Specify `path-to-certificate:path-to-privatekey` for application authentication. The files shall be in PEM format. Can be specified multiple times to add multiple credentials.

#### -d, --discovery

Run network discovery through mDNS and exit. Prints discovered OPC UA Discovery URLs to stdout.

#### --version

Show version and exit.

## Examples

#### Scan single DiscoveryUrl for OPC UA applications

```bash
opalopc opc.tcp://scanme.opalopc.com:53530
```

#### Scan 2 DiscoveryUrls

```bash
opalopc opc.tcp://echo:53530 opc.tcp://foxtrot:48010
```

#### Scan all DiscoveryUrls in file, produce report with custom name

```bash
opalopc -i discoveryuris.txt -o vulnerability-report
```

#### Read targets from stdin and scan them

```bash
opalopc -i -
```

#### Scan with application certificate and credential pair

```bash
opalopc -a /tmp/certificate.pem:/tmp/privkey.pem -l opcadmin:v3rys3cr3t123! opc.tcp://scanme.opalopc.com:53530
```

#### Discover targets on network

```bash
opalopc -d
```

#### Debug output

```bash
opalopc -v opc.tcp://scanme.opalopc.com:53530
```

#### Trace output (most detailed)

```bash
opalopc -vv opc.tcp://scanme.opalopc.com:53530
```
