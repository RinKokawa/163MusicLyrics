#!/bin/bash
set -e

echo "🧹 Cleaning previous publish output..."
rm -rf publish/*

if [ -z "$1" ]; then
  echo "❌ Usage: $0 <version>"
  exit 1
fi

version="$1"
app_name="MusicLyric"
project_path="./MusicLyricApp/MusicLyricApp.csproj"
output_root="publish"

targets=(
  "win-x64"
  "linux-x64"
  "osx-x64"
  "osx-arm64"
)

trap 'echo "❌ An error occurred. Exiting."' ERR

for target in "${targets[@]}"
do
  echo -e "\n-----------------------------"
  echo "📦 Publishing for $target..."

  output_dir="$output_root/$target"
  dotnet publish "$project_path" \
    -c Release \
    -r $target \
    --self-contained true \
    -p:DebugType=None \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "$output_dir"

  # Rename Windows exe for clarity
  if [[ "$target" == win-* ]]; then
    ext=".exe"
    original_file=$(find "$output_dir" -type f -name "*$ext" -print -quit)
    if [[ -n "$original_file" ]]; then
      new_filename="${app_name}-${version}-${target}${ext}"
      new_filepath="${output_dir}/${new_filename}"
      mv "$original_file" "$new_filepath"
      echo "✅ Renamed Windows exe to: $new_filename"
    fi
  fi

  archive_name="${app_name}-${version}-${target}.tar.gz"
  tar -czf "$output_root/$archive_name" -C "$output_dir" .
  echo "🗜️  Compressed all files in $output_dir to $archive_name"
done

echo -e "\n🎉 All done. Archives are in the '$output_root/' folder."
