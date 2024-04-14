﻿global using MotorcycleRent.NotificationConsumer.Models.Enums;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Hosting;
global using MotorcycleRent.NotificationConsumer.Injectors;
global using MongoDB.Bson.Serialization.Attributes;
global using Microsoft.Extensions.DependencyInjection;
global using MotorcycleRent.NotificationConsumer.Models;
global using MotorcycleRent.NotificationConsumer.Workers;
global using MongoDB.Driver;
global using System.Linq.Expressions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MotorcycleRent.NotificationConsumer.Data;
global using Azure.Messaging.ServiceBus;
global using System.Text.Json;
global using Serilog;