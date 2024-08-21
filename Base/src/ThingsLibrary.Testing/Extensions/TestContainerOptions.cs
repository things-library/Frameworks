using DotNet.Testcontainers.Builders;
using ThingsLibrary.Testing.Containers;

namespace ThingsLibrary.Testing.Extensions
{
    public static class TestContainerOptionsExtensions
    {
        /// <summary>
        /// Create a container builder object based on the options object
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ContainerBuilder GetContainerBuilder(this TestContainerOptions options)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(options.Image);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(options.Name);

            if (!Regex.IsMatch(options.Name, "^[a-z0-9_.-]*$", RegexOptions.Singleline | RegexOptions.CultureInvariant))
            {
                throw new ArgumentException("Invalid container name.. must be all lower-case, must start or end with a letter or number, and can contain only letters, numbers, and the dash(-) character.");
            }
            if (!options.Ports.Any()) { throw new ArgumentException("No Ports defined."); }

            var builder = new ContainerBuilder()
                    .WithImage(options.Image)
                    .WithName(options.Name)
                    .WithEnvironment(options.Environment);

            var waitStrategy = Wait.ForUnixContainer();

            // bind the ports
            if (options.Ports != null)
            {
                foreach (var port in options.Ports)
                {
                    var pair = port.Split(':');
                    if (pair.Length == 1)
                    {
                        var containerPort = int.Parse(pair[0]);
                        builder = builder.WithExposedPort(containerPort).WithPortBinding(containerPort, true);

                        waitStrategy = waitStrategy.UntilPortIsAvailable(containerPort);
                    }
                    else if (pair.Length == 2)
                    {
                        var exposedPort = int.Parse(pair[0]);
                        var containerPort = int.Parse(pair[1]);

                        // make no sense that this assignment is needed 
                        builder = builder.WithExposedPort(containerPort).WithPortBinding(exposedPort, containerPort);   //outside:inside

                        waitStrategy = waitStrategy.UntilPortIsAvailable(containerPort);
                    }
                    else
                    {
                        throw new ArgumentException("Port definitions must be {Exposed Port}:{ContainerPort}, or just {ContainerPort}, IE: 8001:8001 syntax.");
                    }
                }
            }

            builder = builder.WithWaitStrategy(waitStrategy);

            return builder;
        }
    }
}
