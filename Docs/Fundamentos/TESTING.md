# Estrategia de pruebas

## Objetivo

SynTools debe demostrar tanto comportamiento común como integración real en cada plataforma.

## Capas

### 1. Tests comunes

Deben ejecutar la lógica compartida sin UI y cubrir:

- modelos;
- planners;
- parsers;
- validadores;
- persistencia;
- conflictos;
- cancelación;
- limpieza;
- seguridad;
- contratos de motores.

### 2. Tests de plataforma

macOS y Windows deben validar sus adapters:

- archivos;
- procesos;
- portapapeles;
- almacenamiento seguro;
- rutas;
- ventanas;
- audio/vídeo;
- packaging cuando proceda.

### 3. Tests de paridad

Para comportamiento heredado de ZEUVE, SynTools debe comprobar que la nueva implementación conserva el resultado observable aprobado.

No se exige igualdad interna de código.

### 4. Build real

La compilación común no sustituye:

- build real macOS;
- build real Windows;
- empaquetado;
- lanzamiento;
- pruebas manuales pertinentes.

## Regla de evidencia

No afirmar que una plataforma está validada si solo se ejecutaron tests comunes en otra.
