#!/usr/bin/env bash
set -euo pipefail

# Usage: ./scripts/build-docs.sh [-c|--configuration <Configuration>]
CONFIGURATION="Release"

while (( "$#" )); do
  case "$1" in
    -c|--configuration)
      if [ -n "${2-}" ] && [ "${2:0:1}" != "-" ]; then
        CONFIGURATION="$2"
        shift 2
      else
        echo "Error: Argument for $1 is missing" >&2
        exit 1
      fi
      ;;
    -h|--help)
      echo "Usage: $0 [-c|--configuration CONFIGURATION]"
      exit 0
      ;;
    *)
      # Allow calling with a single positional argument (e.g., ./script.sh Release)
      CONFIGURATION="$1"
      shift
      ;;
  esac
done

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