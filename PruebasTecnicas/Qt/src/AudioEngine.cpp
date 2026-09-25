#include "AudioEngine.hpp"
#include <QAudioDevice>
#include <QFile>
#include <QMediaDevices>

AudioEngine::AudioEngine(QObject *parent) : QObject(parent) {
    format_.setSampleRate(48000); format_.setChannelCount(2); format_.setSampleFormat(QAudioFormat::Int16);
}
bool AudioEngine::loadWav(const QString &path) {
    stop(); QFile file(path); if (!file.open(QIODevice::ReadOnly)) { emit errorOccurred(file.errorString()); return false; }
    const auto wav = file.readAll(); if (wav.size() <= dataOffset_) { emit errorOccurred("Fixture WAV inválido"); return false; }
    pcm_ = wav.mid(dataOffset_); buffer_.open(QIODevice::ReadOnly); sink_ = std::make_unique<QAudioSink>(QMediaDevices::defaultAudioOutput(), format_); sink_->setBufferSize(48'000); return true;
}
void AudioEngine::play() { if (sink_) { buffer_.seek(0); sink_->start(&buffer_); emit positionChanged(); } }
void AudioEngine::pause() { if (sink_) sink_->suspend(); }
void AudioEngine::resume() { if (sink_) sink_->resume(); }
void AudioEngine::stop() { if (sink_) sink_->stop(); buffer_.close(); }
void AudioEngine::seek(qint64 ms) { if (!sink_) return; const qint64 byte = std::clamp<qint64>(ms * 48000 * 2 * 2 / 1000, 0, pcm_.size()); buffer_.seek(byte); emit positionChanged(); }
void AudioEngine::setVolume(double volume) { if (sink_) sink_->setVolume(std::clamp(volume, 0.0, 1.0)); }
qint64 AudioEngine::position() const { return buffer_.pos() * 1000 / (48000 * 2 * 2); }
