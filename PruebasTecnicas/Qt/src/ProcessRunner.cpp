#include "ProcessRunner.hpp"
#include <QElapsedTimer>
#ifdef Q_OS_UNIX
#include <csignal>
#include <unistd.h>
#endif
#ifdef Q_OS_WIN
#include <windows.h>
#endif

void ProcessRunner::configureTree(QProcess &process) {
#ifdef Q_OS_UNIX
    process.setChildProcessModifier([] { if (::setpgid(0, 0) != 0) _exit(127); });
#elif defined(Q_OS_WIN)
    process.setCreateProcessArgumentsModifier([](QProcess::CreateProcessArguments *args) { args->flags |= CREATE_NEW_PROCESS_GROUP; });
#else
    Q_UNUSED(process);
#endif
}

void ProcessRunner::killTree(QProcess &process) {
    if (process.state() == QProcess::NotRunning) return;
#ifdef Q_OS_UNIX
    ::kill(-static_cast<pid_t>(process.processId()), SIGKILL);
#elif defined(Q_OS_WIN)
    QProcess::execute("taskkill", {"/PID", QString::number(process.processId()), "/T", "/F"});
#else
    process.kill();
#endif
    process.waitForFinished(5000);
}

ProcessResult ProcessRunner::run(const QString &executable, const QStringList &arguments, int timeoutMs, const QString &workingDirectory) {
    QProcess process; configureTree(process); process.setProgram(executable); process.setArguments(arguments);
    if (!workingDirectory.isEmpty()) process.setWorkingDirectory(workingDirectory);
    QElapsedTimer timer; timer.start(); process.start();
    if (!process.waitForStarted(5000)) return {-1, {}, process.errorString().toUtf8(), false, timer.elapsed()};
    const bool finished = process.waitForFinished(timeoutMs);
    if (!finished) killTree(process);
    return {process.exitCode(), process.readAllStandardOutput(), process.readAllStandardError(), !finished, timer.elapsed()};
}
