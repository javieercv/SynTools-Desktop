# Seguridad y privacidad de SynTools

## Privacidad

SynTools no incorporará:

- telemetría;
- analítica;
- anuncios;
- tracking;
- actualización automática silenciosa;
- envío automático de diagnósticos;
- persistencia innecesaria de secretos.

## Red

Cada función debe declarar si necesita red.

Las funciones locales deben seguir funcionando offline.

Credenciales, cookies, tokens y cabeceras privadas no deben aparecer en logs, historial ni presets.

## Archivos

Patrón general:

1. validar entrada;
2. capturar identidad/fingerprint cuando proceda;
3. trabajar en temporal propio;
4. revalidar antes de publicar o modificar;
5. validar salida;
6. resolver conflictos;
7. publicar;
8. limpiar solo artefactos propios.

No se sobrescriben originales silenciosamente.

## Procesos

No se construyen comandos de shell interpolados con datos del usuario.

Los procesos deben iniciarse mediante APIs seguras de plataforma con ejecutable y argumentos separados.

La cancelación debe terminar los procesos relevantes y evitar descendientes huérfanos.

## Archivos comprimidos

Mantener protecciones contra:

- path traversal;
- rutas absolutas;
- colisiones normalizadas;
- expansión insegura;
- consumo de recursos no acotado cuando sea viable imponer límites.

## Logs e historial

Solo información técnica necesaria y saneada.

Evitar:

- rutas completas;
- URLs firmadas;
- headers;
- cookies;
- tokens;
- conversaciones;
- OCR completo;
- listas privadas.
