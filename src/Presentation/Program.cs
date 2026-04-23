
using Presentation.Services;
using System.Text;

namespace Presentation
{
    public class Program
    {
        public static StorageService Storage { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Storage = new StorageService();
            Storage.LoadGlobalConfig();

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}