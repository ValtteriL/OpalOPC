---
title: Overview
description: My document description
sidebar_position: 1
---

# OpalOPC

<img src="/img/opalopc-logo-no-text.png" height="200" align="right"/>

#### [Quickstart](get-started/quick-start.md) | [FAQ](faq.md) | [Security Tests](/docs/category/plugins) | [Changelog](changelog.md) | [Support](mailto:info@opalopc.com)

>OpalOPC is a vulnerability scanner for OPC UA applications that enables anyone to conduct professional-grade security tests against OPC UA servers with minimal effort. It scans OPC UA applications for common misconfigurations and vulnerabilities, providing unmatched visibility into their security posture.

## How it works

On a high level, OpalOPC works as follows:

1. Take a list of OPC UA URIs* as input
2. Discover applications through the URIs
3. Probe each discovered application for misconfigurations and vulnerabilities
4. Generate a report of the findings

*URIs can point to a server, Local Discovery Server (LDS), or Global Discovery Server (GDS)*

![How it works](/img/how-it-works.png)

