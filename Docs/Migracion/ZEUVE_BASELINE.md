# Baseline de migración desde ZEUVE

## Referencia fijada

- Repositorio: `javieercv/ZEUVE-SuperApp`
- Versión: `0.20.5.0`
- Commit: `280d9bc843a8bd11f28597dc323d6e3d06f9b874`
- Fecha del commit: 2026-09-25

## Uso del baseline

El baseline sirve para extraer:

- alcance funcional;
- decisiones aprobadas;
- contratos;
- UX;
- casos límite;
- seguridad;
- privacidad;
- pruebas;
- comportamiento de errores;
- cancelación;
- publicación;
- reglas de motores.

## Regla de frescura

Antes de migrar un módulo, revisar también la versión más reciente de ZEUVE-SuperApp.

Si ha cambiado respecto al baseline:

1. identificar cambios funcionales;
2. distinguir correcciones de comportamiento de detalles macOS;
3. actualizar la especificación SynTools si corresponde;
4. no copiar regresiones ni implementaciones específicas innecesarias.

## Lo que no se hereda automáticamente

No son decisiones de SynTools por el mero hecho de existir en ZEUVE:

- Swift 6;
- SwiftUI;
- AppKit;
- AVFoundation;
- Vision;
- Accelerate/vDSP;
- VideoToolbox;
- ImageIO;
- PDFKit;
- Spotlight;
- Hardened Runtime;
- estructura `.app`;
- rutas `~/Library/...`;
- Mach-O;
- APIs Apple concretas.

Se evaluará el requisito funcional que había detrás de cada una.
