using System;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.BookingServices
{
    /// <summary>
    ///     Represents a contract for service responsible for operations with bookings.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Adds new booking to the system.
        /// </summary>
        /// <param name="request">Booking request.</param>
        void AddBooking(AddBookingRequest request);

        /// <summary>
        /// Cancels the booking.
        /// </summary>
        /// <param name="id">Id of booking to be cancelled.</param>
        void CancelBooking(Guid id);
    }
}
