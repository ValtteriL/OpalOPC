---
id: 10014
title: Invalid Server Certificate
description: Server certificate is invalid.
keywords: [plugin, server certificate invalid, transport-security]
slug: /plugin-10014
tags: [plugin, transport-security]
---

## Plugin details

<table>
  <tr>
    <th>Severity</th>
    <td>Low</td>
  </tr>
  <tr>
    <th>ID</th>
    <td>10014</td>
  </tr>
    <tr>
    <th>Category</th>
    <td>Transport security</td>
  </tr>
    <tr>
    <th>CVSS score</th>
    <td>3.7</td>
  </tr>
  <tr>
    <th>CVSS link</th>
    <td>[https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:A/AC:H/PR:N/UI:R/S:U/C:L/I:L/A:N](https://www.first.org/cvss/calculator/3.1#CVSS:3.1/AV:A/AC:H/PR:N/UI:R/S:U/C:L/I:L/A:N)</td>
  </tr>
</table>

## Synopsis

The server certificate is invalid.

## Description

The server uses certificate that fails validation. The certificate may have expired, been revoked, or is not trusted by the client.

Using an invalid server certificate prevents the client from verifying the server’s identity and the communication channel’s integrity and confidentiality.

## Solution

Configure server with valid certificate. The certificate should be issued by a trusted Certificate Authority (CA) and should not be expired or revoked.

## References

