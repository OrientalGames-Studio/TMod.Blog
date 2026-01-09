using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Domain.Common
{
    public class PatternMode(string? mode)
    {
        public string Mode { get; set; } = string.IsNullOrWhiteSpace(mode) ? "UNKNOW" : mode.ToUpper();

        /// <summary>
        /// 完全模式
        /// </summary>
        public static PatternMode Full => new PatternMode("full");

        /// <summary>
        /// 模糊模式
        /// </summary>
        public static PatternMode Likely => new PatternMode("likely");
    }

}
