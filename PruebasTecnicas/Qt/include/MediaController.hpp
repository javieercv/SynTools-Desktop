#pragma once
#include <QObject>
#include <QTimer>

class MediaController : public QObject {
    Q_OBJECT
    Q_PROPERTY(QString source READ source WRITE setSource NOTIFY sourceChanged)
    Q_PROPERTY(double position READ position WRITE seek NOTIFY positionChanged)
    Q_PROPERTY(QString frameData READ frameData NOTIFY frameChanged)
    Q_PROPERTY(bool playing READ playing NOTIFY playingChanged)
public:
    explicit MediaController(QObject *parent = nullptr);
    QString source() const { return source_; } void setSource(const QString &source);
    double position() const { return position_; } QString frameData() const { return frameData_; } bool playing() const { return timer_.isActive(); }
    Q_INVOKABLE void play(); Q_INVOKABLE void pause(); Q_INVOKABLE void stop(); Q_INVOKABLE void seek(double seconds);
signals: void sourceChanged(); void positionChanged(); void frameChanged(); void playingChanged(); void errorOccurred(const QString &message);
private:
    void decodeFrame(); QString source_; QString frameData_; double position_{0}; QTimer timer_;
};
