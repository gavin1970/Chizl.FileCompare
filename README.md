# Chizl.FileCompare
<img width="1384" alt="image" src="./imgs/ani_image.gif" />

## About:
* Dependencies:
  * &lt;PackageReference Include="System.Memory" Version="4.6.3" /&gt;
  * &lt;PackageReference Include="System.Collections.Concurrent" Version="4.3.0" /&gt;
* (3) Projects:
  * DLL Library: &lt;TargetFrameworks&gt;netstandard2.1;net8.0;net481&lt;/TargetFrameworks&gt;
  * EXE WinForm: .NET Framework 4.8.1
  * EXE Console: &lt;TargetFramework&gt;net8.0&lt;/TargetFramework&gt;

To use the library it has 1 primary class `DiffTool` with 3 static methods with the same return model: `ComparisonResults`.
* File Compare, passing file paths into each.  This can be Binary or ASCII files.  If either are Binary, both will be treated as Binary.
    * `DiffTool.CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30, byte lineLookAhead = 3);`
* String Compare, passing old string vs latest string.  Threshhold is auto set to 10% to ensure detail for a single line.
    * `DiffTool.CompareString(string srcText, string trgText);`
* String Array Compare, also used by File Compare above.
    * `DiffTool.CompareStringArr(string[] linesOld, string[] linesNew, double scoreThreshold = 0.30, byte lineLookAhead = 3);`

The library builds a comparison between 2 files, strings, byte, string array, and byte array and returns this Class Model `ComparisonResults`.  It also allows a threshold and line look ahead option for ASCII files, but ignored for Binary files.
* `Threshold` - Default and Suggested: 30%-60%.  This can be seen in the image above, within the Menu.  If the percentage is too low, you might get more detailed, but also can make 2 lines that are nothing alike might show merged, when it's really a deleted line and a new line added in it's place.  If the percentage is too high the line might say deleted with a new line instead of a modified line.  It really depends on your need.
* `lineLookAhead` - Default and Suggested: 3.  Looking to far ahead might mix lines, or make modifications of 1 line look deleted with new lines been added.  To low and modifications might not be seen.
```csharp
    public class ComparisonResults
    {
        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static ComparisonResults Empty { get; } = new ComparisonResults();
        /// <summary>
        /// If ComparisonResults wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get; } = false;
        /// <summary>
        /// If files compared were binary or not.
        /// </summary>
        public bool IsBinary { get; } = false;
        /// <summary>
        /// If an Exception occurs when files are to be loaded, this will be set to true.<br/>
        /// "Exception" property for this class will be set to exception reason.
        /// </summary>
        public bool HasException { get { return this.Exception != null; } }
        /// <summary>
        /// Only used when one of the files is missing when CompareFiles() is called.<br/>
        /// <code>
        /// if (!sourceFileInfo.Exists)
        ///     return new ComparisonResults(new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found and/or accessible."));
        /// if (!targetFileInfo.Exists)
        ///     return new ComparisonResults(new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found and/or accessible."));
        /// </code>
        /// </summary>
        public Exception Exception { get; } = null;
        /// <summary>
        /// Holds each line with each byte showing any differences that may exist.
        /// </summary>
        public CompareDiff[] LineComparison { get; } = new CompareDiff[0];
        /// <summary>
        /// Shows how many of each different change types were found.<br/>
        /// Add, Modified, Delete, or No Change
        /// </summary>
        public DiffCounts Diffs { get; } = new DiffCounts(0, 0, 0, 0);
    }

    public class DiffCounts
    {
        public int Added { get; } = 0;
        public int Deleted { get; } = 0;
        public int Modified { get; } = 0;
        public int Identical { get; } = 0;
    }

    public class CompareDiff
    {
        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static CompareDiff Empty { get; } = new CompareDiff();
        /// <summary>
        /// LineBreakDown is used for Modified lines and will be empty for New, Deleted, or unmodified lines.<br/>
        /// This is the help translate modified lines in a more structured manner.
        /// </summary>
        public ByteLevel[] ByteByByteDiff { get { return GetByteLevel(); } }
        /// <summary>
        /// If CompareDiff wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get { return this.DiffType.Equals(DiffType.None); } }
        /// <summary>
        /// Type of modification found.  Add, Delete, Modified, or None
        /// </summary>
        public DiffType DiffType { get; } = DiffType.None;
        /// <summary>
        /// Line number created for UI matching and may not match line numbers in file.
        /// </summary>
        public int LineNumber { get; } = -1;
        /// <summary>
        /// Single line in string format.
        /// </summary>
        public string LineDiffStr { get { return _lineDiffStr; } }
        /// <summary>
        /// Encoding used for string to byte[] and back to string conversion.<br/>
        /// Default: UTF8
        /// </summary>
        public Encoding EncodeType { get; } = _defaultEncoding;
    }

    public class ByteLevel
    {
        /// <summary>
        /// Byte state: None, Added, Modified, Deleted
        /// </summary>
        public DiffType DiffType { get; } = DiffType.None;
        /// <summary>
        /// Char representation of the byte.
        /// </summary>
        public char Char { get; } = '\0';
        /// <summary>
        /// Single character string representation of the byte.
        /// </summary>
        public string Str { get; } = string.Empty;
        /// <summary>
        /// Byte found in file.
        /// </summary>
        public byte Byte { get; } = 0;
        /// <summary>
        /// Hex version of the Byte
        /// </summary>
        public string Hex { get; } = string.Empty;
    }    
```

The property `LineComparison` returns a line by line array for side by side look for users that want to build their own UI.<br/>

Model `CompareDiff` has `ByteByByteDiff`, that breaks down a line by character when modified, as seen on Line 8 of the above image.<br/><br/>
Example: `this is a test` <--> `that was a test`
* CompareDiff.LineDiffStr: `th[-i][+a][-s][+t] [-i][+w][+a]s a test`
* CompareDiff.ByteByByteDiff: (Will look cleaner)
    * `t` - CharDiff.DiffType.Modified
    * `h` - CharDiff.DiffType.Modified
    * `i` - CharDiff.DiffType.Deleted
    * `a` - CharDiff.DiffType.Added
    * `s` - CharDiff.DiffType.Deleted
    * `t` - CharDiff.DiffType.Added
    * ` ` - CharDiff.DiffType.Modified
    * `i` - CharDiff.DiffType.Deleted
    * `w` - CharDiff.DiffType.Added
    * `a` - CharDiff.DiffType.Added
    * `s` - CharDiff.DiffType.Modified
    * ` ` - CharDiff.DiffType.Modified
    * `a` - CharDiff.DiffType.Modified
    * ` ` - CharDiff.DiffType.Modified
    * `t` - CharDiff.DiffType.Modified
    * `e` - CharDiff.DiffType.Modified
    * `s` - CharDiff.DiffType.Modified
    * `t` - CharDiff.DiffType.Modified

