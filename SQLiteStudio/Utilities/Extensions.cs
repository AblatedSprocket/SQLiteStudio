namespace SQLiteStudio.Utilities
{
    public static class Extensions
    {
        public static bool IsNumeric(this string value)
        {
            if (int.TryParse(value, out int outInt))
            {
                return true;
            }
            if (double.TryParse(value, out double outDouble))
            {
                return true;
            }
            return false;
        }
    }
}
