namespace Syndication.Tests;

using System.IO;
using System.Reflection;
using System.Text;

static class Samples
{
    private static readonly Assembly assembly;

    static Samples()
    {
        assembly = Assembly.GetExecutingAssembly();
    }

    public static ReadOnlySpan<byte> GetBuffer(string filePath)
    {
        using var stream = GetStream(filePath);
        return ReadAllBytes(stream);
    }

    public static string GetContent(string filePath)
    {
        using var stream = GetStream(filePath);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static Stream GetStream(string filePath)
    {
        return assembly.GetManifestResourceStream($"Syndication.Tests.{filePath.Replace('/', '.')}");
    }

    public static IEnumerable<(string Name, string Content)> EnumerateContents(string folder)
    {
        foreach (var (name, stream) in EnumerateStreams(folder))
        {
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            yield return (name, streamReader.ReadToEnd());
        }
    }

    public static IEnumerable<(string Name, Stream Stream)> EnumerateStreams(string folder)
    {
        folder = string.Concat(".", folder, ".");
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.Contains(folder))
            {
                yield return (resourceName, assembly.GetManifestResourceStream(resourceName));
            }
        }
    }

    private static ReadOnlySpan<byte> ReadAllBytes(Stream stream)
    {
        if (stream.GetType() == typeof(MemoryStream) && ((MemoryStream)stream).TryGetBuffer(out ArraySegment<byte> memoryStreamBuffer))
        {
            int position = (int)stream.Position;
            // Simulate that we read the stream to its end.
            stream.Seek(0, SeekOrigin.End);
            return memoryStreamBuffer.AsSpan(position);
        }

        long length = stream.Length - stream.Position;

        if (length == 0)
        {
            return [];
        }

        if (((ulong)length) > (ulong)Array.MaxLength)
        {
            throw new BadImageFormatException();
        }

        byte[] bytes = GC.AllocateUninitializedArray<byte>((int)length);

        // Copy the stream to the byte array
        stream.ReadExactly(bytes);

        return bytes;
    }
}
