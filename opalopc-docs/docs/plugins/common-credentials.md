---
id: 10003
title: Common credentials
description: Information on the issue detected by Common credentials security testing plugin.
slug: /plugin-10003
tags: [plugin, authentication]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>High</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10003</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Authentication</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>7.3</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>[https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L](https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:L)</td>
  </tr>
</table>

## Synopsis

The server can be accessed using well-known credentials.

## Description

The server uses default credentials (username & password) for potentially critical functionality. An attacker can easily guess the credentials to bypass authentication and to gain access to the server.

The credentials attempted are listed with sources [here](https://github.com/COMSYS/msf-opcua/blob/a3d4fedf91ca59055a083c8047cdc7a1de3cbe7e/credentials/opcua_credentials_sources.txt).

## Solution

Change or disable default credentials.

## References

* [https://cwe.mitre.org/data/definitions/1392.html](https://cwe.mitre.org/data/definitions/1392.html)
* [https://www.comsys.rwth-aachen.de/fileadmin/papers/2020/2020-roepert-opcua-security.pdf](https://www.comsys.rwth-aachen.de/fileadmin/papers/2020/2020-roepert-opcua-security.pdf)
* [https://en.wikipedia.org/wiki/Default_Credential_vulnerability](https://en.wikipedia.org/wiki/Default_Credential_vulnerability)
