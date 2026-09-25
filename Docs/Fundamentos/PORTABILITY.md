# Estrategia de portabilidad

## Clasificación

Cada componente que se migre desde ZEUVE debe clasificarse como:

- **COMMON**: trasladable conceptualmente sin dependencia de plataforma.
- **ADAPTABLE**: misma función y contrato, backend distinto.
- **PLATFORM-SPECIFIC**: función estrechamente ligada a un sistema.
- **DEFERRED**: fuera del alcance inicial.
- **RETIRED**: no se trasladará.

## Matriz inicial

| Área | Clasificación | Nota |
| --- | --- | --- |
| Manifests/módulos | COMMON | El contrato se rediseñará sin dependencia Swift |
| Ajustes | COMMON | Backend SQLite común |
| Historial | COMMON | Modelo común y minimizado |
| OperationCoordinator | COMMON | Reimplementar contrato |
| FFmpeg/FFprobe | ADAPTABLE | Mismos contratos, binarios por plataforma |
| yt-dlp | ADAPTABLE | Empaquetado/diagnóstico por plataforma |
| Deno | ADAPTABLE | Binario y packaging por plataforma |
| gallery-dl / instaloader | ADAPTABLE | Requiere preparación por plataforma |
| Selección de archivos | ADAPTABLE | UI/servicio de plataforma |
| Portapapeles | ADAPTABLE | Servicio de plataforma |
| Almacenamiento de secretos | ADAPTABLE | Keychain vs equivalente Windows |
| Procesos | ADAPTABLE | POSIX vs Windows process/job semantics |
| AVFoundation | PLATFORM-SPECIFIC | No puede ser dependencia del Core |
| Vision | PLATFORM-SPECIFIC | Sustituir o abstraer |
| Accelerate/vDSP | PLATFORM-SPECIFIC | Sustituir o abstraer |
| VideoToolbox | PLATFORM-SPECIFIC | Optimización macOS |
| ImageIO/PDFKit | PLATFORM-SPECIFIC | Sustituir por rutas comunes o adapters |
| Spotlight | PLATFORM-SPECIFIC | No trasladar como concepto común |
| Cleaner | DEFERRED | Fuera de la primera etapa |

## Principio

La sustitución de una tecnología no autoriza a cambiar el comportamiento del producto.

Ejemplo:

- COMMON: «OCR local, revisable y sin red».
- LEGACY-ZEUVE: «OCR implementado con Apple Vision».

SynTools conserva el primero y puede sustituir el segundo.
