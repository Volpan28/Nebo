using System.Globalization;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Common.Models;
using CsvHelper;
using Microsoft.Extensions.Logging;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;

namespace AstroMonitor.Infrastructure.Services;

public class ReportGeneratorService : IReportGeneratorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ReportGeneratorService> _logger;
    
    private static readonly SemaphoreSlim _semaphore = new(2, 2); 

    public ReportGeneratorService(IHttpClientFactory httpClientFactory, ILogger<ReportGeneratorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateCsvAsync(IEnumerable<VisibilityCsvItemDto> items, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        await csvWriter.WriteRecordsAsync(items, cancellationToken);
        await streamWriter.FlushAsync();

        return memoryStream.ToArray();
    }

    public async Task<byte[]> GeneratePdfAsync(ReportMetadataDto metadata, IEnumerable<VisibilityPdfItemDto> items, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("ImageDownloader");
        var imageTasks = new Dictionary<string, Task<byte[]>>();

        var categoryOrder = new List<string> { "Planets", "Moons", "Deep Sky Objects", "Constellations", "Bright Stars" };
        
        var groupedItems = items
            .GroupBy(i => i.Category)
            .OrderBy(g => {
                var index = categoryOrder.IndexOf(g.Key);
                return index == -1 ? 99 : index; 
            })
            .ToList();

        foreach (var obj in items)
        {
            if (!string.IsNullOrEmpty(obj.ImageUrl) && !imageTasks.ContainsKey(obj.ImageUrl))
            {
                imageTasks[obj.ImageUrl] = DownloadImageThrottledAsync(httpClient, obj.ImageUrl, cancellationToken);
            }
        }
        
        await Task.WhenAll(imageTasks.Values);
        var imageCache = imageTasks.ToDictionary(k => k.Key, v => v.Value.Result);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Lato));

                page.Header().Column(col =>
                {
                    col.Item().Text("Night Sky Observation Report").SemiBold().FontSize(22).FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Location: Latitude {metadata.Latitude:F4}, Longitude {metadata.Longitude:F4}").FontSize(12);
                    col.Item().Text($"Observation Window: {metadata.StartTime:yyyy-MM-dd HH:mm} - {metadata.EndTime:HH:mm} UTC").FontSize(12);
                    col.Item().PaddingBottom(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().Column(col =>
                {
                    if (!items.Any())
                    {
                        col.Item().Text("No objects are visible during this time window.").FontSize(14).Italic();
                        return;
                    }

                    foreach (var group in groupedItems)
                    {
                        col.Item()
                            .PaddingTop(15)
                            .PaddingBottom(10)
                            .Background(Colors.Blue.Lighten4)
                            .Padding(5)
                            .Text(group.Key.ToUpper())
                            .SemiBold()
                            .FontSize(16)
                            .FontColor(Colors.Blue.Darken3);

                        foreach (var obj in group.OrderBy(o => o.Name))
                        {
                            col.Item().PaddingBottom(15).Row(row =>
                            {
                                row.ConstantItem(120).Height(120).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Element(e => 
                                {
                                    if (!string.IsNullOrEmpty(obj.ImageUrl) && imageCache.TryGetValue(obj.ImageUrl, out var imgBytes) && imgBytes != null)
                                    {
                                        try { e.Image(imgBytes).FitArea(); }
                                        catch { e.Text("Format Error").FontSize(10).FontColor(Colors.Red.Medium); }
                                    }
                                    else 
                                    {
                                        e.Text("No Photo").FontSize(10).FontColor(Colors.Grey.Medium);
                                    }
                                });

                                row.RelativeItem().PaddingLeft(15).Column(textCol =>
                                {
                                    textCol.Item().Text(obj.Name).SemiBold().FontSize(16).FontColor(Colors.Black);
                                    if(obj.Category != "Constellations")
                                        textCol.Item().Text($"Constellation: {obj.Constellation}").FontSize(10).FontColor(Colors.Grey.Medium);
                                        
                                    textCol.Item().Text($"Visible: {obj.VisibilityWindow}").SemiBold().FontSize(12).FontColor(Colors.Teal.Darken2);
                                    textCol.Item().PaddingTop(5).Text(obj.Description).FontSize(10).LineHeight(1.2f).FontColor(Colors.Grey.Darken3);
                                });
                            });
                            col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated by AstroMonitor | Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private async Task<byte[]> DownloadImageThrottledAsync(HttpClient client, string url, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try 
        { 
            await Task.Delay(400, cancellationToken);

            var response = await client.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to download image from {Url}. Status Code: {StatusCode}", url, response.StatusCode);
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync(cancellationToken); 
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Error while downloading image from {Url}", url);
            return null; 
        }
        finally
        {
            _semaphore.Release();
        }
    }
}