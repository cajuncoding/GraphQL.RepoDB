using System;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Text;

namespace RepoDb.SqlServer.PagingOperations
{
    /// <summary>
    /// Helper Class for serializing and deserializing Opaque cursors from Indexed based result sets.
    /// </summary>
    public static class RepoDbCursorHelper
    {
        //Integers use 4 bytes!
        private const int IntegerByteLength = 4;
        private static readonly int IntegerUtf8EncodedMaxByteLength = Base64.GetMaxEncodedToUtf8Length(IntegerByteLength);
        private static readonly Encoding Utf8 = Encoding.UTF8;

        public static string CreateCursor(int cursorIndex)
        {
            Span<byte> bufferSpan = stackalloc byte[IntegerUtf8EncodedMaxByteLength];

            if (!BinaryPrimitives.TryWriteInt32LittleEndian(bufferSpan, cursorIndex))
                throw new ArgumentException($"Unable to write the specified Integer value [{cursorIndex}] to Span for fast Base64 conversion.");

            Base64.EncodeToUtf8InPlace(bufferSpan, IntegerByteLength, out var bytesWritten);
            var encodedSpan = bufferSpan.Slice(0, bytesWritten);

            var cursor = Utf8.GetString(encodedSpan);
            return cursor;
        }

        public static int ParseCursor(string cursor)
        {
            Span<byte> bufferSpan = stackalloc byte[IntegerUtf8EncodedMaxByteLength];

            Utf8.GetBytes(cursor.AsSpan(), bufferSpan);

            Base64.DecodeFromUtf8InPlace(bufferSpan, out var bytesWritten);
            var decodedSpan = bufferSpan.Slice(0, bytesWritten);

            if (!BinaryPrimitives.TryReadInt32LittleEndian(decodedSpan, out var cursorIndex))
                throw new ArgumentException($"Unable to parse the Integer Index value for the UTF8 Cursor value [{cursor}] specified.");

            return cursorIndex;
        }
    }

    #if NETSTANDARD2_0
    internal static class NetStandard20ShimExtensions
    {
        public static unsafe string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesPointer = bytes)
            {
                return encoding.GetString(bytesPointer, bytes.Length);
            }
        }

        public static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            fixed (char* charsPointer = chars)
            fixed (byte* bytesPointer = bytes)
            {
                return encoding.GetBytes(charsPointer, chars.Length, bytesPointer, bytes.Length);
            }
        }
    }
    #endif

}
