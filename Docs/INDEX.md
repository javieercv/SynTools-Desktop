# Documentación de SynTools

## Fase actual

SynTools está en **Fase 0 — definición y arquitectura**.

## Fuentes principales

| Necesidad | Documento |
| --- | --- |
| Reglas permanentes | `../SUPERAPP_PROJECT_RULES.md` |
| Decisiones aprobadas | `../PROJECT_DECISIONS.md` |
| Arquitectura objetivo | `Fundamentos/ARCHITECTURE.md` |
| Seguridad y privacidad | `Fundamentos/SECURITY.md` |
| Pruebas | `Fundamentos/TESTING.md` |
| Estrategia de portabilidad | `Fundamentos/PORTABILITY.md` |
| Abstracciones de plataforma | `Fundamentos/PLATFORM_ABSTRACTIONS.md` |
| Evaluación tecnológica | `Fundamentos/TECHNOLOGY_EVALUATION.md` |
| Baseline ZEUVE | `Migracion/ZEUVE_BASELINE.md` |
| Auditoría de portabilidad de ZEUVE | `Migracion/PORTABILITY_AUDIT.md` |
| Matriz de paridad | `Migracion/PARITY_MATRIX.md` |
| macOS | `Plataformas/MACOS.md` |
| Windows | `Plataformas/WINDOWS.md` |

## Principio documental

La documentación debe distinguir entre:

- **COMMON**: requisito común de SynTools;
- **MACOS**: detalle específico de macOS;
- **WINDOWS**: detalle específico de Windows;
- **LEGACY-ZEUVE**: detalle del producto Swift/macOS que no necesariamente se trasladará.

Las decisiones de plataforma no deben confundirse con requisitos funcionales.
