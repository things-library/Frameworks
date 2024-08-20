global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Reflection;
global using Microsoft.Extensions.Configuration;

global using System.Diagnostics.CodeAnalysis;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

global using Starlight.Testing.Containers;
global using Starlight.Testing.Attributes;

global using Starlight.Cloud.File;
global using Starlight.Cloud.File.Tests.Integration.Base;

global using DotNet.Testcontainers.Builders;
global using DotNet.Testcontainers.Containers;

global using Gp = Starlight.Cloud.File.Gcp;