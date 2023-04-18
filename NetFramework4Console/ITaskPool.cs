using System;

namespace NetFramework4Console
{
    public interface ITaskPool
    {
        void Enqueue(Action action);
        void StopAdding();
    }
}
