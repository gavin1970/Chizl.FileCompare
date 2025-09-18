using System;
using System.Drawing;
using System.Collections.Generic;

namespace Chizl.Rtf
{
    internal enum Color_Appearance
    {
        Foreground = 0,
        Background = 1,
    }

    internal class RtfColorManager
    {
        readonly string[] ColorAppearances = new string[2] { @"\cf", @"\chcbpat" };

        private readonly List<Color> _colors = new List<Color>();
        public RtfColorManager(Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException(nameof(colors));

            _colors.Add(Color.Transparent); // Index 0 = FG \cf0 or BG \chcbpat0
            foreach (Color c in colors)
            {
                if (!_colors.Contains(c))
                    _colors.Add(c);
            }
        }
        public string GetIndex(Color color, Color_Appearance appearance) => $"{ColorAppearances[(int)appearance]}{GetIndex(color)} ";
        public int GetIndex(Color color)
        {
            int index = _colors.FindIndex(c => c == color);
            return index == -1 ? 0 : index; // -1 (not found, defaults to (0) - No Color)
        }
        public Color GetColor(int index)
        {
            if (index < 0 || index >= _colors.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _colors[index];
        }
        public Color[] ColorArray => _colors.ToArray();
        public int Count => _colors.Count;
    }
}