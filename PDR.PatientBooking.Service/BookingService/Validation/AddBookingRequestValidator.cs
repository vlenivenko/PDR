using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingService.Validation
{
    ///<inheritdoc cref="IAddBookingRequestValidator"/>
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (ValidateRequest(request, ref result))
                return result;

            return result;
        }

        private bool ValidateRequest(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (request.StartTime < DateTime.UtcNow)
            {
                result.PassedValidation = false;
                result.Errors.Add("Appointment start time should be greater than current time");
                return true;
            }

            if (request.EndTime <= request.StartTime)
            {
                result.PassedValidation = false;
                result.Errors.Add("Appointment end time should be greater than start time");
                return true;
            }

            var patient = _context.Patient.SingleOrDefault(p => p.Id == request.PatientId);
            if (patient == null)
            {
                result.PassedValidation = false;
                result.Errors.Add("A patient with provided id was not found in the system");
                return true;
            }

            var doctor = _context.Doctor.SingleOrDefault(d => d.Id == request.DoctorId);
            if (doctor == null)
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor with provided id was not found in the system");
                return true;
            }

            var isDoctorBusy = doctor.Orders.Any(o => o.StartTime < request.EndTime && request.StartTime < o.EndTime);
            if (isDoctorBusy)
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor is busy during selected time range");
                return true;
            }

            return false;
        }
    }
}
