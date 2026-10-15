namespace LZ.Core.Interface
{
    public interface ILzLog
    {
        void Debug(object message);

        void Debug(object message, Exception ex);

        void Error(object message);

        void Error(object message, Exception exception);
        void Info(object message);

        void Info(object message, Exception ex);

        void Warn(object message);

        void Warn(object message, Exception ex);
    }
}
