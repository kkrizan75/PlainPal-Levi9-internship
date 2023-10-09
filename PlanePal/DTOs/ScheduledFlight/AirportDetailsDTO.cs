using Newtonsoft.Json;

namespace PlanePal.DTOs.ScheduledFlight
{
    /// <summary>
    /// Represents detailed information about an airport.
    /// </summary>
    public class AirportDetailsDTO
    {
        /// <summary>
        /// Gets or sets the name of the airport.
        /// </summary>
        /// <example>
        /// "airport_name": "Belgrade Nikola Tesla Airport"
        /// </example>
        [JsonProperty("airport_name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time zone of the airport.
        /// </summary>
        /// <example>
        /// "timeZone": "UTC+1"
        /// </example>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the ICAO (International Civil Aviation Organization) code of the airport.
        /// </summary>
        /// <example>
        /// "icao_code": "LYBE"
        /// </example>
        [JsonProperty("icao_code")]
        public string Icao { get; set; }

        /// <summary>
        /// Gets or sets the name of the country where the airport is located.
        /// </summary>
        /// <example>
        /// "country_name": "Serbia"
        /// </example>
        [JsonProperty("country_name")]
        public string Country { get; set; }
    }
}