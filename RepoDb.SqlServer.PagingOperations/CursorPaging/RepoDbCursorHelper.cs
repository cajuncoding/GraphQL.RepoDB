using System;

namespace RepoDb.CursorPaging
{
    /// <summary>
    /// Helper Class for serializing and deserializing Opaque cursors from Indexed based result sets.
    /// </summary>
    public static class RepoDbCursorHelper
    {
        public static string CreateCursor(int index)
            => Convert.ToBase64String(BitConverter.GetBytes(index));

        public static int ParseCursor(string cursor)
            => BitConverter.ToInt32(Convert.FromBase64String(cursor), 0);
    }
}
