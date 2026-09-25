namespace SynTools.Probe.Core;

public sealed record DecodeMetrics(long Bytes, TimeSpan Duration);

public sealed class FfmpegProbe(ProcessRunner runner)
{
    public async Task<DecodeMetrics> DecodePcmAsync(string ffmpeg, string input, CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(Path.GetTempPath(), "syntools-ffmpeg-probe-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var output = Path.Combine(directory, "audio.pcm");
        try
        {
            var result = await runner.RunAsync(new(ffmpeg,
                ["-hide_banner", "-loglevel", "error", "-progress", "pipe:1", "-nostats", "-i", input,
                 "-f", "s16le", "-ac", "2", "-ar", "48000", "-y", output]), cancellationToken);
            return new(new FileInfo(output).Length, result.Duration);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    public Task<ProcessResult> DiagnoseAsync(string ffmpeg, string input, CancellationToken cancellationToken = default) =>
        runner.RunAsync(new(ffmpeg, ["-hide_banner", "-i", input, "-f", "null", "-"]), cancellationToken);
}
