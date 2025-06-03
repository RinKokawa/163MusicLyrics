#!/bin/bash
set -e

echo "🧹 Cleaning previous publish output..."
rm -rf publish/*

if [ -z "$1" ]; then
  echo "❌ Usage: $0 <version>"
  echo "👉 Example: ./build-all.sh 1.2.3"
  exit 1
fi

version="$1"
app_name="MusicLyricApp"
project_path="./MusicLyricApp/MusicLyricApp.csproj"
output_root="publish"

targets=(
  "win-x64"
  "linux-x64"
  "osx-x64"
  "osx-arm64"
)

trap 'echo "❌ An error occurred. Exiting."' ERR

for target in "${targets[@]}"; do
  echo -e "\n-----------------------------"
  echo "📦 Publishing for $target..."

  output_dir="$output_root/$target"
  dotnet publish "$project_path" \
    -c Release \
    -r "$target" \
    --self-contained true \
    -p:DebugType=None \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "$output_dir"

  if [[ "$target" == win-* ]]; then
    ext=".exe"
    original_file=$(find "$output_dir" -type f -name "*$ext" -print -quit)
    if [[ -n "$original_file" ]]; then
      new_filename="${app_name}-${version}-${target}${ext}"
      mv "$original_file" "$output_dir/$new_filename"
      echo "✅ Renamed Windows executable to: $new_filename"
    fi
  elif [[ "$target" == linux-* ]]; then
    bin_path="$output_dir/$app_name"
    if [[ -f "$bin_path" ]]; then
      chmod +x "$bin_path"
      echo "🔧 Set executable permission for Linux binary: $app_name"
    else
      echo "⚠️  Linux binary not found at: $bin_path"
    fi
  fi

  archive_name="${app_name}-${version}-${target}.tar.gz"
  tar -czf "$output_root/$archive_name" -C "$output_dir" .
  echo "🗜️  Compressed to: $archive_name"

  echo "🧹 Removing intermediate directory: $output_dir"
  rm -rf "$output_dir"
done

echo -e "\n✅ All targets published and compressed."
echo "💡 To package macOS .app, copy the .tar.gz files to a macOS machine and run: ./build-macos-app.sh $version"
