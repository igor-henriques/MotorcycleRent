﻿global using MongoDB.Driver;
global using MotorcycleRent.Domain.Entities;
global using MotorcycleRent.Domain.Interfaces;
global using System.Linq.Expressions;
global using MotorcycleRent.Core.Shared;
global using MotorcycleRent.Application.Interfaces;
global using MotorcycleRent.Application.Services;
global using MotorcycleRent.Domain.Models;
global using System.Security.Claims;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Http;
global using MotorcycleRent.Application.Models.Options;
global using FluentValidation;
global using Azure.Storage.Blobs;
global using MotorcycleRent.Application.Models.Dtos;