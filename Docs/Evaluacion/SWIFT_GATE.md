# Fase 0E — puerta técnica Swift

> Resultado técnico: **no superar el filtro Swift-first con la evidencia actual**.
> Esto no es una decisión de producto ni descarta Swift para componentes aislados.

## Pregunta y respuesta

La pregunta no era si Swift compila en Windows, sino si puede ser el lenguaje principal con una UI única macOS + Windows sin una pila más compleja que Avalonia o Qt.

Swift 6.4 dispone de toolchain oficial Windows x64 y ARM64, SwiftPM y SourceKit-LSP. Foundation y la interoperabilidad C/C++ permiten lógica y motores comunes. El bloqueo está en la UI desktop: no existe una UI oficial de Swift que cubra macOS y Windows; las rutas disponibles son un framework joven con backends, dos UIs o Swift + C++ + Qt/QML + bridge. La primera no cubre aún contratos de escritorio necesarios y la última conserva Swift a costa de añadir casi toda la complejidad de Qt.

## Tecnologías estudiadas

| Ruta | Estado observado | Conclusión para SynTools |
| --- | --- | --- |
| Swift toolchain oficial | Windows y macOS soportados; SwiftPM en ambos; Windows x64/ARM64 | Adecuado para código sin UI; no resuelve UI |
| SwiftCrossUI | MIT; `DefaultBackend` usa AppKit y WinUI; actividad actual; proyecto todavía joven | Única ruta Swift pura plausible, elegida para prototipo mínimo |
| SwiftOpenUI | MIT; backends Win32/macOS recientes | Demasiado nuevo para basar una aplicación desktop exigente |
| Shaft | BSD-3-Clause; UI Swift multiplataforma | Evidencia insuficiente de calidad/soporte desktop Windows productivo |
| Qt Bridge for Swift | Early preview/beta; Qt 6.10+, Swift 6.2+, C++ interop y QML | Introduce Swift + C++ + Qt + bridge + QML; falla el criterio de simplicidad |
| UI distinta AppKit/WinUI | Técnicamente posible | Incumple la UI única mantenida una vez |

Fuentes: [plataformas Swift](https://www.swift.org/platform-support/), [instalación Windows](https://www.swift.org/install/windows/), [estado de interoperabilidad C++](https://www.swift.org/documentation/cxx-interop/status/), [SwiftCrossUI](https://github.com/moreSwift/swift-cross-ui), [Qt Bridge for Swift](https://github.com/qt/qtbridge-swift), [SwiftOpenUI](https://github.com/codelynx/swiftopenui) y [Shaft](https://github.com/ShaftUI/Shaft).

## Prototipo mínimo

`PruebasTecnicas/Swift` fija SwiftCrossUI al commit `0f3ec3958b79cdc39a1a3e604516b71ab33a9043`. Incluye una shell compartida con ventana, sidebar, Inicio/Herramienta/Historial/Ajustes, resize, tema y selector de archivo, más un módulo de estado comprobable.

- 4 archivos Swift/manifest, 210 líneas físicas.
- Código escrito específico macOS: 0 líneas.
- Código escrito específico Windows: 0 líneas.
- La selección del backend está delegada a `DefaultBackend`.
- Dependencias resueltas: 24 repositorios transitivos; `.build` local llegó a 1,6 GB.
- Licencia del framework principal: MIT; cada dependencia transitiva necesitaría auditoría antes de aprobación.

## Builds y ejecución

| Comprobación | Resultado |
| --- | --- |
| `swift package resolve` macOS | Correcto; primera resolución 129,89 s |
| `swift test` macOS | 3/3 correctos; última ejecución 22,37 s incluyendo compilación incremental |
| `swift build -c release` macOS | Correcto; 32,60 s reales en la última ejecución; binario 10.078.336 bytes |
| Advertencias | Avisos reiterados de conformances AppKit no importadas a través de `DefaultBackend` |
| Lanzamiento del binario release | Falló antes de crear ventana: `dyld` no encontró `_swift_initBorrow` en el runtime Swift del sistema |
| Windows x64 | Preparado en GitHub Actions con Swift 6.3.3 estable; no ejecutado manualmente |
| Windows ARM64 | Toolchain disponible; prototipo no compilado ni ejecutado |

La incompatibilidad `dyld` puede ser propia de la combinación Xcode 27/Swift 6.4 y macOS instalada, no demuestra por sí sola un defecto del framework. Sí demuestra que “compila” no basta como puerta de distribución.

## Cobertura desktop y huecos

El prototipo verificó construcción de ventana, layout, estado, selección y tema a nivel de compilación. La revisión del backend fijado encontró:

- sin API común localizada para recibir drag & drop de archivos;
- sin API común localizada para portapapeles;
- picker común básico, pero sin demostrar todos los contratos de carpeta/múltiple;
- TODO y operaciones no soportadas en el backend WinUI para capacidades concretas de ventana/gestos/pickers;
- accesibilidad, revelar en Finder/Explorer, SQLite, procesos y packaging aún requerirían adapters o dependencias adicionales.

Por ello se aplicó el filtro solicitado y no se implementaron audio/vídeo, SQLite ni procesos en Swift.

## Código común y estimación de una aplicación real

En el prototipo, el código escrito es prácticamente 100% común porque las capacidades ausentes se dejaron sin implementar. Esa cifra no puede extrapolarse. Como estimación arquitectónica, no medición, una SynTools completa podría conservar aproximadamente 70–80% de modelos/lógica/UI declarativa y exigir 20–30% de backends, adapters e integración para cerrar archivos, ventanas, accesibilidad, multimedia, packaging y comportamiento Windows. Ese coste se sumaría al riesgo de los backends de terceros.

Del ZEUVE actual podría reutilizarse selectivamente lógica Swift sin dependencias Apple: modelos, parsers, validaciones, planificación y algunos tests. Código ligado a SwiftUI/AppKit, AVFoundation, Vision, Accelerate, VideoToolbox, PDFKit o ImageIO no sería reutilización multiplataforma directa.

## Conclusión técnica

Swift serviría bajo una de dos arquitecturas:

1. SwiftCrossUI más adapters propios AppKit/WinUI, aceptando completar y sostener huecos del framework.
2. Swift como lógica sobre Qt/QML mediante C++ interop y Qt Bridge.

La primera concentra riesgo en una dependencia joven y la segunda es objetivamente más estratificada que usar Qt directamente. Con la evidencia actual, Swift-first no minimiza mantenimiento y no justifica ampliar esta prueba. Swift permanece como opción para paquetes aislados si una futura migración demuestra una reutilización neta clara.
