#include "OperationCoordinator.hpp"
#include <QMutexLocker>
#include <algorithm>

bool OperationCoordinator::begin(const QString &name, bool indeterminate) {
    QMutexLocker lock(&mutex_); if (reserved_) return false; reserved_ = true;
    snapshot_ = {State::Running, name, indeterminate ? std::nullopt : std::optional<double>(0), {}}; return true;
}
bool OperationCoordinator::report(double progress) {
    QMutexLocker lock(&mutex_); if (!reserved_ || snapshot_.state != State::Running) return false;
    snapshot_.progress = std::clamp(progress, 0.0, 1.0); return true;
}
bool OperationCoordinator::requestCancellation() {
    QMutexLocker lock(&mutex_); if (!reserved_ || snapshot_.state != State::Running) return false;
    snapshot_.state = State::Cancelling; return true;
}
bool OperationCoordinator::finish(State terminal, const QString &error) {
    QMutexLocker lock(&mutex_); if (!reserved_) return false;
    if (terminal != State::Succeeded && terminal != State::Failed && terminal != State::Cancelled) return false;
    snapshot_.state = terminal; snapshot_.error = error; if (terminal == State::Succeeded) snapshot_.progress = 1; reserved_ = false; return true;
}
OperationCoordinator::Snapshot OperationCoordinator::snapshot() const { QMutexLocker lock(&mutex_); return snapshot_; }
