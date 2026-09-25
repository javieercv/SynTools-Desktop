# REGLAS PERMANENTES DE SYNTOOLS

## 1. Control del proyecto

SynTools pertenece al usuario. Las decisiones relevantes deben presentarse y aprobarse antes de aplicarse.

Requieren aprobación, entre otras:

- arquitectura;
- lenguaje y framework;
- sistema de UI;
- dependencias;
- motores externos;
- privacidad y red;
- almacenamiento;
- empaquetado e instalación;
- plataformas objetivo;
- comportamiento visible;
- opciones del usuario;
- manejo de archivos;
- compatibilidad;
- cambios de alcance.

Los detalles internos menores que no alteren esas áreas pueden resolverse durante una implementación ya aprobada.

## 2. Flujo antes de modificar

Antes de cambiar código o comportamiento:

1. inspeccionar la versión real;
2. explicar organización y elementos afectados;
3. presentar plan técnico;
4. agrupar preguntas materiales;
5. esperar aprobación explícita.

Una petición funcional no sustituye la aprobación del plan.

## 3. Un producto, dos plataformas

SynTools se diseña para macOS y Windows desde una única base.

Principios:

- La lógica de negocio debe ser común.
- La UI debe ser común salvo excepciones aprobadas.
- Las diferencias de sistema se aíslan detrás de servicios de plataforma.
- No se dispersarán condicionales de plataforma por los módulos cuando exista una abstracción razonable.
- Los contratos compartidos deben expresar capacidad y comportamiento, no APIs concretas del sistema operativo.
- Una implementación específica puede diferir internamente siempre que conserve el contrato observable aprobado.

## 4. Plataforma inicial

Objetivos iniciales aprobados:

- macOS 14+ Apple Silicon.
- Windows 11 x64.

Preparar la arquitectura para Windows ARM64 sin convertirlo en requisito de la primera versión.

Fuera de alcance inicial:

- Linux.
- web.
- móvil.

## 5. Fuente funcional de referencia

SynTools parte funcionalmente de ZEUVE 0.20.5.0, commit:

`280d9bc843a8bd11f28597dc323d6e3d06f9b874`

ZEUVE-SuperApp sigue siendo un proyecto independiente y funcional.

Antes de migrar un módulo se debe comprobar la versión más reciente de ZEUVE por si el comportamiento aprobado ha evolucionado desde el baseline.

La referencia ZEUVE se usa para conservar:

- comportamiento;
- reglas;
- privacidad;
- validaciones;
- flujos;
- decisiones de UX;
- casos límite;
- pruebas conceptuales.

No convierte en requisito de SynTools una implementación específica de Apple.

## 6. Protección de archivos

Regla general:

- originales intactos salvo que una función aprobada tenga precisamente como objetivo moverlos o retirarlos;
- lectura antes de escritura;
- temporales propiedad de la operación;
- validación del resultado;
- política de conflictos;
- publicación segura;
- limpieza solo de temporales verificablemente propios;
- sin sobrescritura silenciosa.

## 7. Privacidad

Prohibido introducir sin una nueva decisión:

- telemetría;
- analítica;
- anuncios;
- tracking;
- perfiles externos;
- informes automáticos a terceros;
- actualización silenciosa;
- persistencia innecesaria de URLs, tokens, cookies, credenciales o contenido privado.

Las funciones locales deben seguir siendo locales.

## 8. Procesos y motores

Los motores externos deben ejecutarse con:

- ejecutable identificado;
- argumentos separados;
- validación de entradas;
- captura controlada de salida;
- cancelación;
- terminación de procesos descendientes cuando corresponda;
- sin interpolación de shell con datos del usuario.

La implementación concreta de procesos puede ser distinta en macOS y Windows, pero el contrato de seguridad debe ser común.

## 9. Operaciones pesadas

SynTools conservará el principio de coordinación global de operaciones pesadas.

El mecanismo definitivo se diseñará en la arquitectura, pero debe garantizar:

- una operación principal pesada cuando así lo exija la política global;
- estado coherente;
- cancelación;
- liberación garantizada en éxito, error y cancelación;
- ausencia de dobles reservas o dobles cierres.

## 10. Ajustes e historial

Las preferencias persistentes deben centralizarse.

El historial debe contener únicamente información operativa necesaria y saneada. No debe convertirse en un archivo de rutas privadas, URLs completas, tokens, cookies, contenidos de chats, OCR o datos personales.

## 11. Dependencias

Antes de introducir una dependencia significativa se debe documentar:

- finalidad;
- necesidad;
- alternativas;
- plataformas;
- arquitectura CPU;
- tamaño;
- mantenimiento;
- funcionamiento offline;
- empaquetado;
- firma;
- permisos;
- impacto en instalación.

## 12. Implementación completa

Tras aprobar un plan no se entregarán como solución definitiva:

- TODOs necesarios para funcionar;
- botones falsos;
- mocks;
- pseudocódigo;
- demos aisladas;
- progreso ficticio;
- rutas incompletas.

## 13. Preservar comportamiento aprobado

No se aprovechará una migración para cambiar silenciosamente funciones ya aprobadas.

Cuando una limitación de plataforma obligue a una divergencia, debe documentarse y aprobarse.

## 14. Limpiador

El Limpiador de ZEUVE queda fuera del alcance inicial de SynTools.

No condicionará la arquitectura común. Su eventual incorporación se decidirá por separado.
