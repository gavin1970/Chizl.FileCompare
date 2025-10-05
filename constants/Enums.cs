namespace Chizl.FileCompare
{
    /// <summary>
    /// LoadContentType: Canceled, InMemory, or Stream
    /// </summary>
    public enum LoadContentType
    {
        Canceled,
        /// <summary>
        /// Loaded in memory as byte array.  e.g. byte[]
        /// </summary>
        InMemory,
        /// <summary>
        /// Using FileStream
        /// </summary>
        Stream
    }
    /// <summary>
    /// DiffType: None, Added, Deleted, or Modified.
    /// </summary>
    public enum DiffType
    {
        /// <summary>
        /// Default, no changes exists
        /// </summary>
        None,
        /// <summary>
        /// Byte never exists and is now been added to the new file
        /// </summary>
        Added,
        /// <summary>
        /// Byte existed in old file and has been deleted from new file.
        /// </summary>
        Deleted,
        /// <summary>
        /// Modified only appears for ASCII diffs.  This represents the line that this byte is in has<br/>
        /// been modified.  Within this modified line, there will be an 'Add' and/or 'Delete' DiffType set.
        /// </summary>
        Modified
    }
    /// <summary>
    /// FileFormat: NotSet, Ascii, or Binary
    /// </summary>
    public enum FileFormat
    {
        /// <summary>
        /// Default, hasn't been checked yet.
        /// </summary>
        NotSet,
        /// <summary>
        /// Interal method does a pre-check BOM detection for UTF encodings, "future" ascii, in the following order:<br/>
        /// If (UTF-16 LE) = true
        /// If (UTF-16 BE) = true
        /// If (UTF-8 BOM) = true
        /// If (UTF-32 LE) = true
        /// If (UTF-32 BE) = true
        /// If Not BOM, Scan the first 1024 bytes. If less than 2 string terminators, e.g. '0x00' are found = ASCII = true
        /// </summary>
        Ascii,
        /// <summary>
        /// Interal method does a pre-check BOM detection for UTF encodings, "future" ascii, in the following order then does a binary check:<br/>
        /// If (UTF-16 LE) = false
        /// If (UTF-16 BE) = false
        /// If (UTF-8 BOM) = false
        /// If (UTF-32 LE) = false
        /// If (UTF-32 BE) = false
        /// If Not BOM, Scan the first 1024 bytes. If 2 or more string terminators, e.g. '0x00' are found = Binary = true
        /// </summary>
        Binary,
    }
}
