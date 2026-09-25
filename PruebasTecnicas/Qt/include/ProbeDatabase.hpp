#pragma once
#include <QString>

class ProbeDatabase {
public:
    explicit ProbeDatabase(QString path);
    bool initialize();
    bool set(const QString &key, const QString &value);
    QString get(const QString &key);
    bool addHistory(const QString &kind, const QString &path);
private:
    QString path_;
    QString connectionName() const;
};
