#!/bin/bash
set -e

if [[ "$(uname)" != "Darwin" ]]; then
  echo "❌ This script must be run on macOS."
  exit 1
fi

if [ -z "$1" ]; then
  echo "❌ Usage: $0 <version>"
  exit 1
fi

version="$1"
app_name="MusicLyricApp"
output_root="publish"
targets=("osx-x64" "osx-arm64")

for target in "${targets[@]}"; do
  echo -e "\n📦 Processing $target..."

  archive_path="$output_root/${app_name}-${version}-${target}.tar.gz"
  extract_dir="$output_root/$target"

  if [ ! -f "$archive_path" ]; then
    echo "❌ Missing archive: $archive_path"
    exit 1
  fi

  echo "📂 Extracting $archive_path..."
  mkdir -p "$extract_dir"
  tar -xzf "$archive_path" -C "$extract_dir"

  echo "🍎 Building .app bundle..."
  app_dir="$output_root/${app_name}-${version}-${target}.app"
  contents_dir="$app_dir/Contents"
  macos_dir="$contents_dir/MacOS"
  resources_dir="$contents_dir/Resources"

  mkdir -p "$macos_dir" "$resources_dir"
  cp -R "$extract_dir"/* "$macos_dir/"
  chmod +x "$macos_dir/$app_name"

  cat > "$contents_dir/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
 "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleExecutable</key>
  <string>$app_name</string>
  <key>CFBundleIdentifier</key>
  <string>com.github.jitwxs.$app_name</string>
  <key>CFBundleName</key>
  <string>$app_name</string>
  <key>CFBundleVersion</key>
  <string>$version</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
</dict>
</plist>
EOF

  echo "🔐 Signing with ad-hoc identity..."
  codesign --force --deep --timestamp=none --sign - "$app_dir"

  echo "🗜️  Compressing .app to tar.gz..."
  tar -czf "$output_root/${app_name}-${version}-${target}-app.tar.gz" -C "$output_root" "$(basename "$app_dir")"

  echo "🧹 Cleaning up .app and extracted files..."
  rm -rf "$app_dir"
  rm -rf "$extract_dir"
done

echo -e "\n✅ Done! .tar.gz app bundles are in $output_root/"
