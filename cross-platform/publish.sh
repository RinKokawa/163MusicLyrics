#!/bin/bash
set -e

targets=(
  "win-x64"
  "linux-x64"
  "osx-x64"
)

for target in "${targets[@]}"
do
  echo "Publishing for $target..."
  dotnet publish ./MusicLyricApp/MusicLyricApp.csproj \
    -c Release \
    -r $target \
    --self-contained true \
    -p:DebugType=None \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "publish/$target"
done

echo -e "\n✅ Done. Files are in the 'publish/*' folders."
