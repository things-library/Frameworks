namespace ThingsLibrary.Services.AzureFunctions.Functions.Base
{
    /// <summary>
    /// Wrapper for functions that require a logged in user / ClaimsPrincipal
    /// </summary>
    public abstract class BaseUserFunctions : BaseFunctions
    {
        ///// <summary>
        ///// Claims Principal Accessor 
        ///// </summary>        
        //public IClaimsPrincipalAccessor ClaimsPrincipalAccessor
        //{
        //    get
        //    {
        //        // not everything uses this so might as well just do it on first use
        //        if (_claimsPrincipalAccessor == null) { _claimsPrincipalAccessor = App.Service.Services.GetRequiredService<IClaimsPrincipalAccessor>(); }

        //        return _claimsPrincipalAccessor;
        //    }
        //}
        //private IClaimsPrincipalAccessor? _claimsPrincipalAccessor = null;

        ///// <summary>
        ///// Current logged in user that has been validated via middleware
        ///// </summary>
        //public ClaimsPrincipal ClaimsPrincipal => ClaimsPrincipalAccessor.Principal;

        /// <summary>
        /// Wrapper for azure functions that need logged in user details 
        /// </summary>        
        protected BaseUserFunctions() : base()
        {
            //nothing
        }
    }
}
