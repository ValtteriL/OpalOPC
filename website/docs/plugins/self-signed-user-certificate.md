---
id: 10016
title: Self signed user certificate
description: Information on the issue detected by Self signed user certificate security testing plugin.
keywords: [plugin, self-signed user certificate, authentication]
slug: /plugin-10016
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
    <td>10016</td>
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

The server can be accessed using unauthorized users.

## Description

The server trusts users applications with self-signed certificates. With this setting, user authentication is effectively disabled. Without user authentication, the server can be accessed by unauthorized personnel.

## Solution

Only trust user certificates signed by a trusted authority.

## References

* [https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/](https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/)
