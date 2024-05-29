#!/bin/bash

# The IP address the domain will resolve to
IP="127.0.0.1"
# The domain to add
DOMAIN="usermanagement-service.local"
# Path to the hosts file
HOSTS_FILE="/etc/hosts"
# Backup file path
BACKUP_FILE="/etc/hosts.backup"

# Create a backup of the hosts file
sudo cp "$HOSTS_FILE" "$BACKUP_FILE"

# Check if the domain already exists in the hosts file
if grep -q "$DOMAIN" "$HOSTS_FILE"; then
    echo "$DOMAIN already exists in $HOSTS_FILE"
else
    # Append the new domain to the hosts file
    echo "Adding $DOMAIN to $HOSTS_FILE"
    echo "$IP    $DOMAIN" | sudo tee -a "$HOSTS_FILE" > /dev/null

    # Flush DNS cache on macOS, for Linux this might not be needed
    # Uncomment the line below if you're sure you need it on your Linux distro
    # sudo systemd-resolve --flush-caches
    echo "Flushing DNS cache on macOS"
    sudo dscacheutil -flushcache; sudo killall -HUP mDNSResponder

    echo "$DOMAIN has been added to $HOSTS_FILE and DNS cache flushed."
fi


#!/bin/bash

# The IP address the domain will resolve to
IP="127.0.0.1"
# The domain to add
DOMAIN="secure-service.local"
# Path to the hosts file
HOSTS_FILE="/etc/hosts"
# Backup file path
BACKUP_FILE="/etc/hosts.backup"

# Create a backup of the hosts file
sudo cp "$HOSTS_FILE" "$BACKUP_FILE"

# Check if the domain already exists in the hosts file
if grep -q "$DOMAIN" "$HOSTS_FILE"; then
    echo "$DOMAIN already exists in $HOSTS_FILE"
else
    # Append the new domain to the hosts file
    echo "Adding $DOMAIN to $HOSTS_FILE"
    echo "$IP    $DOMAIN" | sudo tee -a "$HOSTS_FILE" > /dev/null

    # Flush DNS cache on macOS, for Linux this might not be needed
    # Uncomment the line below if you're sure you need it on your Linux distro
    # sudo systemd-resolve --flush-caches
    echo "Flushing DNS cache on macOS"
    sudo dscacheutil -flushcache; sudo killall -HUP mDNSResponder

    echo "$DOMAIN has been added to $HOSTS_FILE and DNS cache flushed."
fi
