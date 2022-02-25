using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiConsume.Resources.Enums
{
    public enum ResponseStatus
    {
        /// <summary>
        /// The request has been executed successfuly and the data field contains related json object /for given a given request.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Departure date specified in getjourneys request is invalid (eg: a passed has been queried).
        /// </summary>
        InvalidDepartureDate = 2,

        /// <summary>
        /// Locations represented by origin-id or destination-id does not have an active bus route between them.
        /// </summary>
        InvalidRoute = 3,

        /// <summary>
        /// An invalid location id has been used as originid or destination-id parameters.
        /// </summary>
        InvalidLocation = 4,

        /// <summary>
        /// Operation has timed out.
        /// </summary>
        Timeout = 5,

    }
}
