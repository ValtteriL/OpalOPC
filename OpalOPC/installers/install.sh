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
# - delete /usr/local/opalopc/
# - delete /usr/local/bin/opalopc
# 
# Run as root.
#

### Flags

#ACCEPT_EULA=1

### Variables

INSTALL_DIR="/usr/local/opalopc"
OPALOPC_EXECUTABLE_PATH="${INSTALL_DIR}/opalopc"
OPALOPC_SIGNATURE_PATH="${INSTALL_DIR}/opalopc.asc"
OPALOPC_USR_LOCAL_BIN_SYMLINK_PATH="/usr/local/bin/opalopc"
LINUX_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/linux/opalopc"
MAC_LATEST_OPALOPC_URI="https://opalopc.com/releases/latest/osx/opalopc"
OPALOPC_URI=""
OPALOPC_SIGNATURE_URI=""
GPG_KEY_URI="https://github.com/ValtteriL.gpg"
EULA_URI="https://opalopc.com/EULA.txt"

### Functions

# bool function to test if the user is root or not
is_user_root () { [ "${EUID:-$(id -u)}" -eq 0 ]; }

# bool function to check if running on Linux
is_mac () { [[ $OSTYPE == 'darwin'* ]]; }

# Checks to see if the given command (passed as a string argument) exists on the system.
# The function returns 0 (success) if the command exists, and 1 if it doesn't.
is_command () {
    local check_command="$1"
    command -v "${check_command}" >/dev/null 2>&1
}

print_banner () {
    echo '
 .d88888b.                    888  .d88888b.  8888888b.   .d8888b.  
d88P" "Y88b                   888 d88P" "Y88b 888   Y88b d88P  Y88b 
888     888                   888 888     888 888    888 888    888 
888     888 88888b.   8888b.  888 888     888 888   d88P 888        
888     888 888 "88b     "88b 888 888     888 8888888P"  888        
888     888 888  888 .d888888 888 888     888 888        888    888 
Y88b. .d88P 888 d88P 888  888 888 Y88b. .d88P 888        Y88b  d88P 
 "Y88888P"  88888P"  "Y888888 888  "Y88888P"  888         "Y8888P"  
            888                                                     
            888                                                     
            888                                                     
    '
}

### Program

print_banner
echo ""

# Show EULA
echo "[1/7] End User License Agreement (EULA)"
while true; do

    if ! [ -z ${ACCEPT_EULA+x} ] ; then
        echo "EULA accepted using environment variable"
        break
    fi

    wget --quiet --output-document - "${EULA_URI}" | more

    read -p "Do you accept the EULA (y/n)? " answer </dev/tty

    if [ "$answer" != "${answer#[Yy]}" ] ; then
        break
    elif [ "$answer" != "${answer#[Nn]}" ] ; then
        echo "Installation aborted"
        exit 1
    fi
done

# Check that script is run as root
echo "[2/7] Checking if installer run as root"
if ! is_user_root; then
    echo "Installer must be run as root"
    exit 1
fi

# Create directory
echo "[3/7] Creating installation directory (${INSTALL_DIR})"
mkdir -p "${INSTALL_DIR}"

# Download latest OpalOPC
echo "[4/7] Downloading Opal OPC"
if is_mac; then
    OPALOPC_URI="${MAC_LATEST_OPALOPC_URI}"
else
    OPALOPC_URI="${LINUX_LATEST_OPALOPC_URI}"
fi
wget --output-document "${OPALOPC_EXECUTABLE_PATH}" "${OPALOPC_URI}"

# Check signature
echo "[5/7] Checking signature"
if is_command gpg ; then
    wget --quiet --output-document - "${GPG_KEY_URI}" | gpg --quiet --import -

    OPALOPC_SIGNATURE_URI="${OPALOPC_URI}.asc"
    wget --quiet --output-document "${OPALOPC_SIGNATURE_PATH}" "${OPALOPC_SIGNATURE_URI}"

    # if fails, very bad!
    if ! gpg --quiet --batch --verify "${OPALOPC_SIGNATURE_PATH}" "${OPALOPC_EXECUTABLE_PATH}"; then
        echo "CRITICAL: Signature is not valid!!! Contact us immediately"
        exit 1
    fi
else
    echo "WARNING: Cannot check OpalOPC signature (gpg not available)"
fi

# Make executable
echo "[6/7] Making Opal OPC executable"
chmod +x "${OPALOPC_EXECUTABLE_PATH}"

# Add to path
echo "[7/7] Adding \"opalopc\" to path"
ln --symbolic --force "${OPALOPC_EXECUTABLE_PATH}" "${OPALOPC_USR_LOCAL_BIN_SYMLINK_PATH}"

echo ""
echo "Installation complete! Type opalopc to get started"
