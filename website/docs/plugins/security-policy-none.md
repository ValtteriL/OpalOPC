---
id: 10009
title: Security policy None
description: Information on the issue detected by Security policy None security testing plugin.
keywords: [plugin, security policy none, transport-security]
slug: /plugin-10009
tags: [plugin, transport-security]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Medium</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10009</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Transport security</td>
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

The server supports security policy None. With this security policy, application authentication is disabled. Without application authentication, the server can be accessed using unauthorized applications.

## Solution

Disable the security policy None.

## References

* [https://profiles.opcfoundation.org/profile/762](https://profiles.opcfoundation.org/profile/762)
