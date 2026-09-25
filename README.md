# SynTools

**SynTools** es la nueva evolución multiplataforma del producto desarrollado hasta ahora como ZEUVE.

El objetivo del proyecto es mantener **una única base de código de producto** capaz de generar aplicaciones de escritorio para:

- macOS 14+ en Apple Silicon.
- Windows 11 x64.

Windows ARM64 queda preparado como objetivo futuro, sin formar parte de la primera versión. Linux, web y móvil quedan fuera del alcance inicial.

## Estado

SynTools se encuentra en **Fase 0 — definición y arquitectura**. Todavía no se ha elegido la tecnología definitiva ni se ha iniciado la implementación de la aplicación.

La referencia funcional inicial es:

- Proyecto origen: `javieercv/ZEUVE-SuperApp`
- Versión: ZEUVE `0.20.5.0`
- Commit de referencia: `280d9bc843a8bd11f28597dc323d6e3d06f9b874`

ZEUVE-SuperApp continúa como proyecto independiente y no se modifica como consecuencia del desarrollo de SynTools.

## Principio principal

SynTools debe ser **un solo producto**, no dos aplicaciones mantenidas en paralelo.

La lógica de producto, interfaz compartida, módulos, almacenamiento, operaciones y contratos de motores deben ser comunes siempre que sea razonable. Las diferencias inevitables de sistema operativo se aíslan detrás de servicios de plataforma.

## Alcance inicial de módulos

Previstos para la nueva base común:

- Organizador
- Descargador universal
- Analizador de chats
- Conversor universal
- Comparador de seguidores de Instagram
- Inspector multimedia

El **Limpiador** queda fuera del alcance inicial porque depende intensamente del sistema operativo. Su futuro se decidirá por separado.

## Documentación

Empieza por [`Docs/INDEX.md`](Docs/INDEX.md).
