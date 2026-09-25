#!/usr/bin/env bash
set -euo pipefail

project_dir="$(cd "$(dirname "$0")" && pwd)"
publish_dir="$project_dir/artifacts/osx-arm64-framework"
app_dir="$project_dir/artifacts/SynToolsAvaloniaProbe.app"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
dotnet publish "$project_dir/SynTools.Probe.App/SynTools.Probe.App.csproj" \
  --configuration Release --runtime osx-arm64 --self-contained true \
  -p:UseAppHost=true -o "$publish_dir"

rm -rf "$app_dir"
mkdir -p "$app_dir/Contents/MacOS" "$app_dir/Contents/Resources"
cp -R "$publish_dir/"* "$app_dir/Contents/MacOS/"
cp "$project_dir/Info.plist" "$app_dir/Contents/Info.plist"
codesign --force --deep --sign - "$app_dir"
echo "$app_dir"
