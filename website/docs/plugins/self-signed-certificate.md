---
id: 10010
title: Self signed certificate
description: Information on the issue detected by Self signed certificate security testing plugin.
keywords: [plugin, self-signed certificate, authentication]
slug: /plugin-10010
tags: [plugin, authentication]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Medium</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10010</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Authentication</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>5.4</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>[https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N](https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:L/UI:N/S:U/C:L/I:L/A:N)</td>
  </tr>
</table>

## Synopsis

The server can be accessed using unauthorized applications.

## Description

The server trusts client applications with self-signed certificates. With this setting, application authentication is effectively disabled. Without application authentication, the server can be accessed using unauthorized applications.

## Solution

Only trust certificates signed by a trusted authority.

## References

* [https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/](https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/)
