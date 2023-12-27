using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDProtocol
{
    /// <summary>
    /// The Utils class provides utility functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Capitalizes the first letter of a string and ensures the rest of the string is in lowercase.
        /// </summary>
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        /// <summary>
        /// Tests if the player has the ID Card in hand, and returns a bool
        /// </summary>
        public static bool HasIDCard(this ShPlayer player)
        {
            if (Core.Instance.Configuration.RequireIDCard == true && !player.ActiveWeapon.name.StartsWith("IDCard"))
            {
                player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.NeedIDCard);
                return false;
            }
            return true;
        }
    }
}
