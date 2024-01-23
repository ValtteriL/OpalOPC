---
id: 10012
title: Brute Force
description: Information on the issue detected by Brute Force security testing plugin.
slug: /plugin-10012
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
    <td>10012</td>
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

Brute Force attack was successful against the server.

## Description

Authentication to the target server was attempted with a user-provided combination of usernames and passwords. Authentication was successful with one or multiple combinations. This indicates that weak credentials are in use.

## Solution

Use strong credentials that are hard to guess, block authentication from a client after a number of failed attempts, or disable password authentication altogether.

## References

* [https://owasp.org/www-community/attacks/Brute_force_attack](https://owasp.org/www-community/attacks/Brute_force_attack)
