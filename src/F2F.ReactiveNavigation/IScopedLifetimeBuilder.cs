using System.Collections.Generic;
using System.Linq;

namespace System
{
    public interface IScopedLifetimeBuilder<T>
                where T : class
    {
        /// <summary>
        /// Lifetime of object can be ended with disposing the given disposable.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        ScopedLifetime<T> EndingWith(IDisposable disposable);

        /// <summary>
        /// Lifetime of object can be ended with executing the given disposeAction.
        /// </summary>
        /// <param name="disposeAction"></param>
        /// <returns></returns>
        ScopedLifetime<T> EndingWith(Action disposeAction);
    }
}
