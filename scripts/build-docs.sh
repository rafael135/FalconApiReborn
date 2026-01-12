#!/usr/bin/env bash
set -euo pipefail

CONFIGURATION="${1:-Release}"

echo "Building solution in ${CONFIGURATION}..."
dotnet build -c "${CONFIGURATION}"

# Find XML files under bin folders within src
mapfile -t xmls < <(find ./src -type f -name "*.xml" -path "*/bin/*")

if [ ${#xmls[@]} -eq 0 ]; then
  echo "No XML documentation files found. Make sure projects have <GenerateDocumentationFile>true</GenerateDocumentationFile> set and build succeeded." >&2
  exit 1
fi

DEST="docs/api"
mkdir -p "$DEST"

for xml in "${xmls[@]}"; do
  echo "Copying ${xml} -> ${DEST}"
  cp -f "$xml" "$DEST/"
done

echo "Done. XML documentation files copied to ${DEST}."