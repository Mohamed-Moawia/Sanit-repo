#!/usr/bin/env bash
# SSH Key Setup & GitHub Authentication Test Script

set -euo pipefail

SSH_KEY_PATH="$HOME/.ssh/id_ed25519"

# 1. Check if key exists
if [[ ! -f "$SSH_KEY_PATH" ]]; then
    echo "❌ SSH private key not found at $SSH_KEY_PATH"
    echo "Generate one with: ssh-keygen -t ed25519 -C 'your_email@example.com'"
    exit 1
fi

# 2. Fix permissions
echo "🔧 Fixing permissions..."
chmod 700 "$HOME/.ssh"
chmod 600 "$SSH_KEY_PATH"

# 3. Start ssh-agent if not running
if ! pgrep -u "$USER" ssh-agent > /dev/null; then
    echo "🚀 Starting ssh-agent..."
    eval "$(ssh-agent -s)"
else
    echo "ℹ️ ssh-agent already running."
    export SSH_AUTH_SOCK
    export SSH_AGENT_PID
fi

# 4. Add key to agent
echo "➕ Adding SSH key to agent..."
ssh-add "$SSH_KEY_PATH"

# 5. Test GitHub authentication
echo "🔍 Testing GitHub SSH authentication..."
ssh -T git@github.com || true

echo "✅ Setup complete. You can now use Git/SSH without retyping your passphrase."
