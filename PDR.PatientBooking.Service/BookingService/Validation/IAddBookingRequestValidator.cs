using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingService.Validation
{
    /// <summary>
    ///     Represents a contract for booking request validator.
    /// </summary>
    public interface IAddBookingRequestValidator
    {
        /// <summary>
        /// Validates booking request.
        /// </summary>
        /// <param name="request">Booking request.</param>
        /// <returns>Validation result.</returns>
        PdrValidationResult ValidateRequest(AddBookingRequest request);
    }
}
