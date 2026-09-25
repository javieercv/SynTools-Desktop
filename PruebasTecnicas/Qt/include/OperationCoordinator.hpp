#pragma once
#include <QMutex>
#include <QString>
#include <optional>

class OperationCoordinator {
public:
    enum class State { Idle, Running, Cancelling, Succeeded, Failed, Cancelled };
    struct Snapshot { State state{State::Idle}; QString name; std::optional<double> progress; QString error; };
    bool begin(const QString &name, bool indeterminate = false);
    bool report(double progress);
    bool requestCancellation();
    bool finish(State terminal, const QString &error = {});
    Snapshot snapshot() const;
private:
    mutable QMutex mutex_;
    Snapshot snapshot_;
    bool reserved_{false};
};
