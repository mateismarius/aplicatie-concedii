namespace Utilitar.Dictionaries
{
    public class ColumnsDictionary
    {
        public string ConertNumberToColumns(int num)
        {

            List<string> columns = new()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw"
            };
            string result = string.Empty;
            int lgt = columns.Count;
            int rest = num % lgt;
            int row = num / lgt;


            if (row == 0)
                result += columns[rest - 1];
            else if (row == 1 && rest > 0)
                result += columns[0] + columns[rest - 1];
            else
                result += columns[25];

            return result;
            
        }
    }
}
