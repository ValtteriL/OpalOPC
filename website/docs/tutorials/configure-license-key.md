---
title: Configure License Key
description: How to configure the license key for OpalOPC
keywords: [how-to, start, configure]
sidebar_position: 2
---

You can configure a license key to OpalOPC by saving the license key to disk, or by using an environment variable.
The environment variable takes precedence over the license key saved to disk.

To obtain a license key, start a trial or purchase OpalOPC.

## Store license key to disk

The following command stores the the license key to disk. This key will be used by OpalOPC in all future invocations.

```bash
opalopc --set-licence-key <license_key>
```

## Environment variable

Set the `OPALOPC_LICENCE_KEY` environment variable to the license key.
