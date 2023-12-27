using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IDProtocol
{
    /// <summary>
    /// Configuration settings for the ID plugin.
    /// </summary>
    public class Configuration
    {
        /// <summary>Title displayed on the IDs.</summary>
        public string IDTitle { get; set; }

        /// <summary>Time in seconds after which the ID display will close automatically.</summary>
        public float CloseIDAfter { get; set; }

        /// <summary>Distance within which IDs can be shown, measured in game units.</summary>
        public float ShowDistance { get; set; }

        /// <summary>Label used for NPC in the game.</summary>
        public string LabelNPC { get; set; }

        /// <summary>Label used in the title of the register menu.</summary>
        public string LabelRegisterMenuTitle { get; set; }

        /// <summary> Allow the player to override the ID.</summary>
        public bool AllowOverwride { get; set; }

        /// <summary>Requires the object IDCard to use the commands.</summary>
        public bool RequireIDCard { get; set; }

        /// <summary>Commands available in the plugin.</summary>
        public PluginCommands Commands { get; set; }

        /// <summary>Messages used in the plugin.</summary>
        public PluginMessages Messages { get; set; }

        /// <summary>
        /// Defines the set of commands used by the plugin.
        /// </summary>
        public class PluginCommands
        {
            /// <summary>List of commands to issue IDs.</summary>
            public List<string> ID { get; set; }

            /// <summary>List of commands to show IDs.</summary>
            public List<string> ShowID { get; set; }

            /// <summary>List of commands to delete IDs.</summary>
            public List<string> DeleteID { get; set; }
        }

        /// <summary>
        /// Defines the set of messages used by the plugin.
        /// </summary>
        public class PluginMessages
        {
            /// <summary>Message displayed when there is an error in registration.</summary>
            public string ErrorRegister { get; set; }

            /// <summary>Message displayed upon successful registration.</summary>
            public string SucessRegistration { get; set; }

            /// <summary>Message displayed when an ID does not exist.</summary>
            public string IDDoesNotExist { get; set; }

            /// <summary>Message displayed when an ID is not valid.</summary>
            public string IDIsNotValid { get; set; }

            /// <summary>Message displayed when a player has already an ID.</summary>
            public string OverwriteNotAllowed { get; set; }

            /// <summary>Message displayed when an admin deletes an ID.</summary>
            public string DeletedID { get; set; }

            /// <summary>Message displayed when a player needs an ID card.</summary>
            public string NeedIDCard { get; set; }

            /// <summary>Message displayed when a player displays an ID.</summary>
            public string DisplayingID { get; set; }

            /// <summary>Message displayed when a player shows an ID.</summary>
            public string ShowID { get; set; }
        }
    }

}
