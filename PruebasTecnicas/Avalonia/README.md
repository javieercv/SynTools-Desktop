# Prueba técnica Avalonia

Prototipo desechable con .NET 10, Avalonia 12.1.3 y Microsoft.Data.Sqlite 10.0.12. No es arquitectura productiva.

```bash
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test SynTools.Probe.Tests/SynTools.Probe.Tests.csproj
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet run --project SynTools.Probe.App/SynTools.Probe.App.csproj
```

El proyecto separa UI de contratos comprobables. El backend audiovisual completo no se da por resuelto: la prueba usa FFmpeg para diagnóstico/decodificación y el render compartido para medir la superficie; la salida de audio real y la sincronización A/V se registran como riesgo pendiente.
