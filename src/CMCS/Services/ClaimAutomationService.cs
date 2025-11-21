using System;
using CMCS.Models;

namespace CMCS.Services
{
    public class ClaimAutomationService
    {
        private const double MaxHoursPerClaim = 220;
        private const double MaxHourlyRate = 2000;
        private const double MinHoursPerClaim = 1;
        private const double MinHourlyRate = 50;
        private const double MaxTotalAmount = 200_000;

        public ClaimCalculationResult Calculate(double hoursWorked, double hourlyRate)
        {
            var result = new ClaimCalculationResult
            {
                HoursWorked = Math.Round(hoursWorked, 2),
                HourlyRate = Math.Round(hourlyRate, 2)
            };

            if (double.IsNaN(hoursWorked) || double.IsInfinity(hoursWorked))
            {
                result.Errors.Add("Hours worked must be a valid number.");
                return result;
            }

            if (double.IsNaN(hourlyRate) || double.IsInfinity(hourlyRate))
            {
                result.Errors.Add("Hourly rate must be a valid number.");
                return result;
            }

            if (hoursWorked < MinHoursPerClaim)
            {
                result.Errors.Add($"Hours worked must be at least {MinHoursPerClaim} hour.");
            }

            if (hoursWorked > MaxHoursPerClaim)
            {
                result.Errors.Add($"Hours worked cannot exceed {MaxHoursPerClaim} hours per claim.");
            }
            else if (hoursWorked > 160)
            {
                result.Warnings.Add("The captured hours exceed a standard teaching load. Ensure justification is provided in the notes field.");
            }

            if (hourlyRate < MinHourlyRate)
            {
                result.Errors.Add($"Hourly rate must be at least {MinHourlyRate:C0}.");
            }

            if (hourlyRate > MaxHourlyRate)
            {
                result.Errors.Add($"Hourly rate cannot exceed {MaxHourlyRate:C0}.");
            }
            else if (hourlyRate > 1200)
            {
                result.Warnings.Add("The hourly rate is higher than the recommended threshold. Ensure that the HR-approved rate is used.");
            }

            var total = Math.Round(hoursWorked * hourlyRate, 2);
            if (total > MaxTotalAmount)
            {
                result.Errors.Add($"A single claim may not exceed {MaxTotalAmount:C0}. Please split the claim into smaller submissions.");
            }
            else if (total > 75_000)
            {
                result.Warnings.Add("Large claims may trigger manual audits. Consider submitting weekly instead of monthly if possible.");
            }

            result.TotalAmount = total;
            return result;
        }
    }
}
