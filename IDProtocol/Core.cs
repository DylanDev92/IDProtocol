using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrokeProtocol.API;
using UnityEngine;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using System.Configuration;
using Microsoft.SqlServer.Server;
using ENet;

namespace IDProtocol
{
    /// <summary>
    /// Core class for the IDProtocol plugin, handling the main functionalities and configurations.
    /// </summary>
    public class Core : Plugin
    {
        /// <summary>
        /// Singleton instance of the Core class.
        /// </summary>
        public static Core Instance { get; private set; }

        /// <summary>
        /// Manages ID-related commands.
        /// </summary>
        public IDCommands Commands { get; private set; }

        /// <summary>
        /// Handles ID-related events.
        /// </summary>
        public IDEvents Events { get; private set; }

        /// <summary>
        /// Configuration settings for the IDProtocol plugin.
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Core class.
        /// Sets up the plugin info, loads the configuration, and initializes commands and events.
        /// </summary>
        public Core()
        {
            Info = new PluginInfo("IDProtocol", "idp", "Be yourself, have an identity!");

            Instance = this;

            // Loads the config
            LoadConfig();

            Commands = new IDCommands();
            Events = new IDEvents();
        }

        /// <summary>
        /// Loads the configuration from a JSON file. If the file doesn't exist, it creates a default configuration.
        /// </summary>
        private void LoadConfig()
        {
            string path = Path.Combine("Plugins", "IDProtocol.json");

            Configuration = File.Exists(path) ? JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path)) : new Configuration()
            {
                IDTitle = "Example ID Title",
                CloseIDAfter = 0,
                ShowDistance = 30,
                LabelNPC = "Register DNI",
                AllowOverwride = false,
                RequireIDCard = true,
                LabelRegisterMenuTitle = "Register DNI Title",
                Commands = new Configuration.PluginCommands
                {
                    ShowID = new List<string> { "showid", "showdni"},
                    ID = new List<string> { "id", "dni" },
                    DeleteID = new List<string> { "deletedni", "deleteid" }
                },
                Messages = new Configuration.PluginMessages
                {
                    ErrorRegister = "Unsuccessfull registration, try again and use the correct format.",
                    SucessRegistration = "Sucessfull registration! Try your ID with /id",
                    IDDoesNotExist = "Your ID does not exist, try to register it!",
                    IDIsNotValid = "Your ID is not valid, try to contact the STAFF",
                    OverwriteNotAllowed = "You already have an ID, overwrite is not enabled",
                    DeletedID = "You have deleted the ID of the player",
                    NeedIDCard = "You need an ID Card to run this command",
                    DisplayingID = "You have displayed your DNI",
                    ShowID = "{0} is displaying his ID to you"
                }
            };

            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
                Debug.Log("A new config file has been generated for IDProtocol!");
            }

            Debug.Log("IDProtocol has been loaded correctly!");
        }
    }
}
