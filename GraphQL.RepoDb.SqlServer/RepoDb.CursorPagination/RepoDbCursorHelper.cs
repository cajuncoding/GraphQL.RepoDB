using System;

namespace RepoDb.CursorPagination
{
    /// <summary>
    /// Helper Class for serializing and deserializing Opaque cursors from Indexed based result sets.
    /// </summary>
    public static class RepoDbCursorHelper
    {
        public static string CreateCursor(int index)
        {
            var cursor = Convert.ToBase64String(BitConverter.GetBytes(index));
            return cursor;
        }

        public static int ParseCursor(string cursor)
        {
            int index = BitConverter.ToInt32(Convert.FromBase64String(cursor));
            return index;
        }
    }
}
