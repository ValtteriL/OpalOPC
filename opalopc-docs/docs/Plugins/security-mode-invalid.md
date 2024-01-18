---
id: 10005
title: Security mode invalid
description: My document description
slug: /plugin-10005
tags: [transport-security]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Medium</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10005</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Transport security</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>6.5</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>[https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N](https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:L/A:N)</td>
  </tr>
</table>

## Synopsis

The server message security mode is invalid.

## Description

The server reports message security mode ‘Invalid’. This value is the default value to avoid an accidental choice of no security is applied. This choice will always be rejected.

## Solution

Configure server with either message security mode ‘Sign’ or ‘SignAndEncrypt’.

## References

* [https://reference.opcfoundation.org/Core/Part4/v104/docs/7.15](https://reference.opcfoundation.org/Core/Part4/v104/docs/7.15)
* [https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/](https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/)
