using Application.Interfaces;
using SharpToken;

namespace Infrastructure.Services
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
