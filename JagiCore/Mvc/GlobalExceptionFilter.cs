using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace JagiCore.Mvc
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;

        public GlobalExceptionFilter(ILoggerFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            this._logger = factory.CreateLogger<GlobalExceptionFilter>();
        }
        
        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse 
            { 
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace 
            };
            
            context.Result = new ObjectResult(response)
            {
                StatusCode = GetHttpStatusCode(context.Exception),
                DeclaredType = typeof(ErrorResponse)
            };

            context.HttpContext.Response.ContentType = "application/json";

            this._logger.LogError(nameof(GlobalExceptionFilter), context.Exception);            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GlobalExceptionFilter() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private static int GetHttpStatusCode(Exception ex)
        {
            if (ex is HttpResponseException)
            {
                return (int)(ex as HttpResponseException).HttpStatusCode;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}