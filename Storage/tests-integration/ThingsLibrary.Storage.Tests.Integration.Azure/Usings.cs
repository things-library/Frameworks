// ================================================================================
// SYSTEM
// ================================================================================
global using System;
global using Microsoft.Extensions.Configuration;

global using System.Diagnostics.CodeAnalysis;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

// ================================================================================
// THIRD PARTY
// ================================================================================
global using DotNet.Testcontainers.Builders;
global using DotNet.Testcontainers.Containers;

// ================================================================================
// LOCAL
// ================================================================================
global using ThingsLibrary.Testing.Attributes;
global using ThingsLibrary.Testing.Containers;
global using ThingsLibrary.Testing.Extensions;

global using ThingsLibrary.Storage;
global using ThingsLibrary.Storage.Interfaces;

global using ThingsLibrary.Storage.Tests.Integration;
global using ThingsLibrary.Storage.Tests.Integration.Base;


global using Az = ThingsLibrary.Storage.Azure;
