namespace altium.sorter
{
    public class ParsedStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            var xParts = x.Split(new[] { ". " }, 2, StringSplitOptions.None);
            var xNumber = int.Parse(xParts[0]);
            var xText = xParts[1];

            var yParts = y.Split(new[] { ". " }, 2, StringSplitOptions.None);
            var yNumber = int.Parse(yParts[0]);
            var yText = yParts[1];

            int textComparison = string.Compare(xText, yText, StringComparison.Ordinal);

            return textComparison == 0 ? xNumber.CompareTo(yNumber) : textComparison;
        }
    }
}