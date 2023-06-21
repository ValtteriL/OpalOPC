#!/bin/bash

#
# This script installs the latest OpalOPC for Linux 
# (as many distros as possible) and MacOS. 
#
# After installation OpalOPC is found in /opt/opalopc/
# and it is added to all user's paths through /etc/profile.d/opalopc-path.sh
# 
# For uninstallation, just:
# - delete /opt/opalopc/
# - delete /etc/profile.d/opalopc-path.sh
# 
# Run as root.
#

echo "Hello world!"

# Create directory
# Download latest OpalOPC
# (Check signature)
# Make executable
# Add to path
