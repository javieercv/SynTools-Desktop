# Fase 0E — prueba técnica C++20 + Qt 6

> Estado: prototipo experimental, no arquitectura productiva ni licencia aprobada.

## Alcance implementado

`PruebasTecnicas/Qt` usa CMake, C++20 y Qt Quick/QML. Suma 16 archivos de código/build y 413 líneas físicas.

- misma shell, secciones, UI española, tema, resize, fullscreen, atajo, pickers y drag & drop que la prueba Avalonia;
- waveform QML Canvas con 20.000 puntos, zoom, playhead y actualizaciones a 30 Hz;
- fingerprint/revalidación/conflicto/publicación segura;
- SQLite `settings`, `history`, `schema_version` y saneado;
- coordinación de operación pesada;
- `QProcess` con argumentos separados, streams, working directory, timeout y árbol;
- FFmpeg CLI para diagnóstico/progreso y extracción controlada de frames;
- PCM en memoria hacia `QAudioSink`, con play/pause/resume/seek/stop/volumen, posición y buffer solicitado de 48.000 bytes;
- vídeo 1080p mediante FFmpeg por frame hacia una superficie QML, sin `QMediaPlayer`.

No se mezclaron Widgets y QML. Qt Widgets aparece como dependencia transitiva del despliegue Homebrew, no como capa de UI escrita para el prototipo.

## Código común y plataforma específica

La UI, lógica, almacenamiento, audio y vídeo escritos son comunes. `ProcessRunner.cpp` concentra las diferencias:

- Unix/macOS: grupo de proceso con `setpgid` y señal al PGID;
- Windows: `CREATE_NEW_PROCESS_GROUP` y `taskkill /T /F` con argumentos separados.

Son aproximadamente 12 líneas condicionales de 413 (≈3% del prototipo). En una aplicación real aparecerán otros adapters para revelar archivos, credenciales, ventanas, sandbox, firma e instalador, pero no hay indicios aquí de duplicar módulos completos. Estimación arquitectónica: 90–95% común si estas diferencias permanecen tras contratos.

## Pruebas automáticas

QtTest ejecutó 9 casos contando init/cleanup, sin omisiones:

- coordinador;
- SQLite y saneado;
- Unicode/modificación/desaparición/conflicto;
- argumentos separados y stdout/stderr;
- timeout y comprobación de PID de padre/descendiente;
- FFmpeg real con progreso `progress=end` y cancelación;
- decodificación de frames 1080p y seek a 3 s.

Resultado local final: **9 correctos, 0 fallidos, 0 omitidos**, 2.173 ms; CTest: **1/1 suite correcta**, 2,81 s.

## Render, audio y vídeo

Qt proporciona una ruta especialmente directa hacia buffers: `QAudioSink` acepta PCM y `QVideoSink`/Qt Quick pueden recibir frames. La documentación confirma que Qt Multimedia usa FFmpeg como backend principal en la mayoría de plataformas y que `QAudioSink` sirve para audio raw: [Qt Multimedia](https://doc.qt.io/qt-6/qtmultimedia-index.html).

La prueba no usa `QMediaPlayer`. El audio PCM y los controles están implementados, pero no se completó una sesión auditiva final ni mediciones de latencia. El vídeo implementado es deliberadamente sencillo: lanza FFmpeg por frame cada 100 ms. Demuestra control de seek/superficie/aspect ratio, pero no es una arquitectura eficiente ni prueba sincronización A/V, aceleración, colas o 4K.

Una versión inicial del Canvas a 60 Hz consumió 92,8–97,7% CPU y 126–131 MB RSS. Ese dato descubrió un problema real de la implementación de alto nivel; se cambió a framebuffer y 30 Hz, pero la sesión quedó bloqueada antes de repetir métricas. No debe atribuirse a Qt en general ni presentarse como métrica final optimizada.

## Builds y packaging macOS

- Qt 6.11.2 Homebrew, Apple Clang 21, CMake 4.4.3, Ninja 1.13.2.
- configuración inicial: 4,13 s.
- build final incremental: correcto, sin avisos de código.
- binario principal: 218.080 bytes, enlazado dinámicamente.
- `macdeployqt`: completó una vez en ≈95 s y la repetición final en 217,64 s; reescribió dependencias hacia `@executable_path`.
- bundle desplegado: 119 MB (`du`: 122.052 KiB).
- firma ad hoc y verificación profunda: correctas.
- ejecutable de desarrollo: lanzado y permaneció estable antes del bloqueo de sesión.
- bundle final: QA manual visual pendiente.

El despliegue arrastró numerosos frameworks/estilos/plugins QML; puede podarse, pero ilustra que el ejecutable pequeño no representa el coste de distribución. La documentación oficial exige desplegar plugins y dependencias por plataforma: [despliegue Qt](https://doc.qt.io/qt-6.10/deployment.html).

## Windows y ARM64

Windows 11 x64 está preparado en CI con Qt 6.10/MSVC. La ruta de cancelación usa mecanismos Windows explícitos. No hubo ejecución manual. Qt 6.10 declara Windows on ARM64 con MSVC 2022, sin ARM64EC; el código no bloquea ARM64 deliberadamente. Fuente: [plataformas Qt 6.10](https://doc.qt.io/qt-6.10/supported-platforms.html).

## Licencia

Módulos usados: Core, Gui, Quick, Qml, QuickControls2, Sql, Multimedia y Test. Qt ofrece licencia comercial o, para estos módulos, opciones open source LGPLv3/GPL; algunos módulos Qt no usados son solo GPL/comercial. La distribución dinámica y posibilidad efectiva de reemplazar/relinkar las librerías son parte del análisis LGPL; además deben conservarse avisos, fuentes/ofertas aplicables y licencias de terceros. No se ha elegido modalidad. Fuentes: [licencia Qt 6.10](https://doc.qt.io/qt-6.10/licensing.html) y [obligaciones LGPL publicadas por Qt](https://www.qt.io/development/open-source-lgpl-obligations).

Qt Multimedia puede distribuir FFmpeg dinámico y advierte sobre variantes/licencias y patentes de codecs. El prototipo también usa el ejecutable FFmpeg Homebrew, cuya configuración local no se propone para distribución. Fuente: [licencia FFmpeg](https://ffmpeg.org/legal.html).

## Complejidad y conclusión

Qt reduce el riesgo técnico de superficies y audio directo, aporta APIs desktop maduras y ofrece un camino claro para el Inspector. El coste observado es C++ (más disciplina de memoria/concurrencia), QML + C++ como dos lenguajes, cancelación específica por plataforma, bundle complejo y una decisión legal/comercial material. Es el candidato con menor incertidumbre multimedia, pero no necesariamente el de menor mantenimiento total.
