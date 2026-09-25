using System.Diagnostics;
using SynTools.Probe.Core;
using Xunit;

namespace SynTools.Probe.Tests;

public sealed class ProbeTests
{
    [Fact] public void CoordinatorBlocksSecondAndResistsDoubleClose()
    {
        var coordinator = new OperationCoordinator();
        using var operation = coordinator.Begin("pesada");
        Assert.Throws<InvalidOperationException>(() => coordinator.Begin("segunda"));
        operation.Report(.4); Assert.True(operation.Succeed()); Assert.False(operation.Succeed());
        Assert.Equal(OperationState.Succeeded, coordinator.Snapshot.State);
        using var next = coordinator.Begin("siguiente", true); next.Cancel(); Assert.True(next.AcknowledgeCancellation());
    }

    [Fact] public async Task DatabaseMigratesPersistsAndSanitizesHistory()
    {
        using var temp = new TemporaryDirectory(); var path = Path.Combine(temp.Path, "datos.sqlite");
        var database = new ProbeDatabase(path); await database.InitializeAsync(); await database.SetAsync("theme", "dark");
        await Task.WhenAll(Enumerable.Range(0, 8).Select(i => database.SetAsync($"key{i}", i.ToString())));
        var reopened = new ProbeDatabase(path); await reopened.InitializeAsync(); Assert.Equal("dark", await reopened.GetAsync("theme"));
        await reopened.AddHistoryAsync("archivo", Path.Combine("privado", "secreto", "vídeo.mp4"));
        var bytes = await File.ReadAllBytesAsync(path); Assert.DoesNotContain("privado", System.Text.Encoding.UTF8.GetString(bytes));
    }

    [Fact] public async Task FilesHandleUnicodeConflictModificationAndMissingInput()
    {
        using var temp = new TemporaryDirectory(); var input = Path.Combine(temp.Path, "niño_日本語_🚀_" + new string('a', 120) + ".txt"); await File.WriteAllTextAsync(input, "uno");
        var fingerprint = await SafeFiles.FingerprintAsync(input); await File.AppendAllTextAsync(input, "dos");
        Assert.Throws<IOException>(() => SafeFiles.Revalidate(input, fingerprint));
        var existing = Path.Combine(temp.Path, "salida.txt"); await File.WriteAllTextAsync(existing, "original"); Assert.EndsWith("salida (2).txt", SafeFiles.ResolveConflict(existing));
        var owned = Path.Combine(temp.Path, "temporal-propio.tmp"); var published = Path.Combine(temp.Path, "publicado.txt"); await File.WriteAllTextAsync(owned, "resultado"); SafeFiles.Publish(owned, published); Assert.False(File.Exists(owned)); Assert.Equal("resultado", await File.ReadAllTextAsync(published));
        var second = Path.Combine(temp.Path, "segundo.tmp"); await File.WriteAllTextAsync(second, "no sobrescribir"); Assert.Throws<IOException>(() => SafeFiles.Publish(second, published)); Assert.Equal("resultado", await File.ReadAllTextAsync(published));
        File.Delete(input); await Assert.ThrowsAsync<FileNotFoundException>(() => SafeFiles.FingerprintAsync(input));
    }

    [Fact] public async Task ProcessRunnerCapturesStreamsTimeoutAndKillsRecordedDescendants()
    {
        using var temp = new TemporaryDirectory(); var pidFile = Path.Combine(temp.Path, "pids.txt");
        var script = Shared("process_tree_harness.py"); var runner = new ProcessRunner();
        var python = OperatingSystem.IsWindows() ? "python" : "python3";
        var task = runner.RunAsync(new(python, [script, "--pid-file", pidFile, "--seconds", "60"], Timeout: TimeSpan.FromMilliseconds(900)));
        await Assert.ThrowsAsync<ProcessTreeCancelledException>(() => task);
        await Task.Delay(300);
        var pids = File.ReadAllLines(pidFile).Select(line => int.Parse(line.Split(':')[1])).Distinct().ToArray();
        Assert.True(pids.Length >= 2);
        foreach (var pid in pids) Assert.False(IsAlive(pid), $"El PID {pid} sigue vivo");
    }

    [Fact] public async Task ProcessRunnerKeepsArgumentsSeparateAndCapturesOutput()
    {
        var python = OperatingSystem.IsWindows() ? "python" : "python3";
        var result = await new ProcessRunner().RunAsync(new(python, ["-c", "import sys;print(sys.argv[1]);print('err', file=sys.stderr)", "dato con espacios;$(no-shell)"]));
        Assert.Contains("dato con espacios;$(no-shell)", result.StandardOutput); Assert.Contains("err", result.StandardError); Assert.Equal(0, result.ExitCode);
    }

    [Fact] public async Task FfmpegDiagnosesAndDecodesSyntheticFixture()
    {
        var fixture = Fixture("audio_48k_stereo.wav");
        var runner = new ProcessRunner();
        var probe = new FfmpegProbe(runner);
        var diagnosis = await probe.DiagnoseAsync("ffmpeg", fixture);
        Assert.Equal(0, diagnosis.ExitCode);
        Assert.Contains("Audio:", diagnosis.StandardError);
        var decoded = await probe.DecodePcmAsync("ffmpeg", fixture);
        Assert.Equal(8L * 48_000 * 2 * 2, decoded.Bytes);
        await Assert.ThrowsAsync<ProcessTreeCancelledException>(() => runner.RunAsync(new("ffmpeg",
            ["-hide_banner", "-loglevel", "error", "-re", "-stream_loop", "-1", "-i", fixture, "-f", "null", "-"],
            Timeout: TimeSpan.FromMilliseconds(700))));
    }

    private static bool IsAlive(int pid) { try { using var process = Process.GetProcessById(pid); return !process.HasExited; } catch (ArgumentException) { return false; } }
    private static string Shared(string name) => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../Compartido/Scripts", name));
    private static string Fixture(string name) => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../Compartido/Fixtures/Generados", name));
}

internal sealed class TemporaryDirectory : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "syntools-probe-" + Guid.NewGuid().ToString("N"));
    public TemporaryDirectory() => Directory.CreateDirectory(Path);
    public void Dispose() { if (Directory.Exists(Path)) Directory.Delete(Path, true); }
}
