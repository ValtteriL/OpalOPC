---
title: Install
description: The installation documentation shows how to install OpalOPC on Windows or Linux.
keywords: [install, update]
sidebar_position: 2
---

## Windows

Use [Microsoft Store](https://apps.microsoft.com/detail/OpalOPC/9N89VWR0GK7H?launch=true&mode=mini) to install OpalOPC. The Store keeps it up-to-date for you automatically.

<iframe width="100%" height="444" src="https://www.youtube-nocookie.com/embed/0sstrWBaSYA?si=oUtJ1bbVDYxQwUan" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen></iframe>

## Linux

We recommend using Snap if it is available. The manual method is provided for advanced users.

### Snap

Use [Snap Store](https://snapcraft.io/opalopc) to install OpalOPC. Snap keeps it up-to-date for you automatically.

<iframe width="100%" height="444" src="https://www.youtube-nocookie.com/embed/PWbCHtSqhis?si=K929wmVRGjVQP7O7" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen></iframe>

### Manual

When installing manually, you are responsible for keeping OpalOPC up-to-date.

```bash
curl -LO "https://dl.opalopc.com/release/$(curl -L -s https://dl.opalopc.com/release/stable.txt)/bin/linux/amd64/opalopc"
sudo install -o root -g root -m 0755 opalopc /usr/local/bin/opalopc
````

:::note

If you do not have root access on the target system, you can still install opalopc to the `~/.local/bin` directory:

```bash
chmod +x opalopc
mkdir -p ~/.local/bin
mv ./opalopc ~/.local/bin/opalopc
# and then append (or prepend) ~/.local/bin to $PATH
```

:::

## After installation

Before scanning anything, you need to [configure the license key](configure-license-key.md).
