import DefaultBackend
import Foundation
import SwiftCrossUI
import SwiftProbeCore

private enum ThemeMode: String {
    case system = "Sistema"
    case light = "Claro"
    case dark = "Oscuro"

    var colorScheme: ColorScheme? {
        switch self {
        case .system:
            nil
        case .light:
            .light
        case .dark:
            .dark
        }
    }
}

private struct ProbeRootView: View {
    @State private var selectedSection: ProbeSection? = .home
    @State private var selectedFile: URL?
    @State private var theme: ThemeMode = .system

    @Environment(\.chooseFile) private var chooseFile

    var body: some View {
        NavigationSplitView {
            VStack {
                Text("SynTools")
                    .font(.system(size: 24))

                Text("Prueba técnica Swift")
                    .font(.system(size: 14))

                Divider()

                List(ProbeSection.allCases, selection: $selectedSection) { section in
                    Text(section.rawValue)
                }

                Spacer()

                Text(ProbePlatform.displayName)
                    .font(.system(size: 12))
            }
            .padding(12)
        } detail: {
            VStack(spacing: 16) {
                Text(selectedSection?.rawValue ?? ProbeSection.home.rawValue)
                    .font(.system(size: 24))

                Text(selectedSection?.description ?? ProbeSection.home.description)

                Divider()

                Text("Selector de archivos común")
                    .font(.system(size: 18))

                Button("Seleccionar archivo…") {
                    Task {
                        selectedFile = await chooseFile()
                    }
                }

                if let selectedFile {
                    Text(selectedFile.lastPathComponent)
                } else {
                    Text("Ningún archivo seleccionado")
                }

                Divider()

                Text("Tema")
                    .font(.system(size: 18))

                HStack {
                    Button("Sistema") {
                        theme = .system
                    }
                    Button("Claro") {
                        theme = .light
                    }
                    Button("Oscuro") {
                        theme = .dark
                    }
                }

                Text("Tema activo: \(theme.rawValue)")

                Divider()

                Text("Drag & drop")
                    .font(.system(size: 18))
                Text("Pendiente de API común: la revisión actual de SwiftCrossUI no expone todavía un modificador multiplataforma equivalente.")
            }
            .padding(20)
        }
        .preferredColorScheme(theme.colorScheme)
    }
}

@main
struct SynToolsSwiftProbeApp: App {
    var body: some Scene {
        WindowGroup("SynTools · Prueba Swift") {
            ProbeRootView()
        }
        .defaultSize(width: 900, height: 600)
    }
}
