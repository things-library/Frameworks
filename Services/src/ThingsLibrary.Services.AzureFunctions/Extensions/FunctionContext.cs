// ================================================================================
// <copyright file="FunctionContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.AzureFunctions.Extensions
{
    public static class FunctionContextExtensions
    {
        public static bool IsHttpTriggerFunction(this FunctionContext context)
        {
            return context.FunctionDefinition.InputBindings.Values.First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";                        
        }
        
    }
}
