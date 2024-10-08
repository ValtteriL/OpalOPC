---
slug: release-2000
title: Release 2.0.0.0
keywords: [release, version 2.0.0.0]
authors: [valtteri]
tags: [release, 2.0.0.0]
---

![Release 2.0.0.0](release-2.0.0.0.png)

* Support for user-provided credentials
* New plugin: Brute force
* Improved performance
* Improved certificate handling
* Version flag
* Multiple bugfixes

<!-- truncate -->

## Support for user-provided credentials

User can now configure credentials for OpalOPC to use when authenticating to target servers. This ensures that security checks requiring authentication are run even if OpalOPC is unable to bypass authentication. It also allows you to quickly check where certain credentials allow access.

OpalOPC supports configuring application certificate, user certificate, as well as username and password. These can be specified multiple times. When configured, all of them are used to authenticate to all target servers. The scan report shows which targets allowed authenticating with which credentials.

User can configure credentials in “Configuration” tab. Application certificates are set in Application authentication section. User certificates and username-password combinations are set in User authentication section.

![Application and User authentication configuration](gui-configuration-tab-provided-credentials-screenshot.png)

![Credentials can be specified also on CLI](2.0.0.0-cli-help.png)

## New plugin: Brute force

User can now provide username:password combinations that OpalOPC will try against target servers. This allows you to conduct custom password-guessing attacks against OPC UA servers.

Previous versions contain only the [Common credentials plugin](/docs/plugin-10003), that guesses a hard-coded list of credentials. You now have the ability to additionally guess any credentials you want.

The difference between guessing usernames and passwords in Brute force and by setting User credentials is that Brute force findings have higher severity.

User can configure username-password combinations in Brute force section in the “Settings” tab.

![Brute force configuration](gui-configuration-tab-brute-force-screenshot.png)

## Improved performance

Security scans are now faster. Depending on the scan configuration, it may be even 5 times faster than release 1.2.0.0.

## Improved certificate handling

Self-signed certificates created by OpalOPC are now re-used between scans. This avoids polluting target servers’ certificate stores with single-use certificates.

The generated certificates are also better identified to hint server administrators where they come from.

This improvement was thanks to feedback from [Jouni Aro](https://fi.linkedin.com/in/jouni-aro-34b4681) from Prosys OPC.

## Version flag

It is now possible to check the OpalOPC version on CLI with `--version` flag.

![Version flag output](version-flag.png)
