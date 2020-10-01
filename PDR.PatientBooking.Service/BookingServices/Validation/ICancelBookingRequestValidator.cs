using System;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    /// <summary>
    ///     Represents a contract for cancel booking request validator.
    /// </summary>
    public interface ICancelBookingRequestValidator
    {
        /// <summary>
        /// Validates cancel booking request.
        /// </summary>
        /// <param name="id">Id of booking to be cancelled.</param>
        /// <returns>Validation result.</returns>
        PdrValidationResult ValidateRequest(Guid id);
    }
}
