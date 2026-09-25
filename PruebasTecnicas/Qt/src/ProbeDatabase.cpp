#include "ProbeDatabase.hpp"
#include <QFileInfo>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QThread>
#include <QUuid>

ProbeDatabase::ProbeDatabase(QString path) : path_(std::move(path)) {}
QString ProbeDatabase::connectionName() const { return QStringLiteral("syntools-") + QString::number(qHash(path_)) + "-" + QString::number(reinterpret_cast<quintptr>(QThread::currentThreadId())); }
static QSqlDatabase openDatabase(const QString &path, const QString &name) { auto db = QSqlDatabase::contains(name) ? QSqlDatabase::database(name) : QSqlDatabase::addDatabase("QSQLITE", name); db.setDatabaseName(path); db.open(); QSqlQuery pragma(db); pragma.exec("PRAGMA busy_timeout=5000"); return db; }
bool ProbeDatabase::initialize() {
    auto db = openDatabase(path_, connectionName()); QSqlQuery q(db); q.exec("PRAGMA journal_mode=WAL"); q.exec("PRAGMA busy_timeout=5000");
    return q.exec("CREATE TABLE IF NOT EXISTS schema_version(version INTEGER NOT NULL)") &&
           q.exec("INSERT INTO schema_version(version) SELECT 0 WHERE NOT EXISTS(SELECT 1 FROM schema_version)") &&
           q.exec("CREATE TABLE IF NOT EXISTS settings(key TEXT PRIMARY KEY,value TEXT NOT NULL)") &&
           q.exec("CREATE TABLE IF NOT EXISTS history(id INTEGER PRIMARY KEY,kind TEXT NOT NULL,display_name TEXT NOT NULL,created_utc TEXT NOT NULL)") &&
           q.exec("UPDATE schema_version SET version=1 WHERE version=0");
}
bool ProbeDatabase::set(const QString &key, const QString &value) { auto db = openDatabase(path_, connectionName()); QSqlQuery q(db); q.prepare("INSERT INTO settings VALUES(?,?) ON CONFLICT(key) DO UPDATE SET value=excluded.value"); q.addBindValue(key); q.addBindValue(value); return q.exec(); }
QString ProbeDatabase::get(const QString &key) { auto db = openDatabase(path_, connectionName()); QSqlQuery q(db); q.prepare("SELECT value FROM settings WHERE key=?"); q.addBindValue(key); return q.exec() && q.next() ? q.value(0).toString() : QString{}; }
bool ProbeDatabase::addHistory(const QString &kind, const QString &path) { auto db = openDatabase(path_, connectionName()); QSqlQuery q(db); q.prepare("INSERT INTO history(kind,display_name,created_utc) VALUES(?,?,datetime('now'))"); q.addBindValue(kind); q.addBindValue(QFileInfo(path).fileName()); return q.exec(); }
