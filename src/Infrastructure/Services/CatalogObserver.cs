using Application.Interfaces;
using Application.Models;
using MediatR;
using Timer = System.Threading.Timer;

namespace Infrastructure.Services
{
    public class CatalogObserver : ICatalogObserver, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IConfigService _configService;
        private FileSystemWatcher? _watcher;
        private readonly Lock _lockObject = new();
        private Timer? _debounceTimer;
        private bool _disposed;

        public CatalogObserver(IMediator mediator, IConfigService configService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        public void StartObserving()
        {
            lock (_lockObject)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);

                StopObservingInternal();
                InitializeWatcher();
            }
        }

        public void StopObserving()
        {
            lock (_lockObject)
            {
                StopObservingInternal();
            }
        }

        private void InitializeWatcher()
        {
            var rootDirectory = _configService.RootDirectory;
            _watcher = new FileSystemWatcher(rootDirectory)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                InternalBufferSize = 65536
            };
            _watcher.Created += OnFileSystemEvent;
            _watcher.Deleted += OnFileSystemEvent;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnWatcherError;
            // _watcher.Changed += OnFileSystemEvent;
        }

        private void OnFileSystemEvent(object sender, FileSystemEventArgs e)
        {
            if (IsTemporaryFile(e.Name))
                return;
            NotifyCatalogChanged();
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (IsTemporaryFile(e.Name))
                return;
            NotifyCatalogChanged();
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();
            StartObserving();
        }

        private void NotifyCatalogChanged()
        {
            lock (_lockObject)
            {
                _debounceTimer?.Dispose();
                _debounceTimer = new Timer(
                    _ => OnDebounceTimerTick(),
                    null,
                    TimeSpan.FromMilliseconds(500), // Ждем 500ms тишины
                    Timeout.InfiniteTimeSpan
                );
            }
        }

        private void OnDebounceTimerTick()
        {
            _mediator.Publish(new CatalogChangedNotification());
        }

        private static bool IsTemporaryFile(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return true;

            // Фильтруем временные файлы
            var tempExtensions = new[] { ".tmp", ".~tmp", ".bak", "~$" };
            return tempExtensions.Any(ext => fileName.StartsWith(ext, StringComparison.OrdinalIgnoreCase) ||
                                              fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private void StopObservingInternal()
        {
            _debounceTimer?.Dispose();
            _debounceTimer = null;

            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= OnFileSystemEvent;
                _watcher.Deleted -= OnFileSystemEvent;
                _watcher.Renamed -= OnRenamed;
                _watcher.Error -= OnWatcherError;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_lockObject)
            {
                StopObservingInternal();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
