#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"
root_dir="$(cd "$script_dir/.." && pwd)"
output_dir="$root_dir/Fixtures/Generados"

mkdir -p "$output_dir/archivos/Carpeta con espacios/niño_日本語_🚀" "$output_dir/conflictos"
printf 'fixture sintético unicode\n' > "$output_dir/archivos/Carpeta con espacios/niño_日本語_🚀/entrada.txt"
printf 'original sintético\n' > "$output_dir/conflictos/salida.txt"

ffmpeg -hide_banner -loglevel error -y \
  -f lavfi -i 'sine=frequency=440:sample_rate=48000:duration=8' \
  -ac 2 -c:a pcm_s16le "$output_dir/audio_48k_stereo.wav"

ffmpeg -hide_banner -loglevel error -y \
  -f lavfi -i 'testsrc2=size=1920x1080:rate=30:duration=6' \
  -f lavfi -i 'sine=frequency=880:sample_rate=48000:duration=6' \
  -c:v libx264 -preset ultrafast -pix_fmt yuv420p -c:a aac -shortest \
  "$output_dir/video_1080p.mp4"

if [[ "${SYNTOOLS_GENERATE_4K:-0}" == "1" ]]; then
  ffmpeg -hide_banner -loglevel error -y \
    -f lavfi -i 'testsrc2=size=3840x2160:rate=30:duration=3' \
    -c:v libx264 -preset ultrafast -pix_fmt yuv420p "$output_dir/video_4k.mp4"
fi

python3 - "$output_dir/waveform.csv" <<'PY'
import math, sys
with open(sys.argv[1], "w", encoding="utf-8") as output:
    for index in range(20_000):
        value = math.sin(index / 21.0) * (0.55 + 0.45 * math.sin(index / 997.0))
        output.write(f"{index},{value:.8f}\n")
PY

echo "Fixtures sintéticos generados en $output_dir"
