# Prueba técnica Qt

Prototipo desechable con C++20, Qt 6.11, Qt Quick/QML, Qt SQL y Qt Multimedia. No es arquitectura productiva.

```bash
cmake -S . -B build -G Ninja -DCMAKE_PREFIX_PATH=/opt/homebrew
cmake --build build
ctest --test-dir build --output-on-failure
SYNTOOLS_FIXTURE_DIR=../Compartido/Fixtures/Generados \
  ./build/SynToolsQtProbe.app/Contents/MacOS/SynToolsQtProbe
```

La salida de audio usa `QAudioSink` con PCM acotado. El vídeo solicita frames a FFmpeg y los presenta en una superficie QML controlada; deliberadamente no usa `QMediaPlayer`.

También puede pasarse `--fixtures-path <directorio>`. Los fixtures no se incrustan ni se versionan; se generan desde `../Compartido/Scripts/generar_fixtures.sh`.
