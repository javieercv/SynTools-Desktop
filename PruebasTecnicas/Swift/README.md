# Prueba técnica Swift

## Pregunta

¿Puede SynTools conservar Swift como lenguaje principal y mantener **una única UI** para macOS y Windows sin dos implementaciones visuales?

## Ruta evaluada

- Swift.
- Swift Package Manager.
- SwiftCrossUI.
- `DefaultBackend`.
- macOS: AppKitBackend.
- Windows: WinUIBackend.

SwiftCrossUI queda fijado para esta prueba al commit:

`0f3ec3958b79cdc39a1a3e604516b71ab33a9043`

No es una dependencia aprobada para SynTools.

## Qué demuestra este prototipo

El mismo archivo de UI intenta proporcionar en ambas plataformas:

- ventana principal;
- navegación lateral;
- Inicio;
- herramienta de prueba;
- Historial;
- Ajustes;
- estado;
- selector de archivos;
- tema sistema/claro/oscuro.

La lógica mínima de modelo vive en `SwiftProbeCore` y no depende de la UI.

## Qué no demuestra todavía

- drag & drop: la API común revisada de SwiftCrossUI no expone actualmente una abstracción equivalente;
- SQLite;
- FFmpeg;
- árboles de procesos;
- audio;
- vídeo;
- render pesado;
- OCR;
- empaquetado final;
- accesibilidad completa;
- comportamiento visual manual.

Esas pruebas solo se añadirán si esta primera puerta de entrada compila razonablemente en macOS y Windows.

## Ejecución local

```bash
swift build
swift test
swift run SynToolsSwiftProbe
```

En Windows se requiere el toolchain Swift y el entorno de Visual Studio/Windows SDK necesarios para Swift/WinUI.

## CI

`.github/workflows/swift-technical-probe.yml` compila y ejecuta los tests en:

- macOS;
- Windows.

Un build correcto prueba compatibilidad de compilación; no sustituye una prueba manual de la interfaz.
