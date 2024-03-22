---
title: How to discover OPC UA servers on a network
description: Find OPC UA servers with port scanning and mDNS discovery.
keywords: [how-to, nmap, discovery, reconnaissance]
sidebar_position: 3
---

Sometimes you don't know the exact locations of the OPC UA servers on a network.
This makes it difficult to perform a security assessment on them.

This guide will show you how to reliably discover servers in unknown networks through port scanning and mDNS discovery.

:::note

The methods described here will work in **most** cases.
If you have the opportunity, consult with the network administrator to avoid missing anything.

:::

## Prerequisites

- OpalOPC
- [Nmap](https://nmap.org/)

## Port scanning

We will use Nmap to scan the network for open ports.
The standard OPC UA server port is 4840/tcp, but it is common for servers to use other ports.
Scanning for all TCP ports is a good idea if the target network is not too large and you have the time.
Otherwise, you can scan for the most common ports.

Before scanning, we need to configure Nmap to detect OPC UA services.

:::note

When scanning through a lot of ports, you need to go through the results to find the OPC UA servers.
Look for the service name `opc-ua-tcp`.

:::

### Configuring Nmap for OPC UA
```bash
mkdir opcua-nmap-service-probes
cd opcua-nmap-service-probes
curl -LO https://raw.githubusercontent.com/nmap/nmap/5b518af9f22c01dbdbe0e7aadccc33e494ea8f4b/nmap-service-probes
```

Run the scanning command in the directory `opcua-nmap-service-probes` just created.

### Scanning most common OPC UA ports
```bash
nmap -sS -T4 -n -sV --open -Pn --datadir . -p 49320,62541,4897,53530,48050,4885,4840,4855,26543 <host/network>
```

### Scanning all TCP ports

```bash
nmap -sS -T4 -n -sV --open -Pn --datadir . -p- <host/network>
```

## mDNS discovery

mDNS is a protocol that allows devices to discover each other on a local network without a central server.
It is commonly used in IoT devices and can be used to discover OPC UA servers.

OpalOPC has a built-in mDNS discovery tool just for this purpose.
Use it as follows:

```bash
opalopc --discovery
```
