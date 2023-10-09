using Newtonsoft.Json;

namespace PlanePal.DTOs.ScheduledFlight
{
    /// <summary>
    /// Represents detailed information about an airline.
    /// </summary>
    public class AirlineDetailsDTO
    {
        /// <summary>
        /// Gets or sets the name of the airline.
        /// </summary>
        /// <example>
        /// "airline_name": "Serbian Airways"
        /// </example>
        [JsonProperty("airline_name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the IATA (International Air Transport Association) code of the airline.
        /// </summary>
        /// <example>
        /// "iata_code": "SAW"
        /// </example>
        [JsonProperty("iata_code")]
        public string Iata { get; set; }

        /// <summary>
        /// Gets or sets the name of the country where the airline is based.
        /// </summary>
        /// <example>
        /// "country_name": "Serbia"
        /// </example>
        [JsonProperty("country_name")]
        public string Country { get; set; }
    }
}