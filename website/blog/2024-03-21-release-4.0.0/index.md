---
slug: release-400
title: Release 4.0.0
keywords: [release, version 4.0.0]
authors: [valtteri]
tags: [release, 4.0.0]
---

![Release 4.0.0](release-4.0.0.png)

* Licensing enforcement
* Semantic Versioning
* Direct download distribution
* Multiple bugfixes

<!-- truncate -->

## Licensing enforcement

The free plan has been discontinued, and all OpalOPC users are now required to have a software license. Our [EULA](https://opalopc.com/EULA.txt) has been updated accordingly.

To obtain a license, either [start a free trial or purchase](/#starthere) OpalOPC. After obtaining, you need to configure OpalOPC to use the license before scanning.

## Semantic Versioning

Future version numbering conforms to the [Semantic Versioning](https://semver.org/) (SemVer) version 2.0.0 specification. As we have followed the same principles also before, this change is purely cosmetic. It simply means that the trailing `.0` of the old version numbers are dropped.

## Direct download distribution

New releases are now available for direct download at dl.opalopc.com.

The latest stable version string is available at https://dl.opalopc.com/release/stable.txt.

Linux binaries are available at: `https://dl.opalopc.com/release/<VERSION>/bin/linux/amd64/opalopc`

To download the latest stable version, you could use the following command:

```bash
curl -LO "https://dl.opalopc.com/release/$(curl -L -s https://dl.opalopc.com/release/stable.txt)/bin/linux/amd64/opalopc"
```

This is useful for installing OpalOPC in environments where Snap and Microsoft Store are not available, such as in containers. For the time being, only Linux Amd64 release is available in this manner.

## Multiple bugfixes

Multiple bugs were discovered during the [OPC Interoperability Workshop in North America](https://opcfoundation.org/event-detail/opc-interoperability-workshop-2024-north-america/)
These bugs have been fixed, and OpalOPC 4.0.0 works even better with various products of for instance Siemens, Schneider Electric, Unified automation, and Microsoft.
