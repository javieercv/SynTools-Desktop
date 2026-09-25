public enum ProbeSection: String, CaseIterable, Identifiable, Hashable, Sendable {
    case home = "Inicio"
    case tool = "Herramienta de prueba"
    case history = "Historial"
    case settings = "Ajustes"

    public var id: Self { self }

    public var description: String {
        switch self {
        case .home:
            "Shell compartida de SynTools."
        case .tool:
            "Prueba de controles, archivos y estado."
        case .history:
            "Historial sintético; no guarda rutas privadas."
        case .settings:
            "Preferencias de prueba compartidas entre plataformas."
        }
    }
}

public enum ProbePlatform {
    public static let displayName: String = {
        #if os(macOS)
        "macOS · AppKitBackend"
        #elseif os(Windows)
        "Windows · WinUIBackend"
        #else
        "Plataforma de desarrollo no objetivo"
        #endif
    }()
}
