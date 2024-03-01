---
id: 10008
title: Security policy Basic256
description: Server traffic encryption in risk to be broken.
keywords: [plugin, security policy basic256, transport-security]
slug: /plugin-10008
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
    <td>10008</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Transport security</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>4.8</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>[https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N](https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:L/A:N)</td>
  </tr>
</table>

## Synopsis

Server traffic encryption in risk to be broken.

## Description

The server supports security policy Basic256. This security policy has been deprecated with the OPC UA Specification, as the hash algorithm SHA1 is not considered secure anymore. An attacker may be able to break the Basic256 traffic encryption.

## Solution

Disable the security policy Basic256.

## References

* [https://profiles.opcfoundation.org/profilefolder/474](https://profiles.opcfoundation.org/profilefolder/474)
