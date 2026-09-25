# Arquitectura objetivo de SynTools

> Estado: propuesta aprobada a nivel conceptual. La tecnología concreta todavía no está elegida.

## Objetivo

Mantener un solo producto y una sola implementación funcional para macOS y Windows.

## Capas previstas

```text
SynTools.App
    UI y composición compartidas
        |
Modules
    lógica y modelos de cada herramienta
        |
SynTools.Core
SynTools.Storage
SynTools.Operations
SynTools.Engines
        |
SynTools.Platform
    Common contracts
      /        \
   macOS     Windows
```

## SynTools.App

Responsable de:

- navegación;
- Inicio;
- Ajustes;
- Historial;
- ayuda contextual;
- composición visual de módulos.

La intención es compartir la UI entre macOS y Windows.

## SynTools.Core

Contratos y tipos transversales:

- manifiestos de módulos;
- permisos;
- capacidades;
- errores;
- rutas lógicas;
- fingerprints;
- logging local;
- navegación;
- modelos operativos;
- contratos de plataforma.

No debe depender de una API gráfica o de sistema concreta.

## SynTools.Storage

Persistencia local:

- SQLite;
- Ajustes;
- Historial;
- migraciones;
- presets/favoritos cuando proceda.

Debe mantener contratos comunes. El backend elegido deberá estar disponible en ambas plataformas.

## SynTools.Operations

Coordina operaciones pesadas y cancelación.

Debe preservar las garantías aprendidas en ZEUVE:

- propiedad única del ciclo;
- reserva coherente;
- cancelación local/global;
- liberación en todos los caminos;
- ausencia de estados falsamente disponibles.

## SynTools.Engines

Responsable de:

- registro de motores;
- localización;
- diagnóstico;
- verificación;
- ejecución segura;
- procesos;
- progreso;
- cancelación;
- compatibilidad por plataforma/arquitectura.

Los contratos de un motor son comunes aunque el binario empaquetado sea distinto.

## SynTools.Platform

Único lugar donde deben concentrarse las diferencias inevitables:

- selección de archivos/carpetas;
- apertura de carpeta o URL;
- portapapeles;
- almacenamiento seguro;
- integración de ventanas;
- notificaciones;
- rutas del sistema;
- ejecución/terminación de procesos cuando el SO lo exija;
- audio/vídeo nativo si se utiliza;
- firma/packaging;
- Papelera u otras capacidades del sistema.

## Modules

Los módulos no deben depender de otros módulos funcionales.

Objetivo inicial:

- Organizer
- UniversalDownloader
- ChatAnalyzer
- UniversalConverter
- InstagramFollowers
- MultimediaInspector

El Cleaner queda fuera de la primera etapa.

## Regla de plataforma

Un módulo no debe contener ramas `if macOS / if Windows` dispersas cuando la diferencia pueda modelarse como una dependencia de plataforma.

Ejemplo conceptual:

```text
IClipboard
  -> MacClipboard
  -> WindowsClipboard
```

El consumidor usa `IClipboard` y no conoce el sistema operativo.

## Excepciones

Puede existir una capacidad verdaderamente exclusiva de plataforma, pero debe:

1. declararse explícitamente;
2. tener UI honesta;
3. no degradarse silenciosamente;
4. documentarse;
5. aprobarse.
