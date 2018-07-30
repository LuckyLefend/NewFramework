using System;

namespace FirClient.Manager
{
    public interface IThreadRunner
    {
        void Execute(Action action);
    }
}