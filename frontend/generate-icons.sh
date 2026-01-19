#!/bin/bash

# Generate PWA icons from logo-galo.jpg

LOGO="public/logo-galo.jpg"
SIZES=(72 96 128 144 152 192 384 512)

echo "Generating PWA icons..."

for size in "${SIZES[@]}"; do
  echo "Creating icon-${size}x${size}.png..."
  convert "$LOGO" -resize "${size}x${size}" "public/icons/icon-${size}x${size}.png"
done

# Create favicon
echo "Creating favicon.ico..."
convert "$LOGO" -resize 32x32 "public/favicon.ico"

echo "✅ All icons generated!"
