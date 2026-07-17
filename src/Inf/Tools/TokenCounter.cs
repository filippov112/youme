using Core.Tools;
using SharpToken;

namespace Inf.Tools
{
    public class TokenCounter : ITokenCounterTool
    {
        public int CalcTokenCount(string text)
        {
            var encoding = GptEncoding.GetEncoding("cl100k_base");
            var tokens = encoding.Encode(text);

            return tokens.Count();
        }
    }
}
