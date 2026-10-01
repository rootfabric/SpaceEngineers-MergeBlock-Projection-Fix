using System;

namespace VRage.Plugins
{
    // Compile-time reference surface only.
    // This assembly is never shipped with the plugin.
    // At runtime Space Engineers supplies its own VRage.dll containing this interface.
    public interface IPlugin : IDisposable
    {
        void Init(object gameInstance);
        void Update();
    }
}
