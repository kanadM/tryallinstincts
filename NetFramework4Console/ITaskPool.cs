using System;

namespace NetFramework4Console
{
    public interface ITaskPool
    {
        int GetQueueSize();
        void Enqueue(Action action);
        void StopAdding();
    }
}
