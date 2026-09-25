#pragma once
#include <QDateTime>
#include <QString>

struct FileFingerprint { qint64 size; QDateTime modifiedUtc; QByteArray sha256; };
class SafeFiles {
public:
    static FileFingerprint fingerprint(const QString &path);
    static bool revalidate(const QString &path, const FileFingerprint &expected);
    static QString resolveConflict(const QString &path);
    static bool publish(const QString &ownedTemporary, const QString &destination);
};
