---
title: How to manage vulnerabilities
description: XXXXXXXXXXXXXXXXXX
keywords: [how-to, defectdojo, cicd, automation, vulnerability management]
sidebar_position: 2
---

After discovering security issues in a system, the next step is to choose which ones will get fixed and then track the issues until they are fixed.
This process is called vulnerability management.

Effective vulnerability management requires a tool for amassing testing reports from various security tools and filtering the results. Such tools are for instance [DefectDojo](https://www.defectdojo.org/), [Software Risk Manager](https://www.synopsys.com/software-integrity/software-risk-manager.html), and [Faraday](https://faradaysec.com/). Subset of the functionality provided by these tools can behad by configuring Jira or Azure DevOps.

This tutorial shows how to manage vulnerabilities discovered with OpalOPC using DefectDojo. If you are using some other platform, the process is pretty much the same. OpalOPC report uses the SARIF report format, which is understood by all major platforms.

## Overview

## Manually importing scan results

Handling duplicates

## Automatically importing scan results from CI/CD pipelines

## Conclusion
