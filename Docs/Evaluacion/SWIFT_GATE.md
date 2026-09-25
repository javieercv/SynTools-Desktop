# Fase 0E — puerta técnica Swift

## Estado

**En ejecución.**

## Hallazgo que justifica la prueba

Swift está soportado oficialmente en Windows. Además, SwiftCrossUI ofrece actualmente:

- un backend AppKit para macOS;
- un backend WinUI para Windows;
- `DefaultBackend` para seleccionar el backend por plataforma;
- navegación dividida;
- diálogos de archivo;
- color schemes;
- menús y otras primitivas desktop.

Esto hace que una arquitectura Swift-first con UI única sea técnicamente plausible y merezca una prueba real.

## Reserva importante

SwiftCrossUI se describe a sí mismo como **work in progress**. No se considerará apto para SynTools solo porque compile.

Debe demostrar:

- estabilidad;
- cobertura de controles;
- calidad macOS/Windows;
- accesibilidad;
- rendimiento;
- extensibilidad;
- integración con funciones que todavía no abstrae.

## Primera prueba

Se implementa una shell común con:

- `NavigationSplitView`;
- estado compartido;
- selector de archivo;
- cambio de tema;
- cuatro secciones conceptuales;
- tests de lógica común.

## Hallazgo preliminar: drag & drop

En la revisión del framework fijada para esta prueba no se ha encontrado una API pública común equivalente a `dropDestination`/recepción de archivos arrastrados.

Esto no descarta Swift, pero obliga a comprobar si:

1. puede implementarse como una abstracción propia pequeña en AppKit/WinUI;
2. entra en el roadmap de SwiftCrossUI;
3. su ausencia revela otras carencias de escritorio relevantes.

No se introducirá una implementación específica en esta primera puerta de compilación.

## Dependencias de evaluación

- SwiftCrossUI, solo para esta prueba.
- Acciones de CI equivalentes a las utilizadas por el propio proyecto SwiftCrossUI para validar macOS y Windows.

No son todavía dependencias de producción.

## Criterio para continuar

Si el mismo código de UI compila y los tests pasan en macOS y Windows, Swift seguirá como candidato y se ampliará la prueba hacia:

- procesos;
- SQLite;
- render;
- audio/vídeo;
- packaging.

Si la UI básica ya requiere divergencias importantes o resulta inestable, se documentará y Swift-first podrá descartarse antes de invertir más.
