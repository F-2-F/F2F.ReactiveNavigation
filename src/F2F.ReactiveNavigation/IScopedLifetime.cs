using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Models the lifetime of an object. If the scope gets disposed, the object it contains
    /// can longer be used. Instead of requiring the object to implement IDisposable and
    /// introduce IDisposable cascades, a single disposable controls the lifetime of all
    /// disposable objects that were needed to construct the object. 
    /// Furthermore, the interface also makes the lifetime of objects visible in the code.
    /// Whenever you pass an instance of this type, you give up on the lifetime of the object
    /// and transfer that control to the called object. With this it is obvious from reading 
    /// the code, who is in control of the lifetime of an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IScopedLifetime<out T> : IDisposable
            where T : class
    {
        T Object { get; }

        IDisposable Scope { get; }
    }
}