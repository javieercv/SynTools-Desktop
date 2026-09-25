#include "AudioEngine.hpp"
#include "MediaController.hpp"
#include <QCommandLineOption>
#include <QCommandLineParser>
#include <QDir>
#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <QQmlContext>

int main(int argc, char *argv[]) {
    QGuiApplication app(argc, argv); app.setApplicationName("SynTools · Prueba Qt");
    QCommandLineParser parser;
    parser.addHelpOption();
    parser.addOption({"fixtures-path", "Ruta al directorio de fixtures compartidos.", "directorio"});
    parser.process(app);
    QString fixtureRoot = parser.value("fixtures-path");
    if (fixtureRoot.isEmpty()) fixtureRoot = qEnvironmentVariable("SYNTOOLS_FIXTURE_DIR");
    fixtureRoot = QDir::cleanPath(fixtureRoot);
    AudioEngine audio; MediaController media; QQmlApplicationEngine engine;
    engine.rootContext()->setContextProperty("audioEngine", &audio); engine.rootContext()->setContextProperty("mediaController", &media);
    engine.rootContext()->setContextProperty("fixtureRoot", fixtureRoot);
    engine.loadFromModule("SynToolsProbe", "Main"); if (engine.rootObjects().isEmpty()) return -1; return app.exec();
}
