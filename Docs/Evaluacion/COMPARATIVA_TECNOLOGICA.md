# Fase 0E — comparativa tecnológica basada en pruebas

> Esta comparativa formula una recomendación técnica. **No aprueba una tecnología** ni inicia arquitectura productiva.

## Resumen de evidencia

| Factor | Swift + SwiftCrossUI | .NET 10 + Avalonia | C++20 + Qt Quick |
| --- | --- | --- | --- |
| UI compartida escrita | Sí, mínima | Sí | Sí |
| Resultado local de tests | 3/3 | 6/6 | 9/9 contando init/cleanup |
| Lanzamiento macOS | Bloqueado por incompatibilidad `dyld` | Sí en build de desarrollo; bundle final pendiente | Sí en build de desarrollo; bundle final pendiente |
| Windows x64 | CI preparado, no QA | CI build/test/publicación preparado, no QA | CI build/test preparado, no QA |
| Código de prototipo | 210 LOC | 528 LOC | 413 LOC |
| Código propio específico | 0, dejando huecos sin implementar | 0 | ≈12 LOC de procesos |
| Bundle macOS | No generado | 108 MB autocontenido | 119 MB dinámico desplegado |
| Procesos/SQLite/archivos | No, filtro detenido | Implementados y probados | Implementados y probados |
| Audio directo | No | PCM generado; salida pendiente | `QAudioSink` implementado; QA/medición pendiente |
| Vídeo controlado | No | Pendiente | FFmpeg por frame implementado; no optimizado |
| Riesgo principal | Madurez/cobertura UI y bindings | Inspector multimedia + calidad macOS 14 | C++ + licencia + packaging |

## Swift

Podría conservarse como lenguaje principal únicamente aceptando SwiftCrossUI más adapters AppKit/WinUI propios, o Qt Bridge más C++/QML. El prototipo tiene código fuente común, pero omite precisamente drag & drop, portapapeles, folder picker completo, reveal, accesibilidad y multimedia. Para producto se estima 70–80% común y 20–30% de backends/integración, con alta incertidumbre.

No es más sencillo que Avalonia o Qt: SwiftCrossUI desplaza trabajo al proyecto, y Qt Bridge añade una capa sin eliminar Qt. De ZEUVE solo es razonable reutilizar lógica libre de frameworks Apple; conservar código por sí mismo no compensa una UI y motores más frágiles.

## Avalonia

Es la ruta más pequeña conceptualmente: C# para UI/lógica/tests, contratos comunes y cero código específico escrito hasta ahora. La shell se lanzó en macOS y permitió encontrar un fallo real, pero la sesión bloqueada impidió una revisión manual final; no puede afirmarse todavía que “se sienta” suficientemente macOS. Windows compila en el diseño y se valida por CI, pero queda pendiente QA Windows 11 real.

Render simple y procesos son viables. El Inspector no está demostrado: faltan salida PCM, vídeo controlado y medidas. Dependencias mínimas directas: Avalonia, tema Fluent, SQLite y FFmpeg externo/embebido por decidir. El objetivo macOS 14 es Tier 2 de Avalonia, dato material.

## Qt

Qt simplifica audio, superficies y futura integración de frames: `QAudioSink`, Qt Quick y el ecosistema multimedia reducen el número de piezas desconocidas. Aun así, el prototipo por-frame no resuelve sincronización ni rendimiento. La integración macOS de desarrollo fue funcional; Windows queda sin QA.

El coste es concreto: C++ y QML, adapters de árbol de procesos, más disciplina de memoria/concurrencia, despliegue de plugins/frameworks y análisis LGPLv3/comercial. Aproximadamente 97% del prototipo es común; una aplicación real podría sostener 90–95% si lo específico continúa aislado.

## Mantenimiento, filosofía y riesgos

- **Menor mantenimiento general previsto:** Avalonia, por lenguaje administrado, tests concisos y contratos comunes, condicionado a resolver multimedia sin una proliferación de bindings.
- **Mejor encaje con la filosofía de SynTools:** Avalonia y Qt cumplen una UI/lógica compartidas y aislamiento explícito. Avalonia lo hace con menos código de plataforma observado.
- **Menor riesgo para el Inspector:** Qt, por acceso directo a audio raw, superficies y ecosistema C++/FFmpeg. La prueba aún no mide un pipeline final.
- **Menor riesgo de packaging/desarrollo:** Avalonia mostró una publicación autocontenida más directa. Qt produjo un bundle correcto, pero `macdeployqt` añadió numerosos plugins y tardó entre ≈95 y 218 s en dos ejecuciones. Firma/notarización e instalador Windows siguen pendientes en ambos.
- **Swift-first:** no supera la puerta actual; mantenerlo como candidato principal añadiría más riesgo que reutilización demostrada.

## Decisiones que siguen pendientes

1. Avalonia frente a Qt como base de la siguiente prueba/arquitectura.
2. Aceptabilidad de macOS 14 Tier 2 en Avalonia.
3. Modalidad LGPLv3 o comercial si se elige Qt.
4. Diseño y licencia de FFmpeg/codecs.
5. Backend de audio/vídeo y objetivos medibles del Inspector.
6. QA real Windows 11 x64 y macOS desbloqueado.
7. Versión mínima exacta de Windows, apariencia nativa, firma, notarización e instaladores.

## Recomendación técnica, no decisión aprobada

Mantener **Avalonia como candidato preferente provisional** por coste de mantenimiento y alineación arquitectónica, y **Qt como alternativa de menor riesgo multimedia**. Antes de decidir, ejecutar una puerta corta y comparable: salida PCM, vídeo 1080p con seek/sincronización, waveform optimizada, métricas repetibles, QA macOS 14 y Windows 11, y auditoría inicial de distribución/licencias.

Si Avalonia no alcanza los umbrales multimedia o de integración macOS, Qt pasa a ser la opción técnicamente más defendible. No se recomienda invertir más en Swift-first salvo que cambie materialmente la cobertura de UI desktop.
