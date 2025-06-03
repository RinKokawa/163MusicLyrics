#!/bin/bash
set -e

# Ensure we're running on macOS
if [[ "$(uname)" != "Darwin" ]]; then
  echo "❌ This script must be run on macOS."
  exit 1
fi

# Ensure version is provided
if [ -z "$1" ]; then
  echo "❌ Usage: $0 <version>"
  exit 1
fi

version="$1"
app_name="MusicLyricApp"
output_root="publish"
targets=("osx-x64" "osx-arm64")

any_processed=false

for target in "${targets[@]}"; do
  archive_path="$output_root/${app_name}-${version}-${target}.tar.gz"

  # Skip if archive not found
  if [ ! -f "$archive_path" ]; then
    echo "⚠️  Archive not found for $target: skipping."
    continue
  fi

  echo -e "\n📦 Processing target: $target"

  extract_dir="$output_root/$target"
  mkdir -p "$extract_dir"
  tar -xzf "$archive_path" -C "$extract_dir"

  echo "🍎 Creating .app bundle..."
  app_bundle="$output_root/${app_name}-${version}-${target}.app"
  contents_dir="$app_bundle/Contents"
  macos_dir="$contents_dir/MacOS"
  resources_dir="$contents_dir/Resources"

  mkdir -p "$macos_dir" "$resources_dir"
  cp -R "$extract_dir"/* "$macos_dir/"
  chmod +x "$macos_dir/$app_name"

  # Create Info.plist
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

  echo "🔐 Signing .app with ad-hoc identity..."
  codesign --force --deep --timestamp=none --sign - "$app_bundle"

  final_archive="$output_root/${app_name}-${version}-${target}-app.tar.gz"
  echo "🗜️  Compressing .app to $final_archive..."
  tar -czf "$final_archive" -C "$output_root" "$(basename "$app_bundle")"

  echo "🧹 Cleaning up temporary files..."
  rm -rf "$app_bundle" "$extract_dir"

  any_processed=true
done

if [ "$any_processed" = true ]; then
  echo -e "\n🎉 Done! Processed .app bundles are in '$output_root/'"
else
  echo "❌ No valid archives found for any target. Nothing to process."
fi
