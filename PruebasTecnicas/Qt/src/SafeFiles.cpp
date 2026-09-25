#include "SafeFiles.hpp"
#include <QCryptographicHash>
#include <QDir>
#include <QFile>
#include <QFileInfo>

FileFingerprint SafeFiles::fingerprint(const QString &path) {
    QFileInfo before(path); if (!before.exists()) throw std::runtime_error("La entrada ha desaparecido");
    QFile file(path); if (!file.open(QIODevice::ReadOnly)) throw std::runtime_error("No se pudo abrir la entrada");
    QCryptographicHash hash(QCryptographicHash::Sha256); if (!hash.addData(&file)) throw std::runtime_error("No se pudo leer la entrada");
    QFileInfo after(path); if (!after.exists() || before.size() != after.size() || before.lastModified() != after.lastModified()) throw std::runtime_error("La entrada cambió durante la lectura");
    return {after.size(), after.lastModified().toUTC(), hash.result()};
}
bool SafeFiles::revalidate(const QString &path, const FileFingerprint &expected) { QFileInfo current(path); return current.exists() && current.size() == expected.size && current.lastModified().toUTC() == expected.modifiedUtc; }
QString SafeFiles::resolveConflict(const QString &path) { if (!QFileInfo::exists(path)) return path; QFileInfo info(path); for (int i=2;i<10000;i++){ const auto candidate=info.dir().filePath(info.completeBaseName()+QString(" (%1)").arg(i)+(info.suffix().isEmpty()?QString{}:"."+info.suffix())); if(!QFileInfo::exists(candidate)) return candidate; } throw std::runtime_error("Conflicto sin salida"); }
bool SafeFiles::publish(const QString &ownedTemporary, const QString &destination) { return QFileInfo::exists(ownedTemporary) && !QFileInfo::exists(destination) && QFile::rename(ownedTemporary, destination); }
