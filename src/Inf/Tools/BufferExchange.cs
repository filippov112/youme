using Core.Tools;

namespace Inf.Tools
{
    public class BufferExchange : IBufferExchangeTool
    {
        public void Copy(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
