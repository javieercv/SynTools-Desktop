# Decisiones aprobadas — SynTools

## 2026-09-25 — creación del proyecto

### Identidad

- Nombre comercial: **SynTools**.
- Repositorio: `SynTools-Desktop`.
- Proyecto independiente de `ZEUVE-SuperApp`.
- Repositorio público.

### Objetivo

SynTools tendrá una única base de código de producto y generará aplicaciones para macOS y Windows.

No se desarrollarán dos versiones funcionales independientes salvo una decisión futura expresa.

### Baseline

Referencia funcional inicial:

- `javieercv/ZEUVE-SuperApp`
- ZEUVE 0.20.5.0
- commit `280d9bc843a8bd11f28597dc323d6e3d06f9b874`

Antes de trasladar un módulo se comprobará también el estado más reciente de ZEUVE.

### Plataformas

Primera etapa:

- macOS 14+ Apple Silicon.
- Windows 11 x64.

La arquitectura debe permitir Windows ARM64 posteriormente.

Fuera del alcance inicial: Linux, web y móvil.

### Módulos

Objetivo inicial:

- Organizador.
- Descargador universal.
- Analizador de chats.
- Conversor universal.
- Comparador de seguidores de Instagram.
- Inspector multimedia.

El Limpiador queda excluido de momento.

### Tecnología

No se ha elegido todavía.

Candidatos iniciales para investigación:

- .NET + Avalonia.
- C++/Qt.
- Ruta multiplataforma que conserve una parte mayor de Swift si demuestra ventajas suficientes.

La decisión se tomará después de una evaluación y una prueba técnica real.

### Principios heredados

Se mantienen como requisitos de producto:

- privacidad local;
- ausencia de telemetría y anuncios;
- protección de originales;
- publicación segura;
- coordinación de operaciones pesadas;
- cancelación;
- ejecución segura de motores;
- ajustes centralizados;
- historial minimizado;
- UI en español;
- red solo cuando la función lo requiera.

### ZEUVE-SuperApp

No se modifica para soportar SynTools.

Durante la transición seguirá siendo la implementación macOS funcional de referencia.
