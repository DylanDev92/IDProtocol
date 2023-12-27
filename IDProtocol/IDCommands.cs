using BrokeProtocol.API;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDProtocol
{
    /// <summary>
    /// Handles the ID commands in the game.
    /// </summary>
    public class IDCommands
    {
        /// <summary>
        /// Initializes a new instance of the IDCommands class.
        /// </summary>
        public IDCommands()
        {
            // Register all the commands
            CommandHandler.RegisterCommand(Core.Instance.Configuration.Commands.ID, new Action<ShPlayer>(SelfDNI));
            CommandHandler.RegisterCommand(Core.Instance.Configuration.Commands.ShowID, new Action<ShPlayer>(ShowDNI));
            CommandHandler.RegisterCommand(Core.Instance.Configuration.Commands.DeleteID, new Action<ShPlayer, string>(DeleteID));
        }

        /// <summary>
        /// Deletes the ID associated with a specified player.
        /// </summary>
        /// <param name="player">The player who is initiating the ID deletion request.</param>
        /// <param name="username">The username or ID of the player whose ID is to be deleted.</param>
        public void DeleteID(ShPlayer player, string username)
        {
            if (EntityCollections.TryGetPlayerByNameOrID(username, out ShPlayer playerDelete))
            {
                playerDelete.svPlayer.CustomData.TryRemoveCustomData("IDProtocol");
                player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.DeletedID);
            }
        }

        /// <summary>
        /// Handles the command to display the player's own DNI.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        public void SelfDNI(ShPlayer player)
        {
            if (!player.HasIDCard()) return;
            HandleDNI(player, true);
            player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.DisplayingID);
        }

        /// <summary>
        /// Handles the command to show the DNI to others.
        /// </summary>
        /// <param name="player">The player executing the command.</param>
        public void ShowDNI(ShPlayer player)
        {
            if (!player.HasIDCard()) return;
            HandleDNI(player, false);
            SelfDNI(player);
        }

        /// <summary>
        /// Handles the logic for displaying or showing the DNI.
        /// </summary>
        /// <param name="player">The player whose DNI is to be handled.</param>
        /// <param name="self">Boolean indicating whether to show the DNI to the player (true) or to others (false).</param>
        public void HandleDNI(ShPlayer player, bool self)
        {
            DNIData fetchData = player.svPlayer.CustomData.FetchCustomData<DNIData>("IDProtocol");
            if (fetchData == null || !fetchData.VerifyData())
            {
                if (self) player.svPlayer.SendGameMessage(fetchData == null ? Core.Instance.Configuration.Messages.IDDoesNotExist : Core.Instance.Configuration.Messages.IDIsNotValid);
                return;
            }

            fetchData.ID = player.ID.ToString();

            foreach (ShPlayer pShow in self ? new[] { player } : player.svPlayer.GetLocalInRange<ShPlayer>(Core.Instance.Configuration.ShowDistance).ToArray())
            {
                pShow.svPlayer.VisualElementDisplay("DNI", true);

                // Event for closing the ID, name of the event: CloseDNI
                pShow.svPlayer.AddButtonClickedEvent("CLOSE-DNI", "CloseDNI");

                // Closes the menu after x seconds
                pShow.svPlayer.StartCoroutine(CloseMenuAfter(player));

                // Updates the menu after opening it
                UpdateShownDNIData(pShow.svPlayer, fetchData);

                if (pShow != player) pShow.svPlayer.SendGameMessage(string.Format(Core.Instance.Configuration.Messages.ShowID, player.username));
            }
        }

        /// <summary>
        /// Updates the displayed DNI data on the menu.
        /// </summary>
        /// <param name="svPlayer">The server player instance to update the DNI data for.</param>
        /// <param name="data">The DNIData to display.</param>
        private void UpdateShownDNIData(SvPlayer svPlayer, DNIData data)
        {
            svPlayer.SetTextElementText("LABEL-TITLE", Core.Instance.Configuration.IDTitle);
            svPlayer.SetTextElementText("ID", data.ID);
            svPlayer.SetTextElementText("LN", data.LastName);
            svPlayer.SetTextElementText("FN", data.FirstName);
            svPlayer.SetTextElementText("DOB", data.DateOfBirth);
            svPlayer.SetTextElementText("ADDRESS", data.Address);
            svPlayer.SetTextElementText("SEX", data.Sex ? "M" : "F");
            svPlayer.SetTextElementText("HGT", data.Height.ToString());
            svPlayer.SetTextElementText("WGT", data.Weight.ToString());
        }

        /// <summary>
        /// Coroutine to close the DNI menu after a specified amount of time.
        /// </summary>
        /// <param name="player">The player for whom the menu will be closed.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        private IEnumerator CloseMenuAfter(ShPlayer player)
        {
            if (Core.Instance.Configuration.CloseIDAfter != 0)
            {
                yield return new WaitForSeconds(Core.Instance.Configuration.CloseIDAfter);

                player.svPlayer.VisualElementDisplay("DNI", false);
            }

            yield return null;
        }
    }
}
