// ================================================================================
// <copyright file="BaseUserFunctions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Security.OAuth2.Services;

namespace ThingsLibrary.Services.AzureFunctions.Functions.Base
{
    /// <summary>
    /// Wrapper for functions that require a logged in user / ClaimsPrincipal
    /// </summary>
    public abstract class BaseUserFunctions : BaseFunctions
    {
        /// <summary>
        /// Claims Principal Accessor 
        /// </summary>        
        public IClaimsPrincipalAccessor ClaimsPrincipalAccessor { get; init; }
        
        /// <summary>
        /// Current logged in user that has been validated via middleware
        /// </summary>
        public ClaimsPrincipal ClaimsPrincipal => ClaimsPrincipalAccessor.Principal;

        /// <summary>
        /// Wrapper for azure functions that need logged in user details 
        /// </summary>        
        protected BaseUserFunctions(IClaimsPrincipalAccessor claimsPrincipalAccessor) : base()
        {
            this.ClaimsPrincipalAccessor = claimsPrincipalAccessor;            
        }
    }
}
