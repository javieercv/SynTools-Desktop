# Decisiones pendientes de Fase 0

Este archivo registra decisiones materiales que todavía no deben darse por resueltas.

## 1. Tecnología principal

Pendiente tras Fase 0E. No se ha aprobado candidato.

Candidatos:

- .NET 10 + Avalonia.
- C++ + Qt.
- Swift-first queda desaconsejado técnicamente con la evidencia actual, pero no se convierte en descarte aprobado.

Antes de decidir Avalonia frente a Qt faltan:

- puerta multimedia comparable con salida PCM y vídeo 1080p controlado;
- métricas repetibles de render/audio/vídeo;
- QA manual en macOS 14+ y Windows 11 x64;
- confirmar si macOS 14 Tier 2 de Avalonia es aceptable;
- análisis legal/distributivo de Qt y FFmpeg.

## 2. Licencia de SynTools

El repositorio es público, pero todavía no se ha aprobado una licencia de código.

No añadir `LICENSE` por defecto.

La decisión es especialmente relevante si se elige Qt, porque sus opciones open source y comerciales tienen obligaciones distintas.

## 3. Versión mínima concreta de Windows 11

Está aprobado Windows 11 x64, pero no se ha fijado todavía build mínimo.

Avalonia diferencia actualmente:

- Windows 11 24H2 como Tier 1;
- Windows 11 22H2 como Tier 2.

La decisión se tomará después de la evaluación y de valorar cuánto soporte hacia atrás interesa.

## 4. Grado de apariencia nativa

Está aprobado compartir UI, pero todavía debe definirse si SynTools priorizará:

- apariencia prácticamente idéntica entre sistemas; o
- identidad común con pequeñas adaptaciones de convenciones macOS/Windows.

No implica dos UIs.

## 5. Distribución

Pendiente:

- firma Windows;
- formato de instalador Windows;
- notarización/firma macOS;
- canal de actualizaciones si algún día se aprueba;
- stores frente a distribución directa.

No existe actualización automática aprobada.

## 6. Motores en Windows

Los motores comunes previstos necesitarán builds y políticas de verificación Windows:

- FFmpeg/FFprobe;
- yt-dlp;
- Deno;
- gallery-dl;
- instaloader-zeuve;
- Pandoc opcional cuando corresponda.

No se fija todavía el formato de empaquetado.

## 7. Backend multimedia del Inspector

Pendiente decidir:

- si Avalonia usa un backend pequeño de audio y superficies propias sobre FFmpeg;
- si Qt usa APIs de buffers directos o partes de Qt Multimedia;
- estrategia de sincronización, aceleración, codecs y límites 4K;
- umbrales aceptables de latencia, FPS, CPU y memoria.

La Fase 0E no demostró todavía un pipeline final equivalente en ambos candidatos.
