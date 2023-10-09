namespace PlanePal.DTOs.APIResponse
{
    public class APIResponseDTO<T>
    {
        public List<T> Data { get; set; }
        public PaginationDTO Pagination { get; set; }
    }
}