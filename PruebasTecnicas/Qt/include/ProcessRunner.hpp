#pragma once
#include <QByteArray>
#include <QProcess>
#include <QStringList>

struct ProcessResult { int exitCode{-1}; QByteArray standardOutput; QByteArray standardError; bool cancelled{false}; qint64 elapsedMs{0}; };

class ProcessRunner {
public:
    ProcessResult run(const QString &executable, const QStringList &arguments, int timeoutMs, const QString &workingDirectory = {});
private:
    static void configureTree(QProcess &process);
    static void killTree(QProcess &process);
};
