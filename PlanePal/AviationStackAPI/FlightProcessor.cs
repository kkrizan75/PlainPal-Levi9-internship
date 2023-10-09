using PlanePal.Model.Shared;

namespace PlanePal.AviationStackAPI
{
    public static class FlightProcessor
    {
        public static async Task<ServiceResponse<string>> LoadInformation(string dataType, Dictionary<string, string> optionalParams = null)
        {
            try
            {
                var result = await ApiUtil.ApiClient.GetStringAsync(ApiUtil.GetUrl(dataType, optionalParams));
                return new ServiceResponse<string>(result, true, $"{dataType} data fetched successfully from an external API. ");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message);
            }
        }
    }
}