using BrokeProtocol.API;
using BrokeProtocol.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using BrokeProtocol.Utility;
using System.Collections;
using System.CodeDom;
using BrokeProtocol.Collections;
using BrokeProtocol.Managers;

namespace IDProtocol
{
    /// <summary>
    /// Manages various events related to ID handling in the IDProtocol plugin.
    /// </summary>
    public class IDEvents : PlayerEvents
    {
        /// <summary>Temporary storage for DNI data during registration process.</summary>
        private Dictionary<string, DNIData> temporaryData = new Dictionary<string, DNIData>();
        /// <summary>Options for the sex dropdown in the registration form.</summary>
        private readonly List<string> sexDropdown = new List<string>() { "Male", "Female" };

        private readonly int idCardIndex = SceneManager.Instance.GetEntity("IDCard").index;

        /// <summary>
        /// Closes the DNI visual element and hides the cursor for the player.
        /// </summary>
        /// <param name="player">The player for whom the visual element should be closed.</param>
        /// <param name="element">The UI element involved in this action.</param>
        [CustomTarget]
        public void CloseDNI(ShPlayer player, string element)
        {
            player.svPlayer.VisualElementDisplay("DNI", false);
        }

        /// <summary>
        /// Initiates the NPC registration process for the player, displaying the DNI form.
        /// </summary>
        /// <param name="player">The player initiating the registration.</param>
        [CustomTarget]
        public void NPCRegister(ShEntity _, ShPlayer player)
        {
            if (Core.Instance.Configuration.AllowOverwride == false && player.svPlayer.CustomData.FetchCustomData<DNIData>("IDProtocol") != null)
            {
                player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.OverwriteNotAllowed);
                return;
            }

            player.svPlayer.VisualElementDisplay("DNI-FORM", true);
            player.svPlayer.SetTextElementText("LABEL-TITLE-ID-REGISTER", Core.Instance.Configuration.LabelRegisterMenuTitle);
            player.svPlayer.AddButtonClickedEvent("BUTTON-CLOSE-REGISTER", "NPCRegisterClose");
            player.svPlayer.AddButtonClickedEvent("BUTTON-SUBMIT-REGISTER", "NPCRegisterSubmit");
            player.svPlayer.SetDropdownFieldChoices("DROPDOWN-SEX-REGISTER", sexDropdown);
            player.svPlayer.CursorVisibility(true);
        }

        /// <summary>
        /// Handles closing the NPC registration form.
        /// </summary>
        /// <param name="player">The player closing the registration form.</param>
        /// <param name="element">The UI element involved in this action.</param>
        [CustomTarget]
        public void NPCRegisterClose(ShPlayer player, string element)
        {
            player.svPlayer.VisualElementDisplay("DNI-FORM", false);
            player.svPlayer.CursorVisibility(false);
        }

        /// <summary>
        /// Submits the NPC registration form data and initiates data verification.
        /// </summary>
        /// <param name="player">The player submitting the form.</param>
        /// <param name="element">The UI element involved in this action.</param>
        [CustomTarget]
        public void NPCRegisterSubmit(ShPlayer player, string element)
        {
            temporaryData[player.username] = new DNIData();
            foreach (var field in new string[]{ "FIELD-FN-REGISTER", "FIELD-LN-REGISTER", "FIELD-DOB-REGISTER", "FIELD-ADDRESS-REGISTER", "FIELD-HGT-REGISTER", "FIELD-WGT-REGISTER" }) player.svPlayer.GetTextFieldText(field, "NPCRegisterGetField");
            player.svPlayer.GetDropdownFieldValue("DROPDOWN-SEX-REGISTER", "NPCRegisterGetDropdown");
            player.svPlayer.VisualElementDisplay("DNI-FORM", false);
            player.svPlayer.StartCoroutine(CheckSubmit(player));
            player.svPlayer.CursorVisibility(false);
        }

        /// <summary>
        /// Waits for a short duration and then checks the validity of submitted DNI data.
        /// If valid, updates the player's custom data; otherwise, sends an error message.
        /// </summary>
        /// <param name="player">The player whose DNI data is being checked.</param>
        /// <returns>An IEnumerator needed for coroutine execution.</returns>
        private IEnumerator CheckSubmit(ShPlayer player)
        {
            yield return new WaitForSeconds(1f);

            if (temporaryData.TryGetValue(player.username, out DNIData dniData) && dniData.VerifyData())
            {
                player.svPlayer.CustomData.AddOrUpdate<DNIData>("IDProtocol", dniData);
                player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.SucessRegistration);
                player.TransferItem(DeltaInv.AddToMe, idCardIndex, 1);
            }
            else if (temporaryData.ContainsKey(player.username))
            {
                player.svPlayer.SendGameMessage(Core.Instance.Configuration.Messages.ErrorRegister);
            }

            temporaryData.Remove(player.username);
        }

        /// <summary>
        /// Handles the retrieval of text field data from the DNI registration form.
        /// Updates the corresponding field in the temporary data storage.
        /// </summary>
        /// <param name="player">The player providing the data.</param>
        /// <param name="element">The identifier for the text field in the form.</param>
        /// <param name="text">The text data retrieved from the form field.</param>
        [CustomTarget]
        public void NPCRegisterGetField(ShPlayer player, string element, string text)
        {
            switch (element)
            {
                case "FIELD-FN-REGISTER":
                    temporaryData[player.username].FirstName = text;
                    break;
                case "FIELD-LN-REGISTER":
                    temporaryData[player.username].LastName = text;
                    break;
                case "FIELD-DOB-REGISTER":
                    temporaryData[player.username].DateOfBirth = text;
                    break;
                case "FIELD-ADDRESS-REGISTER":
                    temporaryData[player.username].Address = text;
                    break;
                case "FIELD-HGT-REGISTER":
                    temporaryData[player.username].Height = int.Parse(text);
                    break;
                case "FIELD-WGT-REGISTER":
                    temporaryData[player.username].Weight = int.Parse(text);
                    break;
            }
        }

        /// <summary>
        /// Handles the selection from a dropdown field in the DNI registration form.
        /// Updates the corresponding field in the temporary data storage.
        /// </summary>
        /// <param name="player">The player making the selection.</param>
        /// <param name="element">The identifier for the dropdown field in the form.</param>
        /// <param name="index">The selected index in the dropdown field.</param>
        [CustomTarget]
        public void NPCRegisterGetDropdown(ShPlayer player, string element, int index)
        {
            switch (element)
            {
                case "DROPDOWN-SEX-REGISTER":
                    temporaryData[player.username].Sex = sexDropdown[index] == "Male";
                    break;
            }
        }

        /// <summary>
        /// Processes spawning events for entities, adding dynamic actions if necessary.
        /// </summary>
        /// <param name="entity">The entity being spawned.</param>
        /// <returns>True if the spawn process should continue, false otherwise.</returns>
        [Execution(ExecutionMode.Additive)]
        public override bool Spawn(ShEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.data) && entity.Player)
            {
                if (entity.data == "IDProtocolNPC") entity.Player.svPlayer.SvAddDynamicAction("NPCRegister", Core.Instance.Configuration.LabelNPC);
            }
            return true;
        }

    }
}
