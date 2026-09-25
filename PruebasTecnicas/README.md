# Pruebas técnicas de SynTools

Este directorio contiene prototipos **desechables de evaluación**.

No forman parte del código de producción de SynTools y no deben convertirse silenciosamente en la aplicación definitiva.

## Objetivo

Comprobar con código y builds reales qué tecnología permite mantener una sola SynTools para macOS y Windows sin degradar seguridad, privacidad, rendimiento o mantenibilidad.

## Pruebas realizadas en Fase 0E

- `Swift/`: Swift como lenguaje principal con una UI realmente compartida.
- `Avalonia/`: .NET + Avalonia.
- `Qt/`: C++ + Qt.

La existencia de una prueba no significa que su tecnología esté aprobada.

Los resultados, mediciones y limitaciones están en `../Docs/Evaluacion/`. Los fixtures se generan localmente desde `Compartido/Scripts/` y los artefactos de build no se versionan.
