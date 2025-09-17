using System;
using System.IO;
using System.Linq;
using FluentAssertions;

namespace Chizl.FileCompare.Tests
{
    public class DiffToolTests : IDisposable
    {
        private readonly string _tempDir;

        public DiffToolTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "ChizlDiffTests");
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private string CreateTempFile(string name, byte[] bytes)
        {
            var path = Path.Combine(_tempDir, name);
            File.WriteAllBytes(path, bytes);
            return path;
        }

        private string CreateTempFile(string name, string[] lines)
        {
            var path = Path.Combine(_tempDir, name);
            File.WriteAllLines(path, lines);
            return path;
        }

        [Fact]
        public void CompareFiles_IdenticalAscii_ReturnsNoChanges()
        {
            var file1 = CreateTempFile("file1.txt", new[] { "line1", "line2" });
            var file2 = CreateTempFile("file2.txt", new[] { "line1", "line2" });

            var result = DiffTool.CompareFiles(file1, file2);

            result.HasException.Should().BeFalse();
            result.IsBinary.Should().BeFalse();
            result.Diffs.Added.Should().Be(0);
            result.Diffs.Deleted.Should().Be(0);
            result.Diffs.Modified.Should().Be(0);
        }

        [Fact]
        public void CompareFiles_ModifiedAscii_ReturnsModify()
        {
            var file1 = CreateTempFile("file1.txt", new[] { "using System;", "using IO;" });
            var file2 = CreateTempFile("file2.txt", new[] { "using System;", "using IO2;" });

            var result = DiffTool.CompareFiles(file1, file2);

            result.LineComparison.Any(l => l.DiffType == DiffType.Modified).Should().BeTrue();
        }

        [Fact]
        public void CompareFiles_InsertsAndDeletes_ReturnsCorrectCounts()
        {
            var file1 = CreateTempFile("file1.txt", new[] { "line1", "line2", "line3" });
            var file2 = CreateTempFile("file2.txt", new[] { "line1", "line2a", "line4" });

            var result = DiffTool.CompareFiles(file1, file2);

            result.Diffs.Added.Should().Be(1);      // line4
            result.Diffs.Deleted.Should().Be(1);    // line2 -> line2a
            result.Diffs.Modified.Should().Be(1);   // merged line2 -> line2a
        }

        [Fact]
        public void CompareFiles_BinaryFiles_ReturnsIsBinaryTrue()
        {
            var file1 = CreateTempFile("file1.bin", new byte[] { 0, 1, 2, 3 });
            var file2 = CreateTempFile("file2.bin", new byte[] { 0, 1, 2, 4 });

            var result = DiffTool.CompareFiles(file1, file2);

            result.IsBinary.Should().BeTrue();
        }

        [Fact]
        public void CompareFiles_MissingFile_ThrowsException()
        {
            var file1 = Path.Combine(_tempDir, "doesnotexist.txt");
            var file2 = CreateTempFile("file2.txt", new[] { "line1" });

            var result = DiffTool.CompareFiles(file1, file2);

            result.HasException.Should().BeTrue();
            result.Exception.Should().BeOfType<ArgumentException>();
        }
    }
}
