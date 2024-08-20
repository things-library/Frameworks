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
