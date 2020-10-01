﻿using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.PatientServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace PDR.PatientBooking.Service.PatientServices.Validation
{
    public class AddPatientRequestValidator : IAddPatientRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddPatientRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddPatientRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (InvalidFields(request, ref result))
                return result;

            if (PatientAlreadyInDb(request, ref result))
                return result;

            if (ClinicNotFound(request, ref result))
                return result;

            return result;
        }

        private bool MissingRequiredFields(AddPatientRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.FirstName))
                errors.Add("FirstName must be populated");

            if (string.IsNullOrEmpty(request.LastName))
                errors.Add("LastName must be populated");

            if (string.IsNullOrEmpty(request.Email))
                errors.Add("Email must be populated");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        public bool InvalidFields(AddPatientRequest request, ref PdrValidationResult result)
        {
            try
            {
                var mail = new MailAddress(request.Email);
                return false;
            }
            catch (FormatException)
            {
                result.PassedValidation = false;
                result.Errors.Add("Email must be a valid email address");
                return true;
            }
        }

        private bool PatientAlreadyInDb(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (_context.Patient.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A patient with that email address already exists");
                return true;
            }

            return false;
        }

        private bool ClinicNotFound(AddPatientRequest request, ref PdrValidationResult result)
        {
            if (!_context.Clinic.Any(x => x.Id == request.ClinicId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A clinic with that ID could not be found");
                return true;
            }

            return false;
        }
    }
}
