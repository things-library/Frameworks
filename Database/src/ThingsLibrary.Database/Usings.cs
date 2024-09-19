// ================================================================================
// SYSTEM
// ================================================================================
global using System.Data.Common;
global using System.Reflection;

global using System.Text.Json;
global using System.Text.Json.Serialization;

global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using ILogger = Microsoft.Extensions.Logging.ILogger;

// ================================================================================
// THIRD PARTY
// ================================================================================
global using Serilog;

// ================================================================================
// LOCAL 
// ================================================================================
global using ThingsLibrary.Attributes;
global using ThingsLibrary.DataType;