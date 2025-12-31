using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace olieblind.lib.Services.Speech;

[ExcludeFromCodeCoverage]
public static class WavReader
{
    // http://soundfile.sapp.org/doc/WaveFormat/
    public class Riff
    {
        public static Riff Read(BinaryReader br)
        {
            var result = new Riff
            {
                ChunkId = ReadString(br, 4),
                ChunkSize = br.ReadUInt32(),
                Format = ReadString(br, 4)
            };

            if (result.ChunkId != "RIFF") throw new ApplicationException("Missing RIFF header");
            if (result.Format != "WAVE") throw new ApplicationException("Not a WAVE file");

            return result;
        }

        public void Write(BinaryWriter bw)
        {
            WriteString(bw, ChunkId);
            bw.Write((uint)ChunkSize);
            WriteString(bw, Format);
        }

        public void Append(Riff rhs)
        {
            if ((ChunkId != rhs.ChunkId) ||
                (Format != rhs.Format))
                throw new ApplicationException("Both files need to be WAV format");

            const int riffFormat = 4;
            const int fmtHeader = 8;
            const int fmtBody = 16;
            const int fmtDataHeader = 8;

            ChunkSize += rhs.ChunkSize - (riffFormat + fmtHeader + fmtBody + fmtDataHeader);
        }

        private string ChunkId { get; init; } = "RIFF";
        public long ChunkSize { get; set; }
        private string Format { get; init; } = "WAVE";
    }

    public class Subchunk
    {
        public static Subchunk Read(BinaryReader br)
        {
            var result = new Subchunk
            {
                SubchunkId = ReadString(br, 4),
                SubchunkSize = br.ReadUInt32()
            };

            return result;
        }

        public string SubchunkId { get; private init; } = string.Empty;
        public long SubchunkSize { get; private init; }
    }

    public class Fmt
    {
        public static Fmt Read(Subchunk subchunk, BinaryReader br)
        {
            var result = new Fmt
            {
                SubchunkId = subchunk.SubchunkId,
                SubchunkSize = subchunk.SubchunkSize,
                AudioFormat = br.ReadInt16(),
                NumChannels = br.ReadInt16(),
                SampleRate = br.ReadInt32(),
                ByteRate = br.ReadInt32(),
                BlockAlign = br.ReadInt16(),
                BitsPerSample = br.ReadInt16()
            };

            if (result.SubchunkId != "fmt ") throw new ApplicationException("Missing fmt header");
            if (result.AudioFormat != 1) throw new ApplicationException("Expected PCM format");

            return result;
        }

        public void Write(BinaryWriter bw)
        {
            WriteString(bw, SubchunkId);
            bw.Write((uint)SubchunkSize);
            bw.Write((short)AudioFormat);
            bw.Write((short)NumChannels);
            bw.Write(SampleRate);
            bw.Write(ByteRate);
            bw.Write((short)BlockAlign);
            bw.Write((short)BitsPerSample);
        }

        public void Append(Fmt rhs)
        {
            if ((SubchunkId != rhs.SubchunkId) ||
                (SubchunkSize != rhs.SubchunkSize) ||
                (AudioFormat != rhs.AudioFormat) ||
                (NumChannels != rhs.NumChannels) ||
                (SampleRate != rhs.SampleRate) ||
                (ByteRate != rhs.ByteRate) ||
                (BlockAlign != rhs.BlockAlign) ||
                (BitsPerSample != rhs.BitsPerSample))
                throw new ApplicationException("WAV formats need to match");
        }

        private string SubchunkId { get; init; } = string.Empty;
        private long SubchunkSize { get; init; }
        private int AudioFormat { get; init; }
        private int NumChannels { get; init; }
        private int SampleRate { get; init; }
        public int ByteRate { get; private init; }
        private int BlockAlign { get; init; }
        private int BitsPerSample { get; init; }
    }

    public class Data
    {
        public static Data Read(Subchunk subchunk, BinaryReader br)
        {
            var result = new Data
            {
                SubchunkId = subchunk.SubchunkId,
                SubchunkSize = subchunk.SubchunkSize,
            };

            if (result.SubchunkId != "data") throw new ApplicationException("Missing data header");

            var bytesLeft = result.SubchunkSize;

            while (bytesLeft > 0)
            {
                var readLength = bytesLeft > 5242880 ? 5242880 : (int)bytesLeft;
                var buffer = br.ReadBytes(readLength);
                result.SoundData.Add(buffer);

                bytesLeft -= buffer.Length;
            }

            return result;
        }

        public void Write(BinaryWriter bw)
        {
            WriteString(bw, "data");
            bw.Write((uint)SubchunkSize);

            foreach (var chunk in SoundData)
            {
                bw.Write(chunk);
            }
        }

        public void Append(Data rhs)
        {
            if (SubchunkId != rhs.SubchunkId)
                throw new ApplicationException("Both files need to have data chunks");

            SubchunkSize += rhs.SubchunkSize;

            foreach (var chunk in rhs.SoundData)
            {
                SoundData.Add(chunk);
            }
        }

        public long DataLength
        {
            get
            {
                var result = 0L;

                foreach (var chunk in SoundData)
                {
                    result += chunk.Length;
                }

                return result;
            }
        }
        private string SubchunkId { get; init; } = "data";
        private long SubchunkSize { get; set; }
        private List<byte[]> SoundData { get; } = [];
    }

    public class Junk
    {
        public static Junk Read(Subchunk subchunk, BinaryReader br)
        {
            var result = new Junk
            {
                SubchunkSize = subchunk.SubchunkSize,
            };

            var bytesLeft = result.SubchunkSize;

            while (bytesLeft > 0)
            {
                var readLength = bytesLeft > 5242880 ? 5242880 : (int)bytesLeft;
                var buffer = br.ReadBytes(readLength);

                bytesLeft -= buffer.Length;
            }

            return result;
        }

        public long SubchunkSize { get; private init; }
    }

    private static string ReadString(BinaryReader br, int length)
    {
        var bytes = br.ReadBytes(length);
        var value = Encoding.ASCII.GetString(bytes);

        return value;
    }

    private static void WriteString(BinaryWriter bw, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        bw.Write(bytes);
    }

    public static TimeSpan ReadTimeSpan(Stream stream)
    {
        using var br = new BinaryReader(stream, Encoding.Default, true);

        _ = Riff.Read(br);
        Fmt? fmt = null;
        Junk? data = null;

        while (data is null || fmt is null)
        {
            var subchunk = Subchunk.Read(br);

            switch (subchunk.SubchunkId)
            {
                case "data":
                    data = Junk.Read(subchunk, br);
                    break;
                case "fmt ":
                    fmt = Fmt.Read(subchunk, br);
                    break;
                case "JUNK":
                    Junk.Read(subchunk, br);
                    break;
                default:
                    throw new InvalidOperationException($"Don't know what to do with a {subchunk.SubchunkId} chunk");
            }
        }

        var seconds = data.SubchunkSize / fmt.ByteRate + 1;

        return TimeSpan.FromSeconds(seconds);
    }

    public static void CombineWavSources(List<Stream> sources, Stream destination)
    {
        Riff? dRiff = null;
        Fmt? dFmt = null;
        Data? dData = null;
        using var bw = new BinaryWriter(destination);

        foreach (var source in sources)
        {
            using var br = new BinaryReader(source);
            var riff = Riff.Read(br);
            Fmt? fmt = null;
            Data? data = null;

            while (fmt is null || data is null)
            {
                var subchunk = Subchunk.Read(br);

                switch (subchunk.SubchunkId)
                {
                    case "data":
                        data = Data.Read(subchunk, br);
                        break;
                    case "fmt ":
                        fmt = Fmt.Read(subchunk, br);
                        break;
                    case "JUNK":
                        Junk.Read(subchunk, br);
                        riff.ChunkSize -= subchunk.SubchunkSize + 8;
                        break;
                    default:
                        throw new InvalidOperationException($"Don't know what to do with a {subchunk.SubchunkId} chunk");
                }
            }

            if (dRiff is not null)
            {
                dRiff.Append(riff);
            }
            else
            {
                dRiff = riff;
            }

            if (dFmt is not null)
            {
                dFmt.Append(fmt);
            }
            else
            {
                dFmt = fmt;
            }

            if (dData is not null)
            {
                dData.Append(data);
            }
            else
            {
                dData = data;
            }
        }

        if (dRiff is null || dFmt is null || dData is null)
        {
            throw new InvalidOperationException("No files were provided");
        }

        dRiff.Write(bw);
        dFmt.Write(bw);
        dData.Write(bw);
    }
}
