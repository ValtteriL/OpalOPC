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
PROFILE_FILE_PATH="/etc/profile.d/opalopc-path.sh"

### Functions

# bool function to test if the user is root or not
is_user_root () { [ "${EUID:-$(id -u)}" -eq 0 ]; }

### Program

# Check that script is run as root
if ! is_user_root; then
    echo "Installer must be run as root"
    exit 1
fi

# Create directory
# Download latest OpalOPC
# (Check signature)
# Make executable
# Add to path


echo "Hello world!"
