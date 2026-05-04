using Application.Interfaces;

namespace Infrastructure.Services
{
    public class BufferExchange : IBufferExchange
    {
        public void Copy(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
