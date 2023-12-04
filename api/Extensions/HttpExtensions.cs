using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationsHeader(this HttpResponse response, PaginationHeader header)
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
                        
    }
}