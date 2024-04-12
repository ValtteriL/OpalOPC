---
title: Integrating OpalOPC with GitHub Actions
description: How to use GitHub Actions to automate security testing of an OPC UA server
keywords: [how-to, github, github actions, cicd, automation, code scanning]
sidebar_position: 2
---

You can integrate OpalOPC with GitHub Actions. This enables you to run OpalOPC scans as a stage in your existing CI/CD pipeline.

To learn how to do this, see the readme file for our [GitHub Action](https://github.com/marketplace/actions/opalopc-scan-action)

## GitHub Code Scanning

You can also integrate OpalOPC with GitHub Code Scanning by uploading SARIF scan reports to it.
This lets you display the results in GitHub, or configure webhooks that listen to code scanning activity in your repository.

See [Uploading a SARIF file to GitHub](https://docs.github.com/en/code-security/code-scanning/integrating-with-code-scanning/uploading-a-sarif-file-to-github#uploading-a-code-scanning-analysis-with-github-actions).
