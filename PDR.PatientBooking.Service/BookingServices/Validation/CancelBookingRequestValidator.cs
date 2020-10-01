using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    ///<inheritdoc cref="IAddBookingRequestValidator"/>
    public class CancelBookingRequestValidator : ICancelBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public CancelBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(Guid id)
        {
            var result = new PdrValidationResult(true);

            if (ValidateRequest(id, ref result))
                return result;

            return result;
        }

        private bool ValidateRequest(Guid id, ref PdrValidationResult result)
        {
            var order = _context.Order.SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                result.PassedValidation = false;
                result.Errors.Add("A booking with provided id was not found in the system");
                return true;
            }

            if (!order.IsActive)
            {
                result.PassedValidation = false;
                result.Errors.Add("A booking with provided id was already cancelled");
                return true;
            }

            return false;
        }
    }
}
