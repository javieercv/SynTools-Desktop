# Plataforma macOS

## Objetivo inicial

- macOS 14 o posterior.
- Apple Silicon.

## Principio

SynTools no debe depender globalmente de APIs Apple. Las APIs nativas pueden usarse dentro de adapters macOS cuando aporten una ventaja y exista un contrato común.

## Áreas previsiblemente específicas

- integración de ventanas;
- diálogos nativos si se eligen;
- Keychain;
- firma/notarización;
- rutas y convenciones macOS;
- posibles aceleraciones con VideoToolbox;
- APIs audiovisuales si finalmente se utilizan.

## Referencia

ZEUVE-SuperApp sirve como referencia funcional y como fuente de experiencia sobre macOS, pero SynTools no está obligado a mantener SwiftUI/AppKit ni otras tecnologías Apple.
