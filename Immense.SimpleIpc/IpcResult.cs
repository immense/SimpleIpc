#nullable disable
using System;

namespace SimpleIpc
{
    public class IpcResult
    {
        public static IpcResult<T> Empty<T>()
        {
            return new IpcResult<T>(true, default);
        }

        public static IpcResult Fail(string error)
        {
            return new IpcResult(false, error);
        }
        public static IpcResult Fail(Exception ex)
        {
            return new IpcResult(false, null, ex);
        }
        public static IpcResult Fail(Exception ex, string error)
        {
            return new IpcResult(false, error, ex);
        }

        public static IpcResult<T> Fail<T>(string error)
        {
            return new IpcResult<T>(false, default, error);
        }

        public static IpcResult<T> Fail<T>(Exception ex)
        {
            return new IpcResult<T>(false, default, exception: ex);
        }

        public static IpcResult<T> Fail<T>(Exception ex, string message)
        {
            return new IpcResult<T>(false, default, message, ex);
        }

        public static IpcResult Ok()
        {
            return new IpcResult(true);
        }

        public static IpcResult<T> Ok<T>(T value)
        {
            return new IpcResult<T>(true, value, null);
        }


        public IpcResult(bool isSuccess, string error = null, Exception exception = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Exception = exception;
        }

        public bool IsSuccess { get; private set; }

        public string Error { get; private set; }

        public Exception Exception { get; private set; }


    }

    public class IpcResult<T>
    {
        public IpcResult(bool isSuccess, T value, string error = null, Exception exception = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
            Exception = exception;
        }


        public bool IsSuccess { get; private set; }

        public string Error { get; private set; }

        public Exception Exception { get; private set; }

        public T Value { get; private set; }
    }
}
