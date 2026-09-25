# Fase 0E — especificación de la prueba técnica

## Objetivo

Comparar tecnologías con una pequeña aplicación **desechable de evaluación**, no con código de producción SynTools.

La prueba debe demostrar riesgos reales de SynTools antes de elegir framework.

No se busca implementar un módulo completo ni empezar la migración.

## Candidatos iniciales

- .NET 10 + Avalonia.
- Qt 6 + C++/Qt Quick o Widgets, según se determine en la preparación del prototipo.
- Swift-first solo si se identifica una ruta de UI única razonable antes de invertir en un prototipo completo.

## Regla de igualdad

Los candidatos que lleguen al prototipo deben resolver los mismos escenarios funcionales y usar los mismos fixtures siempre que sea posible.

No se acepta declarar ganador a un candidato por una demo que el otro no haya tenido que implementar.

## Escenario 1 — shell

Debe contener:

- ventana principal;
- barra lateral;
- Inicio;
- módulo de prueba;
- Historial;
- Ajustes;
- tema sistema/claro/oscuro;
- atajos básicos;
- resize;
- fullscreen;
- teclado;
- estados vacíos;
- UI en español.

### Medir

- arranque;
- memoria en reposo;
- fluidez de resize;
- consistencia visual;
- accesibilidad básica;
- integración de menús/ventanas;
- cantidad de código específico por plataforma.

## Escenario 2 — archivos

Debe implementar mediante contratos:

- selector de archivo;
- selector múltiple;
- selector de carpeta;
- drag & drop;
- apertura/revelado de salida;
- temporales;
- conflicto de nombre;
- publicación segura;
- fingerprint/revalidación.

### Pruebas

- rutas Unicode;
- nombres largos razonables;
- conflicto;
- cancelación del selector;
- fichero desaparecido;
- fichero modificado durante la operación;
- volumen/ruta no disponible.

## Escenario 3 — SQLite

Debe incluir:

- settings;
- historial mínimo;
- migración de esquema de ejemplo;
- acceso concurrente razonable;
- reinicio de aplicación y persistencia.

No debe guardar rutas privadas completas en el historial de prueba.

## Escenario 4 — OperationCoordinator

Simular una operación pesada:

- iniciar;
- progreso determinado e indeterminado;
- cancelar;
- impedir una segunda operación incompatible;
- liberar correctamente tras éxito/error/cancelación.

Los tests deben intentar provocar dobles cierres y cancelaciones tardías.

## Escenario 5 — procesos / FFmpeg

Usar un FFmpeg de prueba adecuado a cada plataforma.

Contrato:

- executable separado de argumentos;
- stdout/stderr;
- working directory controlado;
- exit code;
- cancelación;
- timeout;
- captura de proceso hijo.

### Prueba crítica

Ejecutar un harness controlado que cree descendientes.

Al cancelar, verificar que no quedan procesos del harness ejecutándose.

No basta con que el proceso padre termine.

## Escenario 6 — render

Mostrar una gráfica dinámica similar a una waveform/espectrograma simplificado:

- miles de puntos;
- zoom;
- scroll;
- cursor/playhead;
- actualización periódica.

### Medir

- FPS aproximado;
- CPU;
- memoria;
- latencia de interacción;
- facilidad para limitar redibujos.

## Escenario 7 — audio

FFmpeg genera/decodifica PCM y el backend reproduce un archivo de fixture.

Debe soportar:

- play;
- pause;
- resume;
- seek;
- stop;
- volumen;
- posición;
- buffer acotado;
- cancelación/cierre.

### Medir

- latencia play/pause;
- latencia seek;
- estabilidad;
- CPU;
- memoria.

No se exige paridad completa del Inspector.

## Escenario 8 — vídeo

Mostrar vídeo local controlado por un playhead:

- play/pause;
- seek;
- resize;
- aspect ratio;
- fullscreen;
- cierre/cancelación;
- archivo 1080p de prueba;
- una prueba 4K si el entorno lo permite.

### Medir

- latencia;
- FPS;
- CPU/GPU observable;
- memoria;
- comportamiento de resize;
- complejidad de compartir posición con audio/timeline.

## Escenario 9 — integración externa mínima

- copiar texto al portapapeles;
- abrir una URL solo por acción explícita;
- revelar una carpeta;
- almacenar y borrar un secreto sintético mediante el backend seguro de plataforma.

Nunca usar credenciales reales.

## Escenario 10 — empaquetado

Generar artefactos instalables o ejecutables autocontenidos:

### macOS

- Apple Silicon;
- aplicación lanzable;
- inspeccionar dependencias empaquetadas;
- registrar pasos de firma/notarización aunque no se disponga aún de credenciales de distribución.

### Windows

- Windows 11 x64;
- aplicación lanzable sin SDK instalado;
- inspeccionar dependencias empaquetadas;
- registrar estrategia de firma/instalación.

## Métricas obligatorias

Para cada candidato:

- líneas/archivos de código común;
- líneas/archivos específicos macOS;
- líneas/archivos específicos Windows;
- tamaño de artefacto;
- memoria al iniciar;
- memoria durante prueba multimedia;
- tiempo de arranque aproximado;
- latencia play/pause/seek;
- CPU durante render/audio/vídeo;
- tiempo de build;
- errores/bugs encontrados;
- complejidad subjetiva documentada con ejemplos concretos;
- dependencias añadidas;
- licencias de dependencias.

No se convertirán estas mediciones en un único número artificial.

## Criterios de descarte

Un candidato puede descartarse si:

- obliga a mantener dos UIs;
- no puede producir build real en ambas plataformas;
- no puede cancelar procesos descendientes de forma verificable;
- requiere instalar runtimes/herramientas manualmente al usuario final sin una razón aprobada;
- hace inviable el Inspector por latencia/rendimiento;
- introduce una licencia incompatible con el futuro decidido para SynTools;
- obliga a degradar reglas de seguridad/privacidad;
- necesita una cantidad desproporcionada de código de plataforma para funciones normales.

## Resultado de la prueba técnica

Crear un informe por candidato con:

1. entorno exacto;
2. versiones;
3. funcionalidades completadas;
4. métricas;
5. fallos;
6. divergencias;
7. dependencias/licencias;
8. calidad macOS;
9. calidad Windows;
10. implicaciones para el Inspector;
11. implicaciones de mantenimiento.

Después se presentará una recomendación técnica, pero la elección seguirá necesitando aprobación explícita del usuario.

## Código de la prueba técnica

El código de evaluación debe mantenerse separado del código futuro de producción.

No debe convertirse silenciosamente en SynTools definitivo.

Si se decide reutilizar una parte, se revisará como código nuevo de producción antes de incorporarla.
