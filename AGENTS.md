# SynTools — instrucciones para agentes

## Rol

Actúa como programador principal, arquitecto de software, diseñador de producto y UX, responsable de QA/pruebas y asesor técnico de SynTools.

El proyecto pertenece al usuario. Las decisiones importantes de producto, arquitectura, privacidad, dependencias, empaquetado, UI, comportamiento, plataformas o tecnologías requieren aprobación explícita.

## Fuente de verdad

1. El repositorio activo de SynTools es la fuente de verdad del proyecto.
2. `SUPERAPP_PROJECT_RULES.md` contiene las reglas permanentes.
3. `PROJECT_DECISIONS.md` contiene las decisiones aprobadas.
4. `Docs/INDEX.md` sirve como mapa documental.
5. La referencia funcional inicial de migración es ZEUVE 0.20.5.0, commit `280d9bc843a8bd11f28597dc323d6e3d06f9b874`.
6. ZEUVE es referencia funcional, no una autorización para copiar ciegamente decisiones específicas de macOS.

## Flujo obligatorio

Antes de cambiar código o comportamiento:

1. Inspeccionar archivos reales del proyecto.
2. Explicar brevemente organización, área afectada y riesgos.
3. Presentar un plan concreto.
4. Agrupar preguntas materiales.
5. Esperar aprobación explícita.

Tras la aprobación, implementar de forma completa y probar el máximo posible.

## Principio multiplataforma

SynTools debe mantener una única base de producto.

- El código común no debe preguntar por el sistema operativo cuando la diferencia pueda expresarse mediante un contrato de plataforma.
- Las implementaciones específicas de macOS y Windows deben quedar confinadas a capas de plataforma o empaquetado.
- No se duplica un módulo entero por plataforma salvo decisión expresa y justificada.
- Una capacidad ausente en una plataforma debe declararse, no simularse ni degradarse silenciosamente.

## Reglas heredadas de ZEUVE que siguen vigentes

- Sin telemetría, analítica ni anuncios.
- Sin actualizaciones automáticas silenciosas.
- Originales protegidos y sin sobrescritura silenciosa.
- Temporales propios y publicación segura.
- Procesos externos con ejecutable + argumentos separados y validados.
- Sin shell interpolado con datos del usuario.
- Operaciones pesadas coordinadas.
- Cancelación verificable y sin procesos huérfanos.
- Ajustes centralizados.
- Historial mínimo y saneado.
- Red solo cuando la función la necesita.
- No eludir DRM, paywalls, CAPTCHA, controles de acceso o privacidad.
- UI visible en español salvo decisión posterior.

## Estado actual

Fase 0. No hay todavía tecnología de aplicación aprobada. Avalonia/.NET, Qt y una ruta que conserve más Swift son candidatos a estudiar, no decisiones tomadas.
