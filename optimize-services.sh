#!/bin/bash
# Debian 13.3 Service Optimization Script for Dell 4520
# Run as root: sudo ./optimize-services.sh

echo "Optimizing services for Debian 13.3 on Dell 4520..."

# --- Disable unnecessary services ---
services_to_disable=(
    cups.service
    avahi-daemon.service
    ModemManager.service
    ssh.service
    tracker-miner-fs.service
    tracker-miner-rss.service
    tracker-store.service
)

for svc in "${services_to_disable[@]}"; do
    if systemctl list-unit-files | grep -q "$svc"; then
        echo "Disabling $svc..."
        systemctl disable --now $svc
    fi
done

# --- Enable essential services ---
services_to_enable=(
    NetworkManager.service
    wpa_supplicant.service
    ufw.service
    cron.service
    rsyslog.service
    acpid.service
    thermald.service
)

for svc in "${services_to_enable[@]}"; do
    if systemctl list-unit-files | grep -q "$svc"; then
        echo "Enabling $svc..."
        systemctl enable --now $svc
    fi
done

# --- Optional: SMART disk monitoring ---
if systemctl list-unit-files | grep -q smartd.service; then
    echo "Enabling smartd (disk health monitoring)..."
    systemctl enable --now smartd.service
fi

echo "Service optimization complete!"
echo "Reboot recommended to apply all changes."

