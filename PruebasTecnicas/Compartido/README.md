# Recursos compartidos de evaluación

Todo este árbol es experimental y no productivo. Los fixtures se generan localmente y no contienen datos personales.

- `Scripts/generar_fixtures.sh`: crea audio, vídeo, rutas Unicode, conflictos y entradas grandes.
- `Scripts/process_tree_harness.py`: proceso controlado que crea descendientes y registra sus PID.
- `Scripts/verificar_fixtures.sh`: comprueba que los artefactos sintéticos son reproducibles.
- `Resultados/`: informes versionables; mediciones locales crudas quedan ignoradas.

Los scripts requieren Bash, Python 3 y FFmpeg/FFprobe. No realizan conexiones de red.
