namespace ThingsLibrary.Entity.Extensions
{
    /// <summary>
    /// Model Builder Extensions
    /// </summary>
    public static class EntityStoreFactoryExtensions
    {
        /// <summary>
        /// Finds all IDataSeeder classes and invokes the Seed() method for the provided assembly
        /// </summary>
        /// <param name="entityStores"></param>
        /// <param name="serviceProvider">If provided class instances can leverage dependency injection (DI)</param>
        /// <param name="assembly">Assembly to search for IDataSeeder classes</param>        
        /// <param name="seedTestData">If test data should also be seeded.</param>        
        public static void SeedDatabase(this IEntityStoreFactory entityStores, IServiceProvider? serviceProvider, Assembly assembly, bool seedTestData)
        {
            // ======================================================================
            // Database Check
            // ======================================================================
            //TODO: Log.Information("Establishing Database Connection...");

            // ======================================================================
            // DATA SEEDING 
            // ======================================================================
            var activity = new Activity("DataSeeding").Start();
   
            // add / ensure the base data for the service                
            entityStores.SeedBaseDataFromAssembly(serviceProvider, assembly);

            // add/ensure the test data for the service 
            if (seedTestData)
            {
                entityStores.SeedTestDataFromAssembly(serviceProvider, assembly);
            }

            activity.Stop();
        }

        /// <summary>
        /// Finds all IDataSeeder classes and invokes the Seed() method for the provided assembly
        /// </summary>
        /// <param name="entityStores"><see cref="Interfaces.IEntityStoreFactory"/></param>
        /// <param name="assembly">Assembly to search</param>        
        private static void SeedBaseDataFromAssembly(this IEntityStoreFactory entityStores, IServiceProvider? serviceProvider, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(Interfaces.IDataSeeder)));
            
            if (types.Any())
            {
                //TODO: Log.Information("= Seeding Base Data From Assembly {AssemblyName} ({AssemblyVersion})...", assembly.GetName().Name, assembly.GetName().Version);

                var parameters = new object[] { entityStores };

                // add/ensure the base data for the service
                foreach (var type in types)
                {
                    //TODO: Log.Information("== Seeding Data: {SeedLibrary}", type.FullName);

                    object? classInstance;
                    if (serviceProvider != null)
                    {
                        classInstance = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
                    }
                    else
                    {
                        classInstance = Activator.CreateInstance(type) ?? throw new ArgumentException($"Unable to create instance of '{type.FullName}'");
                    }

                    //do seeding
                    var methodInfo = type.GetMethod("Seed");
                    _ = methodInfo!.Invoke(classInstance, parameters);
                }
            }            
        }

        /// <summary>
        /// Finds all ITestDataSeeder classes and invokes the seed method for the provided assembly.
        /// </summary>
        /// <param name="entityStores"><see cref="Interfaces.IEntityStoreFactory"/></param>
        /// <param name="assembly">Assembly to search</param>
        /// <param name="currentUser">Current User</param>
        /// <param name="traceId">Unique Trace ID</param>
        private static void SeedTestDataFromAssembly(this IEntityStoreFactory entityStores, IServiceProvider? serviceProvider, Assembly assembly)
        {
            // add / ensure the base data for the service
            var types = assembly.GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(Interfaces.ITestDataSeeder)));
            if(types.Any())
            {
                //TODO: Log.Information("= Seeding Test Data From Assembly {AssemblyName} ({AssemblyVersion})...", assembly.GetName().Name, assembly.GetName().Version);

                var parameters = new object[] { entityStores };
                
                Activity.Current = new Activity("DataSeeding");

                foreach (var type in types)
                {
                    //TODO: Log.Information("= Seeding Test Data: {SeedLibrary}", type.FullName);

                    object? classInstance;
                    if (serviceProvider != null)
                    {
                        classInstance = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
                    }
                    else
                    {
                        classInstance = Activator.CreateInstance(type) ?? throw new ArgumentException($"Unable to create instance of '{type.FullName}'");
                    }

                    //do seeding
                    var methodInfo = type.GetMethod("Seed");
                    _ = methodInfo!.Invoke(classInstance, parameters);
                }
            }           
        }

        /// <summary>
        /// Return a claims principal for seeding base / demo data
        /// </summary>
        /// <returns></returns>
        public static ClaimsPrincipal GetServiceUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim("oid", "00000000-0000-0000-0000-100000000001"),   //clearly a human created id
                    new Claim("upn", "service.user"),
                    new Claim("preferred_username", "service.user"),
                    new Claim("given_name", "Service"),
                    new Claim("family_name", "User"),
                    new Claim("name", "Service User")
                },
                null,
                "name",
                "roles")
            );
        }
    }    
}
