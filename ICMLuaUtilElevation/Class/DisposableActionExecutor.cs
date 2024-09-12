using System;
using System.Threading;

namespace ICMLuaUtilElevation.Class
{
    internal sealed class DisposableActionExecutor : IDisposable
    {
        private Action ActionOnDispose;

        public DisposableActionExecutor()
        {

        }

        public DisposableActionExecutor(Action ActionOnDispose)
        {
            this.ActionOnDispose = ActionOnDispose;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Interlocked.Exchange(ref ActionOnDispose, null) is Action Executor)
            {
                Executor.Invoke();
            }
        }

        ~DisposableActionExecutor()
        {
            Dispose();
        }
    }
}
