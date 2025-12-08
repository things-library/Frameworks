// ================================================================================
// <copyright file="Usings.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

global using System;
global using System.Linq;
global using System.Data.Common;
global using System.ComponentModel.DataAnnotations;
global using System.Collections.Generic;
global using System.Threading;

global using System.Text.Json;
global using System.Text.Json.Serialization;

global using System.Diagnostics.CodeAnalysis;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

// CURRENT LIBRARY
global using ThingsLibrary.DataType.Extensions;


[assembly: Parallelize(Workers = 5, Scope = ExecutionScope.MethodLevel)] 