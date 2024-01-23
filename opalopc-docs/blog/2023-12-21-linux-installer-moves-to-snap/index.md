---
slug: linux-installer-moves-to-snap
title: Linux installer moves to Snap
authors: [valtteri]
tags: [misc, linux, snap, 2.0.3.0]
---

![Snapcraft logo](Snapcraft-logo-bird.png)

Starting from OpalOPC 2.0.3.0, the Linux installer has been replaced by [Snap](https://snapcraft.io/opalopc). This makes it easy for Linux users to install and keep the scanner up-to-date. Furthermore, the security features of Snap protect our customers when installing, updating, or using our product.

## Migration from old installation

If you have installed OpalOPC using the old installer, remove it as follows:

```bash
sudo rm -rf /usr/local/bin/opalopc /usr/local/opalopc
```

Then follow the installation instructions on the [Snap listing page](https://snapcraft.io/opalopc).
