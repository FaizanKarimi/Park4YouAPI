using System.Runtime.CompilerServices;

namespace Components.Services.Interfaces
{
    public interface ILogging
    {
        void Debug(string message = "", string emailAddress = "", [CallerMemberName]string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "");
    }
}