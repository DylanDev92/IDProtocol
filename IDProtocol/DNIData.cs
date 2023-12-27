using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading.Tasks;

namespace IDProtocol
{
    /// <summary>
    /// Represents the personal data for an ID.
    /// </summary>
    public class DNIData
    {
        /// <summary>Unique identifier for the individual.</summary>
        public string ID { get; set; }

        /// <summary>First name of the individual.</summary>
        public string FirstName { get; set; }

        /// <summary>Last name of the individual.</summary>
        public string LastName { get; set; }

        /// <summary>Date of birth of the individual in DD/MM/YYYY format.</summary>
        public string DateOfBirth { get; set; }

        /// <summary>Address of the individual.</summary>
        public string Address { get; set; }

        /// <summary>Sex of the individual. True for male, false for female.</summary>
        public bool Sex { get; set; }

        /// <summary>Height of the individual in centimeters.</summary>
        public int Height { get; set; }

        /// <summary>Weight of the individual in kilograms.</summary>
        public int Weight { get; set; }

        /// <summary>
        /// Validates the DNI data.
        /// </summary>
        /// <returns>
        /// True if the data is valid, meaning it meets all the specified criteria such as name format,
        /// date format, and valid height and weight. False otherwise.
        /// </returns>
        public bool VerifyData()
        {
            Regex validNameRegex = new Regex(@"^[a-zA-Z\s'-]+$");
            string datePattern = @"^\d{2}/\d{2}/\d{4}$";

            FirstName = FirstName.CapitalizeFirstLetter();
            LastName = LastName.CapitalizeFirstLetter();

            return !string.IsNullOrEmpty(FirstName) && validNameRegex.IsMatch(FirstName)
                && !string.IsNullOrEmpty(LastName) && validNameRegex.IsMatch(LastName)
                && !string.IsNullOrEmpty(DateOfBirth) && Regex.IsMatch(DateOfBirth, datePattern)
                && int.TryParse(Height.ToString(), out _)
                && int.TryParse(Weight.ToString(), out _);
        }
    }
}
