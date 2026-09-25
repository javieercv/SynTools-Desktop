#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"
fixture_dir="$(cd "$script_dir/../Fixtures/Generados" && pwd)"

test -s "$fixture_dir/audio_48k_stereo.wav"
test -s "$fixture_dir/video_1080p.mp4"
test -s "$fixture_dir/waveform.csv"
test "$(wc -l < "$fixture_dir/waveform.csv" | tr -d ' ')" = "20000"
test "$(ffprobe -v error -select_streams v:0 -show_entries stream=width,height -of csv=p=0 "$fixture_dir/video_1080p.mp4")" = "1920,1080"
test "$(ffprobe -v error -select_streams a:0 -show_entries stream=sample_rate -of csv=p=0 "$fixture_dir/audio_48k_stereo.wav")" = "48000"
test "$(ffprobe -v error -select_streams a:0 -show_entries stream=channels -of csv=p=0 "$fixture_dir/audio_48k_stereo.wav")" = "2"
echo "Fixtures verificados"
