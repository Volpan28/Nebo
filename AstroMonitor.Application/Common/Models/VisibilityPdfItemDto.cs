namespace AstroMonitor.Application.Common.Models;

public record VisibilityPdfItemDto(
    string Name, 
    string Category, 
    string Constellation, 
    string VisibilityWindow, 
    string Description, 
    string ImageUrl);