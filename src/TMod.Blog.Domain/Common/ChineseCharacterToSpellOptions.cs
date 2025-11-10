using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Common
{
    public sealed class ChineseCharacterToSpellOptions
    {
        public static ChineseCharacterToSpellOptions Default => new()
        {
            AbbreviationOnly = false,
            PatternMode = PatternMode.Full,
            Separator = '|'
        };

        public static ChineseCharacterToSpellOptions LikelyPattern => new()
        {
            AbbreviationOnly = false,
            PatternMode = PatternMode.Likely,
            Separator = '|'
        };

        public static ChineseCharacterToSpellOptions AbbreviationOnlyPattern => new()
        {
            AbbreviationOnly = true,
            PatternMode = PatternMode.Full,
            Separator = '|'
        };

        /// <summary>
        /// 是否仅返回首字母缩写,只有完全匹配<see cref="PatternMode.Full"/>时有效
        /// </summary>
        public bool AbbreviationOnly { get; set; }

        /// <summary>
        /// 匹配模式
        /// </summary>
        public PatternMode PatternMode { get; set; } = PatternMode.Likely;

        /// <summary>
        /// 模糊匹配时，返回多个拼音结果的分隔符
        /// </summary>
        public char Separator { get; set; } = '|';
    }

}
