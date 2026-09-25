# Evaluación tecnológica de SynTools

> Estado: investigación de Fase 0D, 2026-09-25.  
> No existe todavía una tecnología aprobada.  
> Esta comparación no sustituye la prueba técnica real de Fase 0E.

## Objetivo

Elegir una base tecnológica capaz de mantener **una única aplicación SynTools** para macOS y Windows, con el máximo código funcional y visual compartido y sin degradar las garantías ya aprobadas en ZEUVE.

La elección debe servir no solo para los primeros módulos, sino también para los subsistemas más exigentes del Inspector multimedia: procesos, audio, vídeo, render, FFT, OCR, archivos grandes, cancelación y baja latencia.

## Requisitos no negociables para cualquier candidato

- macOS 14+ Apple Silicon.
- Windows 11 x64.
- camino razonable hacia Windows ARM64.
- UI compartida.
- lógica de negocio compartida.
- SQLite.
- procesos externos sin shell interpolado.
- cancelación segura y control de descendientes.
- integración con FFmpeg/FFprobe y demás motores aprobados.
- publicación segura de archivos.
- drag & drop y selectores de archivos/carpetas.
- empaquetado instalable sin obligar al usuario a instalar herramientas de desarrollo.
- funcionamiento local para funciones locales.
- ausencia de telemetría/ads.
- posibilidad de tests comunes y tests de plataforma.

---

# Candidato A — .NET 10 + Avalonia

## Estado de plataforma

Avalonia soporta Windows y macOS desde una misma base de UI.

Estado publicado actualmente por Avalonia:

- Windows 11 24H2: x64/ARM64, Tier 1.
- Windows 11 22H2: x64/ARM64, Tier 2.
- macOS 26: ARM64/x64, Tier 1.
- macOS 15 y macOS 14: ARM64/x64, Tier 2.
- backend Windows basado directamente en Win32.
- backend macOS nativo Objective-C++ propio.

.NET 10 es LTS y Microsoft publica soporte hasta noviembre de 2028. .NET 10 admite macOS 14 y Windows 11, incluidos x64 y Arm64 en Windows.

## Ventajas para SynTools

- C#/.NET es adecuado para lógica de negocio, concurrencia, SQLite, parsing y procesos.
- UI Avalonia compartida con XAML/C#.
- Licencia MIT del framework Avalonia.
- `System.Diagnostics.Process` dispone de terminación de árbol de procesos, aunque SynTools deberá verificar contractualmente que los descendientes han terminado realmente.
- Buen soporte de render personalizado mediante `DrawingContext` y acceso a Skia para operaciones más avanzadas.
- Separación natural en proyectos `Core`, `Storage`, `Operations`, `Engines`, `Platform` y módulos.
- El runtime .NET puede distribuirse junto con la aplicación para no exigir instalación previa al usuario.
- Windows ARM64 no exigiría un cambio de arquitectura conceptual.

## Riesgos a demostrar

- macOS 14 es Tier 2 en Avalonia, no Tier 1.
- Debe probarse la calidad real de menús, ventanas, fullscreen, atajos, accesibilidad y comportamiento de escritorio en macOS.
- Avalonia no debe convertirse en el motor multimedia de SynTools por defecto; el Inspector necesita backends explícitos y medidos.
- Debe probarse latencia de audio, vídeo, seek y render con una arquitectura FFmpeg + superficies/backends propios.
- Debe comprobarse tamaño final de distribución self-contained y tiempos de arranque.
- Cualquier dependencia adicional para audio, vídeo, PDF, imágenes u OCR debe evaluarse por separado.

## Hipótesis a validar

Avalonia puede ser una buena capa de aplicación/UI mientras SynTools mantiene multimedia, procesos y otras capacidades críticas detrás de contratos propios.

---

# Candidato B — C++ + Qt 6

## Estado de plataforma

Qt 6.10 publica soporte para:

- macOS 13+ en x86_64 y ARM64.
- Windows 11 x86_64.
- Windows on ARM ARM64 con MSVC 2022.

Qt ofrece Widgets y Qt Quick/QML para interfaces, además de APIs de escritorio maduras.

Qt Multimedia usa actualmente FFmpeg como backend predeterminado salvo excepciones de plataforma, lo que resulta especialmente relevante para el Inspector.

## Ventajas para SynTools

- Trayectoria muy madura en aplicaciones desktop complejas.
- Soporte oficial sólido tanto en macOS como Windows.
- Potente render y Qt Quick/QML.
- Excelente interoperabilidad con C/C++ y bibliotecas nativas.
- Multimedia integrada con backend FFmpeg disponible.
- Buen control de procesos, filesystem, ventanas y gráficos.
- Windows ARM64 ya forma parte de las configuraciones soportadas.
- Puede reducir la cantidad de adapters necesarios para algunas funciones de escritorio y multimedia.

## Riesgos a demostrar

- C++ aumenta el coste de seguridad de memoria y complejidad frente a C#/Swift para gran parte de la lógica de producto.
- Hay que decidir entre Widgets y Qt Quick/QML; mezclar ambos sin una razón clara aumentaría complejidad.
- La política de cancelación de árboles de procesos de SynTools deberá validarse igualmente por plataforma.
- La licencia es una decisión material:
  - Qt dispone de licencia comercial;
  - gran parte del framework está disponible bajo LGPLv3/GPL;
  - algunos componentes pueden tener condiciones diferentes;
  - usar LGPL implica obligaciones de distribución y relinkado que deben estudiarse antes de producir software.
- Si SynTools no quiere quedar condicionado por LGPL/GPL, habría que estudiar una licencia comercial antes de comenzar desarrollo de producción.
- La elección de módulos Qt concretos puede cambiar obligaciones de licencia y dependencias.

## Hipótesis a validar

Qt puede ser la opción técnicamente más fuerte para escritorio/multimedia, pero solo será aceptable si su coste de complejidad y su modelo de licencia encajan con el futuro comercial de SynTools.

---

# Candidato C — conservar Swift como base principal

## Estado de plataforma

Swift tiene soporte oficial de desarrollo y despliegue en Windows. El toolchain oficial actual ofrece Windows x86_64 y ARM64, Swift Package Manager, SourceKit-LSP y debugger.

Sin embargo, ese soporte no convierte SwiftUI/AppKit en frameworks de Windows. Apple documenta SwiftUI como tecnología para sus plataformas Apple.

## Posibles enfoques

1. Swift común + UI distinta por plataforma.
2. Swift común + framework UI de terceros multiplataforma.
3. Swift común + Qt/u otra UI mediante FFI.
4. Reutilizar solo algunos paquetes de lógica Swift.

## Ventajas para SynTools

- Permitiría conservar más conocimiento y posiblemente parte de la lógica Swift existente.
- Swift 6 mantiene un buen modelo de concurrencia y seguridad.
- SwiftPM funciona oficialmente en Windows.
- Puede ser útil para componentes concretos incluso si no termina siendo el lenguaje principal.

## Riesgos

- La opción 1 incumple el objetivo de una única UI mantenida una vez.
- Las opciones 2/3 añaden una capa o ecosistema adicional y reducen el beneficio de conservar Swift.
- SwiftUI, AppKit, AVFoundation, Vision, Accelerate, VideoToolbox, PDFKit e ImageIO siguen necesitando sustitución.
- El ecosistema de escritorio Windows en Swift es menor que .NET o Qt.
- Packaging, UI, multimedia y bindings pueden convertirse en una combinación más compleja que una reimplementación común.
- Preservar Swift no debe convertirse en un objetivo superior a reducir mantenimiento futuro.

## Hipótesis a validar

Swift puede conservar valor en lógica reutilizable, pero debe demostrar que una arquitectura Swift-first no obliga a mantener dos interfaces o una pila de bindings difícil de sostener.

---

# Comparación previa a la prueba

| Factor | .NET + Avalonia | C++ + Qt | Swift-first |
| --- | --- | --- | --- |
| Una UI compartida | Sí, objetivo central | Sí | Requiere framework adicional o bindings |
| macOS ARM64 | Sí | Sí | Sí |
| Windows x64 | Sí | Sí | Sí |
| Windows ARM64 futuro | Sí | Sí | Sí en toolchain |
| macOS 14 | Avalonia Tier 2 / .NET soportado | Qt soportado | Swift soportado |
| Licencia base UI | MIT | LGPL/GPL o comercial | Depende de UI elegida |
| Complejidad de lógica general | Baja-media | Alta | Media |
| Multimedia de escritorio | Requiere diseñar backend | Muy fuerte; Qt Multimedia/FFmpeg | Requiere resolver fuera de APIs Apple |
| Render personalizado | Sí | Sí, muy fuerte | Depende de UI |
| Procesos | .NET + adapters cuando haga falta | Qt + adapters cuando haga falta | Foundation/WinSDK + adapters |
| Reutilización directa de Swift actual | Baja | Baja | Potencialmente mayor |
| Riesgo de dos UIs | Bajo | Bajo | Alto si no se añade UI común |
| Cuestión de licencia | Baja para Avalonia | Material | Depende de dependencias |
| Prueba crítica | calidad macOS + multimedia | licencia + complejidad | UI común + ecosistema Windows |

Esta tabla no asigna puntuaciones ni ganador porque faltan mediciones reales.

---

# Fuentes oficiales consultadas

- Avalonia — Supported platforms: https://docs.avaloniaui.net/docs/supported-platforms
- Avalonia — repositorio/licencia MIT: https://github.com/AvaloniaUI/Avalonia
- Avalonia — custom rendering: https://docs.avaloniaui.net/docs/graphics-animation/custom-rendering
- Microsoft — ciclo de vida de .NET: https://learn.microsoft.com/lifecycle/products/microsoft-net-and-net-core
- Microsoft — .NET en macOS: https://learn.microsoft.com/dotnet/core/install/macos
- Microsoft — .NET en Windows: https://learn.microsoft.com/dotnet/core/install/windows
- Microsoft — Process.Kill: https://learn.microsoft.com/dotnet/api/system.diagnostics.process.kill
- Qt — Supported Platforms 6.10: https://doc.qt.io/qt-6.10/supported-platforms.html
- Qt — Multimedia: https://doc.qt.io/qt-6/qtmultimedia-index.html
- Qt — Licensing: https://doc.qt.io/qt-6.8/licensing.html
- Qt — LGPL obligations: https://www.qt.io/development/open-source-lgpl-obligations
- Swift — Platform Support: https://www.swift.org/platform-support/
- Swift — Windows install: https://www.swift.org/install/windows/
- Apple — SwiftUI apps: https://developer.apple.com/documentation/technologyoverviews/swiftui

## Siguiente fase

La elección final se aplaza hasta ejecutar la prueba técnica definida en `TECHNICAL_SPIKE_SPEC.md`.
