#!/bin/bash
# Debian 13.3 Xfce Master Post-Install Script
# Run as root: sudo ./debian-xfce-master.sh

echo "Starting Debian 13.3 Xfce optimization..."

# --- Remove heavy office suite ---
echo "Removing LibreOffice..."
sudo apt purge -y libreoffice* libreoffice-common

# --- Remove games and educational extras ---
echo "Removing games and extras..."
sudo apt purge -y gnome-games* aisleriot* gnome-sudoku* gnome-mahjongg* gnome-mines* gnome-tetravex* gnome-taquin* gnome-robots*

# --- Remove indexing/search services ---
echo "Removing Tracker (file indexing)..."
sudo apt purge -y tracker3* tracker-miner* tracker-extract*

# --- Remove unnecessary multimedia apps ---
echo "Removing heavy multimedia apps..."
sudo apt purge -y rhythmbox* totem* gnome-music* brasero* shotwell*

# --- Remove printing system if not needed ---
echo "Removing printing system (CUPS)..."
sudo apt purge -y cups* printer-driver* system-config-printer*

# --- Clean up orphaned packages ---
echo "Cleaning up unused dependencies..."
sudo apt autoremove -y
sudo apt clean

# --- Install lean essentials ---
echo "Installing lean essentials..."

# PDF viewer
sudo apt install -y evince

# Media player
sudo apt install -y vlc

# Image viewer
sudo apt install -y ristretto

# Archive manager
sudo apt install -y xarchiver

# Browser (Firefox ESR)
sudo apt install -y firefox-esr

# Text editor
sudo apt install -y mousepad

# Terminal (Konsole instead of xfce4-terminal)
echo "Installing Konsole terminal..."
sudo apt install -y konsole
sudo apt purge -y xfce4-terminal

# Monitoring & utilities
sudo apt install -y htop fastfetch curl wget gpg

# --- Install Microsoft Edge ---
echo "Adding Microsoft Edge repository..."
curl -fSsL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor | sudo tee /usr/share/keyrings/microsoft.gpg > /dev/null

echo "deb [arch=amd64 signed-by=/usr/share/keyrings/microsoft.gpg] https://packages.microsoft.com/repos/edge stable main" | sudo tee /etc/apt/sources.list.d/microsoft-edge.list

echo "Updating package lists..."
sudo apt update

echo "Installing Microsoft Edge (Stable)..."
sudo apt install -y microsoft-edge-stable

echo "Optimization complete! Debian Xfce is now lean, fast, and stable with Konsole and Microsoft Edge."

