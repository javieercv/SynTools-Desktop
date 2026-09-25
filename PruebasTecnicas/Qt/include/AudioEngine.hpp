#pragma once
#include <QAudioSink>
#include <QBuffer>
#include <QObject>

class AudioEngine : public QObject {
    Q_OBJECT
    Q_PROPERTY(qint64 position READ position NOTIFY positionChanged)
public:
    explicit AudioEngine(QObject *parent = nullptr);
    Q_INVOKABLE bool loadWav(const QString &path);
    Q_INVOKABLE void play(); Q_INVOKABLE void pause(); Q_INVOKABLE void resume(); Q_INVOKABLE void stop();
    Q_INVOKABLE void seek(qint64 milliseconds); Q_INVOKABLE void setVolume(double volume);
    qint64 position() const;
signals: void positionChanged(); void errorOccurred(const QString &message);
private:
    QByteArray pcm_; QBuffer buffer_{&pcm_}; std::unique_ptr<QAudioSink> sink_; QAudioFormat format_; qint64 dataOffset_{44};
};
