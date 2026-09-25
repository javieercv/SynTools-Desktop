# Fase 0E — prueba técnica .NET 10 + Avalonia

> Estado: prototipo experimental, no arquitectura productiva ni dependencia aprobada.

## Alcance implementado

`PruebasTecnicas/Avalonia` contiene 12 archivos C#/proyecto y 528 líneas físicas. Todo el código escrito es común; no hay archivos ni condicionales propios de macOS o Windows. Avalonia aporta sus backends nativos.

- shell redimensionable en español con sidebar, cuatro secciones, estados vacíos y tema sistema/claro/oscuro;
- atajos, fullscreen, picker simple/múltiple/carpeta y drag & drop;
- waveform de 20.000 puntos con zoom y playhead periódico;
- contratos de fingerprint SHA-256, revalidación, conflicto y publicación mediante temporal propio;
- SQLite con `settings`, `history`, `schema_version`, WAL, migración y saneado de historial;
- `OperationCoordinator` determinado/indeterminado, exclusión, cancelación, fallo/éxito, cierre tardío y doble cierre;
- `ProcessRunner` con ejecutable y argumentos separados, working directory, stdout/stderr, timeout y terminación de árbol;
- FFmpeg para diagnóstico, progreso y decodificación PCM a temporal propio;
- bundle macOS autocontenido y publicación autocontenida macOS/Windows en CI.

## Pruebas automáticas

La suite xUnit ejecuta 6 tests sin omisiones:

1. coordinación y doble cierre;
2. migración/reinicio/concurrencia SQLite y ausencia de la ruta privada en el fichero;
3. Unicode, modificación, desaparición y conflicto;
4. argumentos separados y captura de stdout/stderr;
5. timeout de un harness padre-hijo y comprobación posterior de cada PID;
6. diagnóstico/progreso/cancelación FFmpeg y decodificación exacta de 8 s PCM estéreo 48 kHz/16 bit (1.536.000 bytes).

Resultado local final: **6 correctos, 0 fallidos, 0 omitidos**, duración informada 2 s.

## Estado de UI e integración

| Área | Estado |
| --- | --- |
| Shell/navegación/resize/tema | Implementado, compilado y lanzado antes del bloqueo de sesión |
| Pickers y drag & drop | Implementados; no recorridos manualmente de extremo a extremo tras la corrección final |
| Fullscreen/atajos | Implementados; QA manual final pendiente |
| Abrir temporal | Implementado mediante launcher de Avalonia |
| Reveal específico Finder/Explorer | No implementado; requeriría servicio de plataforma |
| Accesibilidad | No auditada |
| Waveform | Implementada; lanzamiento observado, métricas finales repetibles pendientes |

Avalonia documenta Windows 11 24H2 x64/ARM64 y macOS 26 como Tier 1; Windows 11 22H2 y macOS 14/15 son Tier 2. El objetivo mínimo macOS 14 queda por tanto soportado en best effort, un riesgo de producto que no debe ocultarse. Fuente: [plataformas soportadas](https://docs.avaloniaui.net/docs/supported-platforms). Sus APIs de almacenamiento cubren archivo, múltiple, carpeta y bookmarks, aunque el comportamiento concreto debe validarse en cada sandbox: [StorageProvider](https://docs.avaloniaui.net/docs/services/storage/storage-provider).

## Render y multimedia

La superficie custom de Avalonia/Skia es viable para una waveform simple. En una ejecución framework-dependent se observó aproximadamente 139–170 MB RSS en reposo y 0% CPU una vez estable. No se obtuvo una captura final fiable de FPS/CPU/latencia porque macOS quedó bloqueado y Avalonia Native no pudo iniciar el render timer (`-6661`).

FFmpeg está integrado como proceso controlado para diagnóstico/progreso/PCM, pero el prototipo no incluye todavía:

- backend real de salida PCM;
- play/pause/resume/seek/volumen audible;
- superficie de vídeo con buffers, sincronización y seek;
- medidas CPU/RAM/latencia de audio o vídeo 1080p/4K.

Por tanto, el Inspector es arquitectónicamente plausible con FFmpeg + backend propio, pero **no está demostrado** por esta prueba. Éste es el mayor riesgo pendiente de Avalonia.

## Builds y packaging macOS

- .NET SDK 10.0.401; runtime 10.0.12; Avalonia 12.1.3.
- build Release incremental final: 0,46 s reales, 0 advertencias, 0 errores.
- build incremental anterior comparable: 1,36 s.
- publicación autocontenida medida: 1,80 s en caliente.
- empaquetado final completo (restore + publish + bundle + firma): 6,14 s.
- `.app` autocontenida `osx-arm64`: 108 MB (`du`: 110.664 KiB).
- firma ad hoc: `codesign --verify --deep --strict` correcta.
- lanzamiento framework-dependent: realizado; se detectó y corrigió un error de árbol visual por reutilizar el mismo control.
- lanzamiento/QA manual del bundle final: pendiente por sesión macOS bloqueada.

## Windows y ARM64

El proyecto no contiene una dependencia intencionadamente x64. Publica `win-x64` autocontenido en CI y Avalonia declara ARM64. Windows no se ejecutó manualmente en esta máquina. El resultado de CI acredita build/tests, no calidad visual, audio, accesibilidad ni integración manual.

## Dependencias y licencias

Dependencias directas: .NET 10, Avalonia/Avalonia.Desktop/Avalonia.Themes.Fluent 12.1.3 (MIT) y Microsoft.Data.Sqlite 10.0.12 (MIT). Entre las transitivas relevantes están SkiaSharp, HarfBuzzSharp y SQLitePCLRaw; antes de producción se requiere SBOM y auditoría de redistribución. FFmpeg se usa como ejecutable externo de prueba y su configuración concreta de distribución puede ser LGPL o GPL: [consideraciones legales de FFmpeg](https://ffmpeg.org/legal.html).

## Complejidad y conclusión

C# permitió expresar contratos y tests con poca plataforma propia; el árbol de procesos se resolvió con una API común y verificación real. El paquete autocontenido es directo pero grande. Avalonia es el candidato que mejor reduce complejidad general y aislamiento de plataforma en esta prueba. Su candidatura depende de una segunda puerta centrada en Inspector multimedia, calidad macOS 14 y QA Windows real.
