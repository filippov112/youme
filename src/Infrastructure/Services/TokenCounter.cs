using Application.Interfaces;
using Domain.Models;
using SharpToken;
using System;
using System.Collections.Generic;
using System.Text;

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
