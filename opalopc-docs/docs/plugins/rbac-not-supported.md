---
id: 10004
title: RBAC not supported
description: Information on the issue detected by RBAC not supported security testing plugin.
slug: /plugin-10004
tags: [plugin, authorization]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Info</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10004</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Authorization</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>0</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>-</td>
  </tr>
</table>

## Synopsis

All identities have same level of access on the server.

## Description

The target server reports that it does not support Role Based Access Control (RBAC). This means that access to server resources cannot be restricted based on roles. The outcome is that all identities have same level of access on the server.

## Solution

Update or change server software.

## References

* [https://reference.opcfoundation.org/Core/Part18/v105/docs/4](https://reference.opcfoundation.org/Core/Part18/v105/docs/4)
