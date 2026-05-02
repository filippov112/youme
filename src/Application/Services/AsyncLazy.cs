namespace Application.Services
{
    // Вспомогательный класс для ленивой асинхронной инициализации
    public class AsyncLazy<T> where T : class
    {
        private readonly object _lock = new object();
        private readonly Func<Task<T>> _factory;
        private Task<T>? _task;
        private T? _cachedValue;

        public AsyncLazy(Func<Task<T>> factory)
        {
            _factory = factory;
        }

        public Task<T> GetValueAsync()
        {
            lock (_lock)
            {
                if (_cachedValue != null)
                    return Task.FromResult(_cachedValue);

                if (_task == null || _task.IsFaulted)
                    _task = _factory();

                return _task;
            }
        }

        public void Reset(T? newValue = null)
        {
            lock (_lock)
            {
                if (newValue != null)
                {
                    _cachedValue = newValue;
                    _task = Task.FromResult(newValue);
                }
                else
                {
                    _cachedValue = null;
                    _task = null;
                }
            }
        }
    }
}
