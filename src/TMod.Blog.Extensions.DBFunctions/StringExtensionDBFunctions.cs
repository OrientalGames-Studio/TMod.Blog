using Microsoft.SqlServer.Server;

using System;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace TMod.Blog.Extensions.DBFunctions
{
    public static class StringExtensionDBFunctions
    {
        private static readonly Regex _nonAlnum = new Regex(@"[^\p{L}\p{N}\-]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [SqlFunction(IsDeterministic = true, IsPrecise = false)]
        public static SqlBoolean IsSimilarTo(SqlString s1,SqlString s2,SqlDouble threshold)
        {
            return GetSimilarity(s1,s2, new SqlDouble(0.6), new SqlDouble(0.4)) >= threshold;
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = false)]
        public static SqlDouble GetSimilarity(SqlString s1, SqlString s2, SqlDouble levWeight, SqlDouble jwWeight)
        {
            var a = NormalizeForComparison(s1);
            var b = NormalizeForComparison(s2);
            if ( string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b) )
            {
                return new SqlDouble(1.0);
            }
            double lev = LevenshteinSimilarity(a, b);
            double jw = JaroWinklerSimilarity(a, b);
            return levWeight * lev + jwWeight * jw;
        }

        private static string NormalizeForComparison(SqlString input)
        {
            if ( input == null || input.IsNull || string.IsNullOrWhiteSpace(input.Value) )
            {
                return string.Empty;
            }
            string s = input.Value.Trim().ToLowerInvariant();
            // 保留字母数字和连字符，其他替换为空格
            s = _nonAlnum.Replace(s, " ");
            // 把空白替换为单连字符
            s = Regex.Replace(s, @"\s+", "-");
            // 合并多个连字符
            s = Regex.Replace(s, "-+", "-");
            s = s.Trim('-');
            return s;
        }

        private static double LevenshteinSimilarity(string a, string b)
        {
            a = a ?? string.Empty;
            b = b ?? string.Empty;
            if ( a.Length == 0 && b.Length == 0 ) return 1.0;
            int dist = LevenshteinDistance(a,b);
            int max = Math.Max(a.Length, b.Length);
            if ( max == 0 ) return 1.0;
            return 1.0 - ( double )dist / max;
        }

        private static int LevenshteinDistance(string a, string b)
        {
            int n = a.Length, m = b.Length;
            if ( n == 0 ) return m;
            if ( m == 0 ) return n;

            // 使用一维滚动数组节省内存
            int[] prev = new int[m + 1];
            int[] curr = new int[m + 1];
            for ( int j = 0; j <= m; j++ ) prev[j] = j;

            for ( int i = 1; i <= n; i++ )
            {
                curr[0] = i;
                char ca = a[i - 1];
                for ( int j = 1; j <= m; j++ )
                {
                    int cost = ca == b[j - 1] ? 0 : 1;
                    int deletion = prev[j] + 1;
                    int insertion = curr[j - 1] + 1;
                    int substitution = prev[j - 1] + cost;
                    curr[j] = Math.Min(Math.Min(deletion, insertion), substitution);
                }
                // swap
                var tmp = prev; prev = curr; curr = tmp;
            }
            return prev[m];
        }

        private static double JaroWinklerSimilarity(string s1, string s2, double p = 0.1, int maxPrefixLength = 4)
        {
            double jaro = JaroSimilarity(s1, s2);
            if ( jaro <= 0.0 ) return 0.0;

            int prefix = 0;
            int max = Math.Min(Math.Min(s1?.Length ?? 0, s2?.Length ?? 0), maxPrefixLength);
            for ( int i = 0; i < max; i++ )
            {
                if ( s1[i] == s2[i] ) prefix++;
                else break;
            }
            return jaro + prefix * p * ( 1 - jaro );
        }

        private static double JaroSimilarity(string s1, string s2)
        {
            int len1 = s1.Length;
            int len2 = s2.Length;
            if ( len1 == 0 ) return len2 == 0 ? 1.0 : 0.0;
            int matchDistance = Math.Max(len1, len2) / 2 - 1;

            bool[] s1Matches = new bool[len1];
            bool[] s2Matches = new bool[len2];

            int matches = 0;
            for ( int i = 0; i < len1; i++ )
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance, len2 - 1);
                for ( int j = start; j <= end; j++ )
                {
                    if ( s2Matches[j] ) continue;
                    if ( s1[i] != s2[j] ) continue;
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if ( matches == 0 ) return 0.0;

            double t = 0.0;
            int k = 0;
            for ( int i = 0; i < len1; i++ )
            {
                if ( !s1Matches[i] ) continue;
                while ( !s2Matches[k] ) k++;
                if ( s1[i] != s2[k] ) t += 0.5;
                k++;
            }

            double m = matches;
            return ( m / len1 + m / len2 + ( m - t ) / m ) / 3.0;
        }
    }
}
