namespace Chizl.FileCompare
{
    public enum LoadContentType
    {
        Canceled,
        InMemory,
        Stream
    }

    public enum DiffType
    {
        None,
        Added,
        Deleted,
        Modified
    }
    public enum FileFormat
    {
        NotSet,
        Ascii,
        Binary,
    }
}
