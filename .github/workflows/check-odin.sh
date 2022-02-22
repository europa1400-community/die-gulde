SIRENIX_DIR="./src/Assets/Plugins/Sirenix"

if [ -d "$SIRENIX_DIR" ]; then
    echo "Sirenix directory found."
    exit 0
else
    echo "Sirenix directory not found."
    exit 1
fi