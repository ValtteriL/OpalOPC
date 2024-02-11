---
title: GUI Reference
description: The GUI Reference provides exhaustive instructructions to the OpalOPC GUI.
sidebar_position: 4
keywords: [gui, reference]
slug: usage-gui
---

<iframe width="100%" height="444" src="https://www.youtube-nocookie.com/embed/sZ3j8nrZ1Pc?si=7sFwaQpmcSSyYZmS" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen></iframe>

## Target specification

Specify one or more target URIs. One by one, or in bulk by importing from file.
To import targets from file, you need a file with one URI per line.

If target URI points to a server, all its endpoints will be scanned.
If target URI points to a Local Discovery Server (LDS) or Global Discovery Server (GDS), all endpoints listed in them will be scanned.

Currently only `ocp.tcp` scheme is supported.

![OPC URL format](/img/opc-ua-uri-format.png)

### Example

```text
opc.tcp://scanme.opalopc.com:53530
```

### Network discovery

Select `Network Discovery` to scan the network for available OPC UA servers using mDNS. Discovered servers will be added to the target list.

## Verbosity level

Specify logging verbosity.

## Output

Specify path to write output report to.

If points to directory, will write report with auto-generated name in it.

## Configuration (optional)

### Application authentication

Specify certificates and private keys in PEM format for application authentication.

### User authentication

Specify certificates and private keys in PEM format for certificate-based user authentication.

Specify username and password -pairs for user authentication.

### Brute force

Specify username and password -pairs for user authentication brute force attack.

## Scanning

Selecting `Scan` will start a vulnerability scan against the specified targets.
Scanning will generate log messages to the log textbox as the scan progresses.

An ongoing scan can be canceled. No report is written for canceled scans.

## Opening report

After a successful scan, the resulting report can be opened by selecting `Open Report`.

The report will be opened using your default HTML viewer. If you have not specified the default, you will be prompted for a program.
We recommend using your web browser, as the report is human readable in it.

You can also access the report directly in your filesystem.
