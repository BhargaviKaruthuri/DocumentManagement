using Microsoft.AspNetCore.Http;


namespace DocumentManagement.Data
{
    public class DataTableRequest
    {
        public string? Draw { get; set; }
        public string? Start { get; set; }
        public string? Length { get; set; }
        public string? SortColumn { get; set; }
        public string? SortColumnDirection { get; set; }
        public string? SearchValue { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int RecordsTotal { get; set; }
        public DataTableRequest(IFormCollection form)
        {
            Draw = form["draw"].FirstOrDefault();
            Start = form["start"].FirstOrDefault();
            Length = form["length"].FirstOrDefault();
            SortColumn = form["columns[" + form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            SortColumnDirection = form["order[0][dir]"].FirstOrDefault();
            SearchValue = form["search[value]"].FirstOrDefault();
            PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            Skip = Start != null ? Convert.ToInt32(Start) : 0;
            RecordsTotal = 0;
        }

        // only for test cases
        public DataTableRequest(string? draw, string? start, string? length, string? sortColumn, string? sortColumnDirection, string? searchValue, int pageSize, int skip, int recordsTotal)
        {
            Draw = draw;
            Start = start;
            Length = length;
            SortColumn = sortColumn;
            SortColumnDirection = sortColumnDirection;
            SearchValue = searchValue;
            PageSize = pageSize;
            Skip = skip;
            RecordsTotal = recordsTotal;
        }
    }
}
