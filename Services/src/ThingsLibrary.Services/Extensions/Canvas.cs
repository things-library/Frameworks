namespace ThingsLibrary.Services.Extensions
{
    /// <summary>
    /// Service Canvas Extensions
    /// </summary>
    public static class CanvasExtensions
    {
        /// <summary>
        /// Get the service canvas from IConfiguration, register it, auth and authJwt as a singletons.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="HostBuilderContext"/> for chaining</returns>
        /// <exception cref="ArgumentException"></exception>
        public static HostBuilderContext AddServiceCanvas(this HostBuilderContext context, IServiceCollection services)
        {
            // see if this has already been called
            if (App.Service.Canvas != null) { return context; }
            Log.Debug("+ Service Canvas from configuration...");
            
            var canvasSection = context.Configuration.GetSection("ServiceCanvas");
            if (!canvasSection.Exists()) { throw new ArgumentException("Missing 'ServiceCanvas' section in configuration"); }

            var canvas = canvasSection.Get<ItemDto>();
            if (canvas == null) { throw new ArgumentException("Unable to deserialize 'ServiceCanvas' section in appSettings / Configuration"); }

            // make sure we don't have a copy and paste problem or something
            context.VerifyCanvas(canvas);

            // update fields as tie the class
            App.Service.Init(canvas);
            
            // allow chaining
            return context;
        }

        /// <summary>
        /// Initialize the AppService with various canvas related fields
        /// </summary>
        /// <param name="appService"></param>
        /// <param name="canvas"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Init(this AppService appService, ItemDto canvas)
        {
            if(appService.Canvas != null) { throw new ArgumentException("AppService.Canvas already initialized.");  }

            // set the canvas environment name
            var hostUri = canvas.Get<Uri?>("host", null);
            if (hostUri != null)
            {
                appService.HostUri = (hostUri.AbsolutePath.EndsWith('/') ? hostUri : new Uri($"{hostUri.OriginalString}/"));  // the host uri MUST be a path where it ends with / otherwise the last folder is considered a file and appending relative paths won't append correctly.
                Log.Information("+ Host Uri: {HostUri}", appService.HostUri);
            }
            appService.EnvironmentName = canvas["environment"];
            Log.Information("+ Environment: {EnvironmentName}", appService.EnvironmentName);

            appService.Canvas = canvas;
        }


        /// <summary>
        /// Check the service canvas against known items such as environment name
        /// </summary>
        /// <param name="context">Host Builder Context</param>
        /// <param name="canvas">Service Canvas</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static HostBuilderContext VerifyCanvas(this HostBuilderContext context, ItemDto canvas)
        {
            if (string.Compare(context.HostingEnvironment.EnvironmentName, canvas["Environment"], false) != 0)
            {
                throw new ArgumentException("Canvas 'Environment' does not match the hosting environment.");
            }

            //TODO: add other checks to make sure we aren't loading the wrong one

            return context;
        }
    }
}
