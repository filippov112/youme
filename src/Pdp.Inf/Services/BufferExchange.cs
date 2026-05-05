using Pdp.App.Interfaces;

namespace Pdp.Inf.Services
{
    public class BufferExchange : IBufferExchange
    {
        public void Copy(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
