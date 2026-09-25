# Catálogo inicial de abstracciones de plataforma

## Objetivo

Definir fronteras que permitan que módulos y lógica común no conozcan si se ejecutan en macOS o Windows.

Los nombres son conceptuales. La tecnología elegida podrá ajustar la sintaxis y nomenclatura, pero no debe eliminar estas responsabilidades sin justificarlo.

## Servicios mínimos

### FilePicker

Responsabilidad:

- seleccionar archivos;
- seleccionar múltiples;
- seleccionar carpetas;
- filtros por tipo/extensión;
- cancelación de diálogo.

No contiene lógica de módulo.

### DragDropInput

Responsabilidad:

- recibir archivos/carpetas desde el sistema;
- normalizar entradas;
- entregar rutas/handles al código común.

Las reglas de aceptación pertenecen al módulo.

### PlatformPaths

Responsabilidad:

- directorio de datos de aplicación;
- caché;
- temporales;
- logs;
- motores externos instalados por el usuario cuando se aprueben.

El código común usa ubicaciones lógicas, no rutas codificadas de macOS o Windows.

### PersistentFolderAccess

Responsabilidad:

- recordar carpetas expresamente autorizadas;
- recuperar acceso;
- distinguir autorización inválida, ruta ausente y backend no disponible.

En macOS puede apoyarse en bookmarks. Windows puede usar otro mecanismo. El contrato común no debe exponer blobs Apple.

### SecureCredentialStore

Responsabilidad:

- guardar, recuperar y borrar secretos expresamente aprobados;
- identificar entradas por clave lógica;
- no exponer secretos en logs.

Implementaciones previstas:

- macOS: Keychain o mecanismo aprobado;
- Windows: almacén seguro equivalente aprobado.

### Clipboard

Responsabilidad:

- copiar texto y, si se aprueba en el futuro, otros tipos.

### ExternalOpen

Responsabilidad:

- abrir una URL por acción explícita;
- revelar/abrir una carpeta o archivo mediante el sistema.

### ProcessRunner

Responsabilidad:

- executable + argumentos separados;
- working directory controlado;
- entorno permitido;
- stdout/stderr;
- código de salida;
- progreso consumible por el llamador;
- cancelación;
- terminación de descendientes;
- timeout cuando el contrato lo requiera.

macOS y Windows pueden necesitar implementaciones muy diferentes.

### FileIdentity

Responsabilidad:

- fingerprint suficiente para revalidación;
- tamaño;
- timestamps/identidad disponible;
- comportamiento documentado ante filesystems con capacidades distintas.

No debe asumir inode como identidad universal.

### SafePublisher

Responsabilidad:

- temporales;
- conflictos;
- publicación atómica o la alternativa segura disponible;
- revalidación;
- limpieza.

La política de producto es común; las primitivas del filesystem pueden variar.

### Trash / RecycleBin

No es requisito inicial para todos los módulos.

Si se utiliza:

- debe mover mediante mecanismo del sistema;
- devolver identidad/ubicación suficiente para saber qué ocurrió;
- no prometer Undo si el backend no puede verificarlo.

### Notifications

Solo si una función aprobada las necesita.

La ausencia de notificaciones no puede romper el flujo principal.

### WindowIntegration

Responsabilidad:

- pantalla completa;
- ventana propietaria;
- comportamiento modal;
- información mínima necesaria para funciones compartidas.

No debe filtrarse un `NSWindow` o `HWND` hacia los módulos.

### AudioOutput

Especialmente importante para Inspector.

Contrato esperado:

- PCM streaming;
- formato declarado;
- buffer acotado;
- play/pause;
- posición;
- control de volumen;
- cierre inmediato;
- baja latencia razonable.

La decodificación puede seguir a cargo de FFmpeg.

### VideoSurface

Contrato esperado:

- presentar frames o superficie de vídeo;
- escalado;
- aspect ratio;
- pantalla completa a través de WindowIntegration;
- actualización eficiente;
- límites de memoria.

La arquitectura no debe imponer que macOS y Windows utilicen el mismo backend de GPU.

### LocalOCR

Contrato esperado:

- imágenes/frames de entrada;
- idioma cuando proceda;
- confianza;
- líneas/bloques;
- cancelación;
- ejecución completamente local.

No se aprueba todavía un motor OCR concreto.

### FFT / SignalProcessing

Contrato esperado:

- FFT necesaria para espectrograma y análisis;
- buffers reutilizables;
- rendimiento suficiente;
- resultados numéricos validados.

Accelerate/vDSP pasa a ser una implementación legacy de ZEUVE, no el contrato.

### ImageCodec

Responsabilidad potencial:

- lectura/escritura de formatos aprobados;
- metadata relevante;
- perfiles de color cuando correspondan;
- calidad/compresión.

Debe evaluarse antes de sustituir ImageIO.

### PdfBackend

Responsabilidad potencial:

- render de páginas;
- extracción de texto;
- creación de PDF cuando el alcance lo exija.

Debe evaluarse antes de sustituir PDFKit.

## Regla de dependencias

La dirección debe ser:

```text
Module / App
      |
Common contract
      |
Platform implementation
```

No:

```text
Module
  -> AppKit / Win32 / Cocoa / Windows API
```

salvo excepción aprobada y documentada.

## Capacidades opcionales

Cada backend deberá poder declarar capacidades reales.

Ejemplo conceptual:

- hardwareVideoDecode;
- secureCredentialStorage;
- nativeNotifications;
- pdfRender;
- localOCR.

Una capacidad ausente debe detectarse antes de ofrecer la función. No se debe descubrir a mitad de una operación después de modificar archivos.

## Pruebas contractuales

Cada abstracción crítica deberá tener una suite común que se ejecute contra macOS y Windows.

Especialmente:

- ProcessRunner;
- FileIdentity;
- SafePublisher;
- PersistentFolderAccess;
- SecureCredentialStore;
- AudioOutput;
- VideoSurface.

La suite de contrato no sustituye QA real de plataforma.
