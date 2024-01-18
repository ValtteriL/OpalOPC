---
id: 10006
title: Security mode none
description: My document description
slug: /plugin-10006
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
    <td>10006</td>
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

Server traffic can be intercepted and modified on the fly.

## Description

The server supports message security mode ‘None’, which does not provide any protection. Anyone can intercept and modify the traffic, and read any secrets within it.

## Solution

Configure server with either message security mode ‘Sign’ or ‘SignAndEncrypt’.

## References

* [https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/](https://opcconnect.opcfoundation.org/2018/06/practical-security-guidelines-for-building-opc-ua-applications/)
