#include "MediaController.hpp"
#include <QProcess>

MediaController::MediaController(QObject *parent) : QObject(parent) {
    timer_.setInterval(100); connect(&timer_, &QTimer::timeout, this, [this] { position_ += .1; if (position_ > 5.9) position_ = 0; decodeFrame(); emit positionChanged(); });
}
void MediaController::setSource(const QString &source) { if (source_ == source) return; source_ = source; position_ = 0; emit sourceChanged(); decodeFrame(); }
void MediaController::play() { if (!source_.isEmpty()) { timer_.start(); emit playingChanged(); } }
void MediaController::pause() { timer_.stop(); emit playingChanged(); }
void MediaController::stop() { timer_.stop(); position_ = 0; decodeFrame(); emit playingChanged(); emit positionChanged(); }
void MediaController::seek(double seconds) { position_ = std::clamp(seconds, 0.0, 6.0); decodeFrame(); emit positionChanged(); }
void MediaController::decodeFrame() {
    if (source_.isEmpty()) return; QProcess ffmpeg;
    ffmpeg.start("ffmpeg", {"-hide_banner", "-loglevel", "error", "-ss", QString::number(position_, 'f', 3), "-i", source_, "-frames:v", "1", "-vf", "scale=960:-2", "-f", "image2pipe", "-vcodec", "png", "-"});
    if (!ffmpeg.waitForFinished(5000)) { ffmpeg.kill(); emit errorOccurred("Timeout decodificando frame"); return; }
    const auto png = ffmpeg.readAllStandardOutput(); if (png.isEmpty()) { emit errorOccurred(QString::fromUtf8(ffmpeg.readAllStandardError())); return; }
    frameData_ = "data:image/png;base64," + QString::fromLatin1(png.toBase64()); emit frameChanged();
}
