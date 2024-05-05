namespace TeleMed.Services;

using System;

public class CircuitHandlerService
{
    public event EventHandler<UnhandledExceptionEventArgs>? OnUnhandledException;

    public void TriggerUnhandledException(Exception exception, bool isTerminating)
    {
        OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(exception, isTerminating));
    }
}