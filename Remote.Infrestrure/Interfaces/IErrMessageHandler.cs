using System;

namespace Remote.Infrastructure.Interfaces
{
    public interface IErrMessageHandler
    {
        event EventHandler<string> OnErrorMessage;
        string ErrorMessage { get; set; }
    }
}
