using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Chizl.FileCompare
{
    internal static class BinaryComparer
    {
        const int _foreSight = 16;
        static readonly Encoding _encode = Encoding.UTF8;

        public static ComparisonResults CompareFiles(FileLevel sourceFile, FileLevel targetFile)
        {
            var arrDiffs = new List<CompareDiff>();
            var trgLen = targetFile.Size.Format_ByteSize;
            var srcLen = sourceFile.Size.Format_ByteSize;
            var maxSize = Math.Max(srcLen, trgLen);

            while (maxSize > targetFile.Pointer && maxSize > sourceFile.Pointer)
            {
                var trgReadLen = Math.Min(_foreSight, trgLen - targetFile.Pointer);
                var srcReadLen = Math.Min(_foreSight, srcLen - sourceFile.Pointer);

                var trgSpan = targetFile.Bytes.AsSpan(targetFile.Pointer, trgReadLen);
                var srcSpan = sourceFile.Bytes.AsSpan(sourceFile.Pointer, srcReadLen);

                if (trgSpan.SequenceEqual(srcSpan))
                {
                    arrDiffs.Add(new CompareDiff(DiffType.None, targetFile.Pointer, _encode, trgSpan.ToArray()));

                    targetFile.Pointer += trgReadLen;
                    sourceFile.Pointer += srcReadLen;
                }
                else
                {
                    int initModIndex = targetFile.Pointer;
                    int initOrgIndex = sourceFile.Pointer;

                    while (initModIndex < trgLen &&
                           initOrgIndex < srcLen &&
                           targetFile.Bytes[initModIndex] == sourceFile.Bytes[initOrgIndex])
                    {
                        initModIndex++;
                        initOrgIndex++;
                    }

                    if (initModIndex > targetFile.Pointer)
                    {
                        var matchedSpan = targetFile.Bytes.AsSpan(targetFile.Pointer, initModIndex - targetFile.Pointer);
                        arrDiffs.Add(new CompareDiff(DiffType.None, targetFile.Pointer, _encode, matchedSpan.ToArray()));

                        // adding
                        targetFile.Pointer = initModIndex;
                        sourceFile.Pointer = initOrgIndex;
                        continue;
                    }

                    targetFile.Pointer = initModIndex;
                    sourceFile.Pointer = initOrgIndex;

                    if (!FindMatch(targetFile.Bytes, targetFile.Pointer, sourceFile.Bytes, sourceFile.Pointer, out int foundNext))
                    {
                        if (!FindMatch(sourceFile.Bytes, sourceFile.Pointer, targetFile.Bytes, targetFile.Pointer, out foundNext))
                        {
                            if (foundNext <= sourceFile.Pointer)
                                foundNext = sourceFile.Pointer + 1;
                            if (sourceFile.Pointer >= sourceFile.Bytes.Length)
                            {
                                if (targetFile.Pointer >= trgLen)
                                    break;

                                var addedSpan = targetFile.Bytes.AsSpan(targetFile.Pointer, trgLen - targetFile.Pointer);
                                arrDiffs.Add(new CompareDiff(DiffType.Added, targetFile.Pointer, _encode, addedSpan.ToArray()));
                                break;
                            }

                            var deletedSpan = sourceFile.Bytes.AsSpan(sourceFile.Pointer, foundNext - sourceFile.Pointer);
                            arrDiffs.Add(new CompareDiff(DiffType.Deleted, sourceFile.Pointer, _encode, deletedSpan.ToArray()));
                            sourceFile.Pointer = foundNext;
                        }
                        else
                        {
                            if (foundNext <= targetFile.Pointer)
                                foundNext = targetFile.Pointer + 1;
                            if (targetFile.Pointer >= trgLen)
                            {
                                if (sourceFile.Pointer >= trgLen)
                                    break;

                                var deletedSpan2 = sourceFile.Bytes.AsSpan(sourceFile.Pointer, srcLen - sourceFile.Pointer);
                                arrDiffs.Add(new CompareDiff(DiffType.Deleted, sourceFile.Pointer, _encode, deletedSpan2.ToArray()));
                                break;
                            }

                            var addedSpan = targetFile.Bytes.AsSpan(targetFile.Pointer, foundNext - targetFile.Pointer);
                            arrDiffs.Add(new CompareDiff(DiffType.Added, targetFile.Pointer, _encode, addedSpan.ToArray()));
                            targetFile.Pointer = foundNext;
                        }
                    }
                    else
                    {
                        if (sourceFile.Pointer >= srcLen)
                        {
                            if (targetFile.Pointer >= trgLen)
                                break;

                            var addedSpan = targetFile.Bytes.AsSpan(targetFile.Pointer, trgLen - targetFile.Pointer);
                            arrDiffs.Add(new CompareDiff(DiffType.Added, targetFile.Pointer, _encode, addedSpan.ToArray()));
                            break;
                        }

                        var matchedSpan = sourceFile.Bytes.AsSpan(sourceFile.Pointer, foundNext - sourceFile.Pointer);
                        arrDiffs.Add(new CompareDiff(DiffType.None, sourceFile.Pointer, _encode, matchedSpan.ToArray()));
                        sourceFile.Pointer = foundNext;
                    }
                }
            }

            return new ComparisonResults(arrDiffs, sourceFile, targetFile);
        }
        private static ConcurrentDictionary<string, int> BuildHashLookups(byte[] byteData, int start, int length, byte startingByte)
        {
            ConcurrentDictionary<string, int> retVal = new ConcurrentDictionary<string, int>();
            var maxLoopCount = length * 2;

            for (int i = start; i < byteData.Length; i++, maxLoopCount--)
            {
                while (i < byteData.Length && byteData[i] != startingByte) i++;

                if (i == byteData.Length)
                    break;

                while (byteData.Length - 1 < i + length)
                    length--;

                var bytes = byteData.Skip(i).Take(length).ToArray();
                retVal.TryAdd(CHash.GetFnv1aHash(bytes), i);

                // if (maxLoopCount <= 0 || retVal.Count == length)
                if (maxLoopCount <= 0 || retVal.Count == (length * 2))
                    break;
            }

            return retVal;
        }
        private static bool FindMatch(byte[] srcBytes, int srcIndex, byte[] trgBytes, int trgIndex, out int srcNext)
        {
            int byteMatch = _foreSight; // adjustable during search.
            var sizeSearch = 8;

            int initSrcIndex = srcIndex;
            int initTrgIndex = trgIndex;

            bool hasStarted = false;
            srcNext = srcIndex;

            if (initSrcIndex >= srcBytes.Length || initTrgIndex >= trgBytes.Length)
                return false;

            while (srcBytes[initSrcIndex].Equals(trgBytes[initTrgIndex]))
            {
                initSrcIndex++;
                initTrgIndex++;
            }

            int orgInitSrcIndex = initSrcIndex;
            int orgInitTrgIndex = initTrgIndex;

            if (initSrcIndex + sizeSearch > srcBytes.Length)
                sizeSearch = srcBytes.Length - initSrcIndex;

            var searchSrcBytes = srcBytes.AsSpan(initSrcIndex, sizeSearch).ToArray();
            var quickSearch = CHash.GetFnv1aHash(searchSrcBytes);
            var hashLookupData = BuildHashLookups(trgBytes, initTrgIndex, sizeSearch, srcBytes[initSrcIndex]);

            if (hashLookupData.TryGetValue(quickSearch, out var index))
            {
                srcNext = index;
                return true;
            }

            // when 'sizeSearch' byte grouping isn't found, lets try '_foreSight' byte,
            // that adjusts in half each round and has a 1 byte slide.
            while (byteMatch > 2)
            {
                hasStarted = false;
                if (initSrcIndex + byteMatch > srcBytes.Length)
                    byteMatch = srcBytes.Length - initSrcIndex;

                var searchForBytes = srcBytes.AsSpan(initSrcIndex, byteMatch);
                for (var i = 0; i < _foreSight - byteMatch; i++)
                {
                    var ndx = initTrgIndex + i;
                    if (trgBytes.Length < ndx + byteMatch)
                        break;

                    // searching 4 byte chunk matches
                    var curBytes = trgBytes.AsSpan(ndx, byteMatch);
                    var isEqual = searchForBytes.SequenceEqual(curBytes);

                    if (!hasStarted && !isEqual)
                        hasStarted = true;
                    else if (hasStarted && isEqual)
                    {
                        srcNext = ndx;
                        return true;
                    }
                }

                if (srcNext > srcIndex)
                    return true;
                else
                {
                    // reset with new byteMatch
                    initSrcIndex = orgInitSrcIndex;
                    initTrgIndex = orgInitTrgIndex;

                    byteMatch /= 2;
                }
            }

            return false;
        }
    }
}
