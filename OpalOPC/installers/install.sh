#!/bin/bash
set -e # exit on any command failure

#
# This script installs the latest OpalOPC for Linux 
# (as many distros as possible) and MacOS. 
#
# After installation OpalOPC is found in /opt/opalopc/
# and it is added to all user's paths (Borne Shell derivatives)
# through /etc/profile.d/opalopc-path.sh
# 
# To uninstall, just:
# - delete /opt/opalopc/
# - delete /etc/profile.d/opalopc-path.sh
# 
# Run as root.
#

### Variables

INSTALL_DIR="/opt/opalopc"
OPALOPC_EXECUTABLE_PATH="${INSTALL_DIR}/opalopc"
OPALOPC_SIGNATURE_PATH="${INSTALL_DIR}/opalopc.asc"
PROFILE_FILE_PATH="/etc/profile.d/opalopc-path.sh"
LINUX_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/linux/opalopc"
MAC_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/osx/opalopc"
OPALOPC_URI=""
OPALOPC_SIGNATURE_URI=""
GPG_KEY_URI="https://github.com/ValtteriL.gpg"

### Functions

# bool function to test if the user is root or not
is_user_root () { [ "${EUID:-$(id -u)}" -eq 0 ]; }

# bool function to check if running on Linux
is_mac () { [[ $OSTYPE == 'darwin'* ]]; }

# Checks to see if the given command (passed as a string argument) exists on the system.
# The function returns 0 (success) if the command exists, and 1 if it doesn't.
is_command() {
    local check_command="$1"
    command -v "${check_command}" >/dev/null 2>&1
}

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
wget --quiet --output-document "${OPALOPC_EXECUTABLE_PATH}" "${OPALOPC_URI}"

# Check signature
if is_command gpg ; then
    wget -qO - "${GPG_KEY_URI}" | gpg --import -

    OPALOPC_SIGNATURE_URI="${OPALOPC_URI}.asc"
    wget --quiet --output-document "${OPALOPC_SIGNATURE_PATH}" "${OPALOPC_SIGNATURE_URI}"

    # if fails, very bad!
    if ! gpg --verify "${OPALOPC_SIGNATURE_PATH}"; then
        echo "CRITICAL: Signature is not valid!!! Contact us immediately"
        exit 1
    fi
else
    echo "WARNING: Cannot check OpalOPC signature (gpg not available)"
fi

# Make executable
chmod +x "${OPALOPC_EXECUTABLE_PATH}"

# Add to path
cat > "${PROFILE_FILE_PATH}" << EOF
export PATH="\$PATH:${INSTALL_DIR}"
EOF

# Reload bash profile
source "${PROFILE_FILE_PATH}"

echo "Installation complete! Type opalopc to get started"
