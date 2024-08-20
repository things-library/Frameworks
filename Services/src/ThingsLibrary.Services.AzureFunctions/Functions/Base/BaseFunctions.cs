using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ThingsLibrary.Services.AzureFunctions.Functions.Base
{
    /// <summary>
    /// Wrapper for functions that require a logged in user / ClaimsPrincipal
    /// </summary>
    public abstract class BaseFunctions
    {
        //private ILoggerFactory? _loggerFactory = null;
        //private ILogger? _logger = null;

        ///// <summary>
        ///// Context Logger
        ///// </summary>
        //public ILogger Logger
        //{
        //    get
        //    {
        //        // not everything uses this so might as well just do it on first use
        //        if (_logger == null)
        //        {
        //            if (_loggerFactory == null) { _loggerFactory = App.Service.Services.GetRequiredService<ILoggerFactory>(); }                    
        //            _logger = _loggerFactory.CreateLogger(this.GetType());
        //        }

        //        return _logger;
        //    }
        //}


        /// <summary>
        /// Wrapper for azure functions that need logged in user details 
        /// </summary>        
        protected BaseFunctions()
        {

        }
    }
}
