using Identity.API.DTOs;

using PulseDelivery.Shared.DTOs;

public class PagedResponseDto<T> : ResponseDto<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }

    public static PagedResponseDto<T> Success(T data, int statusCode, int pageNumber, int pageSize, int totalRecords)
    {
        var totalPages = ((double)totalRecords / (double)pageSize);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        return new PagedResponseDto<T>
        {
            Data = data,
            StatusCode = statusCode,
            IsSuccessful = true,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = roundedTotalPages
        };
    }
}