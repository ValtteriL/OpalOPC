---
id: 10002
title: Auditing disabled
description: Information on the issue detected by Auditing disabled security testing plugin.
slug: /plugin-10002
tags: [plugin, accounting]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Medium</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10002</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Accounting</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>5.0</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>-</td>
  </tr>
</table>

## Synopsis

The server is not tracking activities.

## Description

The target server reports that auditing is disabled. Without auditing it is not possible to determine what has been done on the server and by who. It is also not possible to review and verify system operations, or track abnormal behavior.

## Solution

Enable auditing on the server.

## References

* [https://reference.opcfoundation.org/Core/Part4/v105/docs/6.5](https://reference.opcfoundation.org/Core/Part4/v105/docs/6.5)
* [https://reference.opcfoundation.org/Core/Part2/v105/docs/3.1.9](https://reference.opcfoundation.org/Core/Part2/v105/docs/3.1.9)
* [https://reference.opcfoundation.org/Core/Part2/v105/docs/3.1.8](https://reference.opcfoundation.org/Core/Part2/v105/docs/3.1.8)
