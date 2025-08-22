# Chizl.FileCompare
<img width="1384" height="821" alt="image" src="https://github.com/user-attachments/assets/60d313d1-8d8e-451b-955a-d3421b569d80" />

## About:
* No Dependencies
* (2) Projects:
  * DLL Library: &lt;TargetFrameworks&gt;netstandard2.0;netstandard2.1&lt;/TargetFrameworks&gt;
  * EXE UI: .NET Framework 4.8.1

To use the library it has 1 primary class `DiffTool` with 3 static methods with the same return model: `ComparisonResults`.
* File Compare, passing file paths into each.
    * `DiffTool.CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30, byte lineLookAhead = 3);`
* String Compare, passing old string vs latest string.  Threshhold is auto set to 10% to ensure detail for a single line.
    * `DiffTool.CompareString(string srcText, string trgText);`
* String Array Compare, also used by File Compare above.
    * `DiffTool.CompareStringArr(string[] linesOld, string[] linesNew, double scoreThreshold = 0.30, byte lineLookAhead = 3);`

The library builds a comparison between 2 files, strings, or string array and returns this Class Model `ComparisonResults`.  It also allows a threshold and line look ahead option.
* `Threshold` - Default and Suggested: 30%-60%.  This can be seen in the image above, within the Menu.  If the percentage is too low, you might get more detailed, but also can make 2 lines that are nothing alike might show merged, when it's really a deleted line and a new line added in it's place.  If the percentage is too high the line might say deleted with a new line instead of a modified line.  It really depends on your need.
* `lineLookAhead` - Default and Suggested: 3.  Looking to far ahead might mix lines, or make modifications of 1 line look deleted with new lines been added.  To low and modifications might not be seen.
```csharp
    public class ComparisonResults
    {
        private ComparisonResults() { IsEmpty = true; }
        internal ComparisonResults(Exception ex) 
        { 
            HasException = true;
            IsEmpty = true; 
            Exception = ex; 
        }
        internal ComparisonResults(List<CompareDiff> compareDiffs)
        {
            this.LineComparison = compareDiffs.ToArray();
            this.Diffs = new Counts(
                compareDiffs.Where(w => w.DiffType == DiffType.Added).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Modified).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.None).Count()
            );
        }

        public static ComparisonResults Empty { get; } = new ComparisonResults();
        public bool IsEmpty { get; } = false;
        public bool HasException { get; } = false;

        public Exception Exception { get; } = new Exception();
        public CompareDiff[] LineComparison { get; } = new CompareDiff[0];
        public Counts Diffs { get; } = new Counts(0, 0, 0, 0);

        public class Counts
        {
            internal Counts(int added, int deleted, int modified, int identical)
            {
                Added = added;
                Deleted = deleted;
                Modified = modified;
                Identical = identical;
            }
            public int Added { get; } = 0;
            public int Deleted { get; } = 0;
            public int Modified { get; } = 0;
            public int Identical { get; } = 0;
        }
    }
```

The property `LineComparison` returns a line by line array for side by side look for users that want to build their own UI.<br/>

Model `CompareDiff` has `LineDiffBytes`, that breaks down a line by character when modified, as seen on Line 8 of the above image.<br/>
Example: `this is a test` <--> `that was a test`<br/>
Results: `th[-i][+a][-s][+t] [-i][+w][+a]s a test`
```csharp
    public class CompareDiff
    {
        static readonly Encoding _defaultEncoding = Encoding.UTF8;

        private CompareDiff() {}
        private CompareDiff(DiffType diff, int line, Encoding enc)
        {
            if (diff.Equals(DiffType.NotSet))
                throw new ArgumentException("'DiffType.NotSet' is not a valid DiffType.");
            if (line < 0)
                throw new ArgumentException("'Diff Start' cannot be below 0.");

            DiffType = diff;
            LineNumber = line;
            EncodeType = enc;
        }

        internal CompareDiff(DiffType diff, int line, byte[] diffArray) : this(diff, line, _defaultEncoding, diffArray) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, byte[] diffArray) : this(diff, line, enc)
        {
            LineDiffBytes = diffArray;
            if (diffArray != null)
                LineDiffStr = EncodeType.GetString(diffArray);
        }

        internal CompareDiff(DiffType diff, int line, string diffStr) : this(diff, line, _defaultEncoding, diffStr) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, string diffStr) : this(diff, line, enc)
        {
            LineDiffStr = diffStr;
            if (!string.IsNullOrWhiteSpace(diffStr))
                LineDiffBytes = EncodeType.GetBytes(diffStr);
        }

        public static CharDiff[] LineBreakDown { get; } = new CharDiff[0];
        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static CompareDiff Empty { get; } = new CompareDiff();
        
        /// <summary>
        /// If CompareDiff wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get { return this.DiffType.Equals(DiffType.NotSet); } }
        /// <summary>
        /// Type of modification found.  Add, Delete, Modified, or None
        /// </summary>
        public DiffType DiffType { get; } = DiffType.NotSet;
        /// <summary>
        /// Line number created for UI matching and may not match line numbers in file.
        /// </summary>
        public int LineNumber { get; } = -1;
        /// <summary>
        /// Single line in byte[] format.
        /// </summary>
        public byte[] LineDiffBytes { get; } = new byte[0];
        /// <summary>
        /// Single line in string format.
        /// </summary>
        public string LineDiffStr { get; } = string.Empty;
        /// <summary>
        /// Encoding used for string to byte[] and back to string conversion.<br/>
        /// Default: UTF8
        /// </summary>
        public Encoding EncodeType { get; } = _defaultEncoding;
    }
```
