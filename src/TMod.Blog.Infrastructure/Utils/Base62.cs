using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMod.Blog.Infrastructure.Utils
{
    internal static class Base62
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static string Encode(ulong value)
        {
            if ( value == 0 ) return "0";
            var sb = new StringBuilder();
            while ( value > 0 )
            {
                var idx = (int)(value % 62);
                sb.Insert(0, Alphabet[idx]);
                value /= 62;
            }
            return sb.ToString();
        }

        public static bool TryDecode(string s, out ulong value)
        {
            value = 0;
            if ( string.IsNullOrEmpty(s) ) return false;
            foreach ( var c in s )
            {
                int v = Alphabet.IndexOf(c);
                if ( v < 0 ) { value = 0; return false; }
                value = value * 62 + ( ulong )v;
            }
            return true;
        }
    }
}
