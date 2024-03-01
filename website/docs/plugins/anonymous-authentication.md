---
id: 10001
title: Anonymous authentication
description: The server resources can be accessed anonymously.
keywords: [plugin, anonymous authentication, authentication]
slug: /plugin-10001
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
    <td>10001</td>
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

The server resources can be accessed anonymously.

## Description

The target server allows accessing resources using ‘anonymous’ identifier. Usage of this identifier prevents tracing changes of data or configuration back to user. An attacker can use the ‘anonymous’ identifier to read and write data in an unauthorized manner.

## Solution

Block anonymous authentication, or restrict anonymous user access to only non-critical UA server resources.

## References

* [https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/](https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/)
