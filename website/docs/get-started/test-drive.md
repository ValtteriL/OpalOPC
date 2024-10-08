---
title: Test Drive
description: Information on a test OPC UA target that you can scan with security scanners.
keywords: [scan, test]
sidebar_position: 4
---

## Practice target

We set up a OPC UA server to help folks learn about OpalOPC and also to test and make sure that their OpalOPC installation (or Internet connection) is working properly. You are authorized to scan this machine with OpalOPC or other OPC UA scanners. Try not to hammer on the server too hard. A few scans in a day is fine, but don’t scan 100 times a day.

```text
opc.tcp://scanme.opalopc.com:53530
```

The target server is the [Prosys OPC UA Simulation Server](https://www.prosysopc.com/products/opc-ua-simulation-server/) and it has been intentionally configured with insecure settings.
