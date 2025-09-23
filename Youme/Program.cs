
using Youme.Services;

namespace Youme
{
    public class Program
    {
        public static StorageService Storage { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            Storage = new StorageService();
            Storage.LoadGlobalConfig();

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}