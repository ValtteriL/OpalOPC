#!/bin/bash

#
# This script installs the latest OpalOPC for Linux 
# (as many distros as possible) and MacOS. 
#
# After installation OpalOPC is found in /opt/opalopc/
# and it is added to all user's paths (Borne Shell derivatives)
# through /etc/profile.d/opalopc-path.sh
# 
# For uninstallation, just:
# - delete /opt/opalopc/
# - delete /etc/profile.d/opalopc-path.sh
# 
# Run as root.
#

### Variables

INSTALL_DIR="/opt/opalopc"
OPALOPC_EXECUTABLE_PATH="${INSTALL_DIR}/opalopc"
PROFILE_FILE_PATH="/etc/profile.d/opalopc-path.sh"
LINUX_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/linux/opalopc"
MAC_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/osx/opalopc"
OPALOPC_URI=""
OPALOPC_SIGNATURE_URI=""

### Functions

# bool function to test if the user is root or not
is_user_root () { [ "${EUID:-$(id -u)}" -eq 0 ]; }

# bool function to check if running on Linux
is_mac () { [[ $OSTYPE == 'darwin'* ]]; }

### Program

# Check that script is run as root
if ! is_user_root; then
    echo "Installer must be run as root"
    exit 1
fi

# Create directory
mkdir -p "${INSTALL_DIR}"

# Download latest OpalOPC
if is_mac; then
    OPALOPC_URI="${MAC_LATEST_OPALOPC_URI}"
else
    OPALOPC_URI="${LINUX_LATEST_OPALOPC_URI}"
fi
wget --output-document "${OPALOPC_EXECUTABLE_PATH}" "${OPALOPC_URI}"


# (Check signature)
OPALOPC_SIGNATURE_URI="${OPALOPC_URI}.asc"
echo "${OPALOPC_SIGNATURE_URI}"

# Make executable
# Add to path


echo "Hello world!"
