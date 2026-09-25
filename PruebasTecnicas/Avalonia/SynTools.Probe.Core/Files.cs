using System.Security.Cryptography;

namespace SynTools.Probe.Core;

public sealed record FileFingerprint(long Length, DateTime LastWriteUtc, string Sha256);

public static class SafeFiles
{
    public static async Task<FileFingerprint> FingerprintAsync(string path, CancellationToken cancellationToken = default)
    {
        var before = new FileInfo(path);
        if (!before.Exists) throw new FileNotFoundException("La entrada ha desaparecido.", path);
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        var after = new FileInfo(path);
        if (!after.Exists || before.Length != after.Length || before.LastWriteTimeUtc != after.LastWriteTimeUtc)
            throw new IOException("La entrada cambió durante la lectura.");
        return new(after.Length, after.LastWriteTimeUtc, Convert.ToHexString(hash));
    }

    public static void Revalidate(string path, FileFingerprint expected)
    {
        var current = new FileInfo(path);
        if (!current.Exists) throw new FileNotFoundException("La entrada ha desaparecido.", path);
        if (current.Length != expected.Length || current.LastWriteTimeUtc != expected.LastWriteUtc)
            throw new IOException("La entrada fue modificada.");
    }

    public static string ResolveConflict(string requested)
    {
        if (!File.Exists(requested) && !Directory.Exists(requested)) return requested;
        var directory = Path.GetDirectoryName(requested) ?? ".";
        var stem = Path.GetFileNameWithoutExtension(requested);
        var extension = Path.GetExtension(requested);
        for (var index = 2; index < 10_000; index++)
        {
            var candidate = Path.Combine(directory, $"{stem} ({index}){extension}");
            if (!File.Exists(candidate) && !Directory.Exists(candidate)) return candidate;
        }
        throw new IOException("No se pudo resolver el conflicto de nombre.");
    }

    public static void Publish(string ownedTemporaryFile, string destination)
    {
        if (!File.Exists(ownedTemporaryFile)) throw new FileNotFoundException("Falta el temporal propio.", ownedTemporaryFile);
        if (File.Exists(destination) || Directory.Exists(destination)) throw new IOException("El destino ya existe.");
        File.Move(ownedTemporaryFile, destination);
    }
}
