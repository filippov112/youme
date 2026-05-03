using Domain.Models;
using Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Services
{
    public class BufferExchange : IBufferExchange
    {
        public void Copy(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
