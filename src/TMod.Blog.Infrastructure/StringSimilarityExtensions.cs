using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMod.Blog.Infrastructure
{
    /// <summary>
    /// 字符串相似度与标准化扩展，包含 Levenshtein 与 Jaro-Winkler 及组合相似度方法。
    /// 适合用于 slug/短码/短文本的近似匹配判定。
    /// </summary>
    public static class StringSimilarityExtensions
    {
        private static readonly Regex _nonAlnum = new Regex(@"[^\p{L}\p{N}\-]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// 将输入规整为用于相似度比较的形式：
        /// - 小写
        /// - 去掉多余空白/标点（保留字母/数字/连字符）
        /// - 连续连字符合并
        /// - 修剪前后连字符
        /// </summary>
        public static string NormalizeForComparison(this string? input)
        {
            if ( string.IsNullOrWhiteSpace(input) ) return string.Empty;
            string s = input.Trim().ToLowerInvariant();
            // 保留字母数字和连字符，其他替换为空格
            s = _nonAlnum.Replace(s, " ");
            // 把空白替换为单连字符
            s = Regex.Replace(s, @"\s+", "-");
            // 合并多个连字符
            s = Regex.Replace(s, "-+", "-");
            s = s.Trim('-');
            return s;
        }

        /// <summary>
        /// 计算 Levenshtein 编辑距离（两个字符串的最小编辑操作数）。
        /// 时间复杂度 O(n*m)。
        /// </summary>
        public static int LevenshteinDistance(this string? a, string? b)
        {
            a ??= string.Empty;
            b ??= string.Empty;
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

        /// <summary>
        /// 基于 Levenshtein 编辑距离计算归一化相似度（0..1），1 表示完全相同。
        /// <para>similarity = 1 - distance / maxLen</para>
        /// </summary>
        public static double LevenshteinSimilarity(this string? a, string? b)
        {
            a = a ?? string.Empty;
            b = b ?? string.Empty;
            if ( a.Length == 0 && b.Length == 0 ) return 1.0;
            int dist = a.LevenshteinDistance(b);
            int max = Math.Max(a.Length, b.Length);
            if ( max == 0 ) return 1.0;
            return 1.0 - ( double )dist / max;
        }

        /// <summary>
        /// Jaro 相似度（0..1）。Jaro-Winkler 的基础。
        /// 对短串与局部错位（transposition）敏感，常用于姓名/短文本相似匹配。
        /// </summary>
        public static double JaroSimilarity(this string? s1, string? s2)
        {
            s1 ??= string.Empty;
            s2 ??= string.Empty;
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

        /// <summary>
        /// Jaro-Winkler 相似度。对前缀匹配给予更高权重。
        /// 参数 p 最大为 0.25（常用 0.1）。
        /// </summary>
        public static double JaroWinklerSimilarity(this string? s1, string? s2, double p = 0.1, int maxPrefixLength = 4)
        {
            double jaro = JaroSimilarity(s1, s2);
            if ( jaro <= 0.0 ) return 0.0;

            int prefix = 0;
            int max = Math.Min(Math.Min(s1?.Length ?? 0, s2?.Length ?? 0), maxPrefixLength);
            for ( int i = 0; i < max; i++ )
            {
                if ( s1![i] == s2![i] ) prefix++;
                else break;
            }
            return jaro + prefix * p * ( 1 - jaro );
        }

        /// <summary>
        /// 组合相似度函数：对 slug 场景，先做 NormalizeForComparison，然后
        /// 返回 weighted(levenshteinSimilarity, jaroWinkler).
        /// 权重可调（默认 Levenshtein 0.6, JaroWinkler 0.4）。
        /// </summary>
        public static double GetSimilarity(this string? s1, string? s2, double levWeight = 0.6, double jwWeight = 0.4)
        {
            string a = s1.NormalizeForComparison();
            string b = s2.NormalizeForComparison();
            if ( string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b) ) return 1.0;
            double lev = LevenshteinSimilarity(a, b);
            double jw = JaroWinklerSimilarity(a, b);
            return levWeight * lev + jwWeight * jw;
        }

        /// <summary>
        /// 简便方法：判断两个字符串是否“近似相等”
        /// threshold (0..1)，建议对 slug 场景使用 0.75~0.9。
        /// </summary>
        public static bool IsSimilarTo(this string? s1, string? s2, double threshold = 0.85)
        {
            return GetSimilarity(s1, s2) >= threshold;
        }
    }
}
