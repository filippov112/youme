using Pdp.App.Interfaces;
using SharpToken;

namespace Pdp.Inf.Services
{
    public class TokenCounter : ITokenCounter
    {
        public int CalcTokenCount(string text)
        {
            var encoding = GptEncoding.GetEncoding("cl100k_base");
            var tokens = encoding.Encode(text);

            return tokens.Count();
        }
    }
}
