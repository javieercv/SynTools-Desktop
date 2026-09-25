#include "MediaController.hpp"
#include "OperationCoordinator.hpp"
#include "ProbeDatabase.hpp"
#include "ProcessRunner.hpp"
#include "SafeFiles.hpp"
#include <QFile>
#include <QProcess>
#include <QTemporaryDir>
#include <QtTest>
#include <future>

class ProbeTests : public QObject {
    Q_OBJECT
private slots:
    void coordinatorBlocksSecondAndResistsDoubleClose() {
        OperationCoordinator c; QVERIFY(c.begin("pesada")); QVERIFY(!c.begin("segunda")); QVERIFY(c.report(.4)); QVERIFY(c.finish(OperationCoordinator::State::Succeeded)); QVERIFY(!c.finish(OperationCoordinator::State::Succeeded)); QVERIFY(c.begin("siguiente", true)); QVERIFY(c.requestCancellation()); QVERIFY(c.finish(OperationCoordinator::State::Cancelled));
    }
    void sqliteMigratesPersistsAndSanitizes() {
        QTemporaryDir dir; const auto path=dir.filePath("datos.sqlite"); ProbeDatabase db(path); QVERIFY(db.initialize()); QVERIFY(db.set("theme","dark")); QCOMPARE(db.get("theme"),QString("dark"));
        std::vector<std::future<bool>> writes; for(int i=0;i<8;i++) writes.push_back(std::async(std::launch::async,[path,i]{ ProbeDatabase concurrent(path); return concurrent.set("key"+QString::number(i),QString::number(i)); })); for(auto &write:writes) QVERIFY(write.get());
        ProbeDatabase reopened(path); QCOMPARE(reopened.get("theme"),QString("dark")); QVERIFY(reopened.addHistory("archivo", "/privado/secreto/vídeo.mp4")); QFile raw(path); QVERIFY(raw.open(QIODevice::ReadOnly)); QVERIFY(!raw.readAll().contains("privado"));
    }
    void filesHandleUnicodeConflictModificationAndMissing() {
        QTemporaryDir dir; const auto path=dir.filePath(QString::fromUtf8("niño_日本語_🚀_")+QString(120,'a')+".txt"); QFile f(path); QVERIFY(f.open(QIODevice::WriteOnly)); f.write("uno"); f.close(); auto fp=SafeFiles::fingerprint(path); QVERIFY(f.open(QIODevice::Append)); f.write("dos"); f.close(); QVERIFY(!SafeFiles::revalidate(path,fp)); const auto output=dir.filePath("salida.txt"); QFile original(output); QVERIFY(original.open(QIODevice::WriteOnly)); original.write("original"); original.close(); QVERIFY(SafeFiles::resolveConflict(output).endsWith("salida (2).txt")); const auto owned=dir.filePath("temporal-propio.tmp"); QFile temporary(owned); QVERIFY(temporary.open(QIODevice::WriteOnly)); temporary.write("resultado"); temporary.close(); const auto published=dir.filePath("publicado.txt"); QVERIFY(SafeFiles::publish(owned,published)); QVERIFY(!QFileInfo::exists(owned)); const auto second=dir.filePath("segundo.tmp"); QFile other(second); QVERIFY(other.open(QIODevice::WriteOnly)); other.write("no sobrescribir"); other.close(); QVERIFY(!SafeFiles::publish(second,published)); QVERIFY(QFile::remove(path)); QVERIFY_THROWS_EXCEPTION(std::runtime_error, SafeFiles::fingerprint(path));
    }
    void processUsesSeparateArgumentsAndCapturesStreams() {
        ProcessRunner runner;
#ifdef Q_OS_WIN
        const QString python="python";
#else
        const QString python="python3";
#endif
        auto r=runner.run(python, {"-c","import sys;print(sys.argv[1]);print('err',file=sys.stderr)","dato con espacios;$(no-shell)"},5000); QCOMPARE(r.exitCode,0); QVERIFY(r.standardOutput.contains("$(no-shell)")); QVERIFY(r.standardError.contains("err"));
    }
    void processTimeoutKillsParentAndDescendant() {
        QTemporaryDir dir; const auto pids=dir.filePath("pids.txt"); const auto script=QDir(QCoreApplication::applicationDirPath()).absoluteFilePath("../../Compartido/Scripts/process_tree_harness.py"); ProcessRunner runner;
#ifdef Q_OS_WIN
        const QString python="python";
#else
        const QString python="python3";
#endif
        auto r=runner.run(python,{script,"--pid-file",pids,"--seconds","60"},900); QVERIFY(r.cancelled); QTest::qWait(300); QFile file(pids); QVERIFY(file.open(QIODevice::ReadOnly)); const auto lines=file.readAll().split('\n'); int count=0; for(const auto &line:lines){ if(line.isEmpty())continue; count++; const auto pid=line.split(':').last(); QProcess check;
#ifdef Q_OS_WIN
            check.start("tasklist",{"/FI","PID eq "+QString::fromUtf8(pid),"/NH"});
#else
            check.start("ps",{"-p",QString::fromUtf8(pid),"-o","pid="});
#endif
            check.waitForFinished(); const auto output=check.readAllStandardOutput().trimmed();
#ifdef Q_OS_WIN
            QVERIFY2(!output.contains(pid),"Quedó un proceso descendiente vivo");
#else
            QVERIFY2(output.isEmpty(),"Quedó un proceso descendiente vivo");
#endif
        } QVERIFY(count>=2);
    }
    void ffmpegDiagnosesSyntheticFixtureAndReportsProgress() {
        const auto fixture=QDir(QCoreApplication::applicationDirPath()).absoluteFilePath("../../Compartido/Fixtures/Generados/audio_48k_stereo.wav");
        ProcessRunner runner;
        auto r=runner.run("ffmpeg", {"-hide_banner","-loglevel","error","-progress","pipe:1","-nostats","-i",fixture,"-f","null","-"},10000);
        QCOMPARE(r.exitCode,0);
        QVERIFY(r.standardOutput.contains("progress=end"));
        QVERIFY(!r.cancelled);
        auto cancelled=runner.run("ffmpeg", {"-hide_banner","-loglevel","error","-re","-stream_loop","-1","-i",fixture,"-f","null","-"},700);
        QVERIFY(cancelled.cancelled);
    }
    void videoDecodesControlledFramesAndSeeks() {
        const auto fixture=QDir(QCoreApplication::applicationDirPath()).absoluteFilePath("../../Compartido/Fixtures/Generados/video_1080p.mp4");
        MediaController media; media.setSource(fixture); QVERIFY(media.frameData().startsWith("data:image/png;base64,")); media.seek(3.0); QCOMPARE(media.position(),3.0); QVERIFY(media.frameData().size()>1000);
    }
};

QTEST_MAIN(ProbeTests)
#include "ProbeTests.moc"
