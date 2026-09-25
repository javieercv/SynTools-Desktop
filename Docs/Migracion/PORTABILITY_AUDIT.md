# Auditoría de portabilidad desde ZEUVE 0.20.5.0

## Objetivo

Esta auditoría clasifica el estado de ZEUVE 0.20.5.0 desde el punto de vista de SynTools. No evalúa qué framework usar; identifica qué comportamiento puede ser común y qué implementaciones actuales están ligadas a macOS.

Referencia:

- repositorio: `javieercv/ZEUVE-SuperApp`;
- versión: `0.20.5.0`;
- commit: `280d9bc843a8bd11f28597dc323d6e3d06f9b874`.

## Resumen ejecutivo

ZEUVE tiene una separación útil entre aplicación, núcleo, almacenamiento, operaciones, motores y módulos. Esa forma conceptual debe preservarse.

El mayor acoplamiento a macOS se concentra en:

- UI SwiftUI/AppKit;
- selección/ventanas/portapapeles;
- audio y algunas superficies multimedia;
- OCR Vision;
- FFT Accelerate/vDSP;
- ImageIO/PDFKit;
- Spotlight/Security.framework y semántica de aplicaciones del Limpiador;
- bookmarks y rutas del sistema;
- proceso POSIX y firma Mach-O;
- build, firma y packaging Apple.

En cambio, una parte importante de la lógica funcional ya está expresada como modelos, planners, parsers, validadores, políticas y contratos de motores. Esos comportamientos son buenos candidatos para una reimplementación común.

## Capas transversales

| Área ZEUVE | Portabilidad | Tratamiento SynTools |
| --- | --- | --- |
| `ZEUVECore` conceptual | Alta | Reimplementar contratos comunes |
| `ZEUVEStorage` / SQLite | Alta | Backend común |
| `ZEUVEOperations` | Alta | Mantener semántica de coordinación |
| Registro/diagnóstico de motores | Alta | Común con packaging por plataforma |
| Ejecución de procesos | Media | Contrato común, backend macOS/Windows |
| UI SwiftUI/AppKit | Baja directa | Sustituir por UI multiplataforma |
| Rutas/bookmarks | Media | Contratos comunes + adapters |
| Firma/inspección Mach-O | Nula en Windows | Servicio específico de plataforma |
| Manifests y permisos | Alta conceptual | Redefinir en tipos neutrales |

## Organizador

### Portabilidad conceptual: alta

Común:

- escaneo dentro de alcance autorizado;
- categorías y reglas;
- preview;
- planificación;
- conflictos;
- deduplicación;
- selección;
- ejecución;
- Undo verificable;
- exportación CSV;
- ajustes;
- historial.

Adaptaciones:

- definición de paquetes/bundles;
- rutas especiales;
- semántica de filesystem y nombres;
- enlaces/symlinks/reparse points;
- diálogo de carpetas;
- integración con Finder/Explorer.

Riesgo principal: no trasladar literalmente reglas de paquetes macOS a Windows. La política común debe ser «no atravesar contenedores especiales de forma inesperada» y cada plataforma define cuáles son.

## Descargador universal

### Portabilidad conceptual: alta

Común:

- normalización de entradas;
- routing;
- perfiles;
- presets;
- análisis;
- builders/parsers de motores;
- reglas de sesión;
- privacidad;
- progreso;
- publicación;
- validación;
- fallbacks;
- historial.

Adaptaciones:

- binarios de yt-dlp, Deno, FFmpeg/FFprobe, gallery-dl e instaloader;
- rutas de caché/datos;
- almacenamiento seguro de sesiones recordadas;
- importación explícita de cookies cuando proceda;
- ejecución y cancelación de procesos;
- firma y empaquetado.

Riesgo principal: procesos y secretos. El comportamiento público-antes-que-credenciales debe seguir siendo idéntico.

## Analizador de chats

### Portabilidad conceptual: muy alta

Común:

- parsing;
- detección de formatos;
- catálogo ZIP;
- seguridad de archivos comprimidos;
- SQLite temporal;
- deduplicación;
- filtros;
- búsquedas;
- métricas;
- agregados;
- paginación;
- cancelación;
- privacidad;
- historial.

Adaptaciones:

- selector/drag & drop;
- libarchive o sustituto común;
- rutas temporales;
- integración de UI.

Riesgo principal: rendimiento real con datasets grandes y comportamiento del backend de archivos comprimidos.

## Conversor universal

### Portabilidad conceptual: alta con backends relevantes

Común:

- detección;
- compatibilidad;
- planners;
- recetas;
- lotes;
- políticas de nombre;
- conflictos;
- workspaces;
- publicación;
- FFmpeg/FFprobe;
- Pandoc opcional;
- validación;
- progreso/cancelación;
- favoritos/presets.

Específico actual de Apple:

- ImageIO;
- PDFKit;
- VideoToolbox;
- partes de render/salida nativa.

Decisión futura necesaria: sustituir ImageIO/PDFKit por una ruta común o mantener adapters equivalentes con paridad comprobada.

Riesgo principal: que una sustitución de biblioteca cambie calidad, metadata, perfiles de color o formatos disponibles sin hacerlo explícito.

## Comparador de seguidores de Instagram

### Portabilidad conceptual: muy alta

Común:

- inspección segura de ZIP;
- localización de JSON;
- tolerancia de variantes;
- normalización;
- comparación;
- categorías;
- exportación CSV;
- privacidad.

Adaptaciones:

- file picker;
- apertura manual de URL;
- backend de ZIP/libarchive.

Es el mejor candidato para primer módulo funcional de SynTools porque ejercita importación, parsing, archivos comprimidos, UI, exportación y publicación segura con poco acoplamiento al sistema.

## Inspector multimedia

### Portabilidad conceptual: alta; complejidad de implementación: muy alta

Común:

- inspección FFprobe;
- modelos de streams;
- compatibilidad;
- planners;
- edición estructural;
- FFmpeg command builders;
- validación;
- publicación;
- lotes;
- reglas;
- informes;
- sonoridad EBU R128;
- señal;
- silencios/clipping;
- análisis de fuente con pérdida;
- timeline;
- presets/favoritos;
- identidad de sesión;
- límites de memoria;
- backpressure;
- política de una única fuente temporal/playhead.

Backends a sustituir o abstraer:

- AVAudioEngine/AVAudioPlayerNode;
- AppKit y superficies de imagen/ventana;
- Vision OCR;
- Accelerate/vDSP;
- VideoToolbox;
- ImageIO cuando proceda.

FFmpeg ya reduce mucho el acoplamiento porque audio PCM, frames BGRA, inspección, remux y análisis pueden conservar contratos comunes.

Riesgo principal: latencia y calidad del preview. La nueva arquitectura debe probar reproducción, seek, pausa, sincronización y consumo antes de aprobar tecnología.

## Limpiador

### Estado: DEFERRED

Es el módulo más específico de plataforma:

- inventario de aplicaciones;
- bundles y Bundle ID;
- Security.framework;
- Team ID/App Groups;
- Spotlight;
- LaunchAgents;
- Application Support/Containers;
- Papelera macOS;
- semántica de desinstaladores.

No debe condicionar la arquitectura inicial de SynTools.

Si se recupera en el futuro, será un módulo funcional común solo a nivel de UX/seguridad, con descubrimiento y ejecución profundamente específicos por plataforma.

## Prioridad de migración recomendada

1. Infraestructura común mínima.
2. Comparador de seguidores de Instagram.
3. Analizador de chats.
4. Organizador.
5. Descargador universal.
6. Conversor universal.
7. Inspector multimedia.
8. Limpiador: decisión separada futura.

Este orden no es una decisión irreversible de producto. Reduce riesgo técnico y permite probar la arquitectura común con funciones reales antes de abordar multimedia compleja.

## Conclusión

La viabilidad de una base única no depende de poder compilar el Swift actual en Windows. Depende de preservar contratos funcionales comunes y confinar las integraciones de plataforma.

La prueba tecnológica debe concentrarse especialmente en los subsistemas que más condicionan al Inspector y al Descargador: procesos, cancelación, multimedia, render, filesystem, almacenamiento seguro y empaquetado.
