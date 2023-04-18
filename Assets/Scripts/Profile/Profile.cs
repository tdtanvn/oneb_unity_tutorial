using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using TMPro;
using UnityEngine;

public class Profile : MonoBehaviour
{
    private PlayerProfile data;
    public TextMeshProUGUI InfoText;

    public void LoadProfile(PlayerProfile playerProfile)
    {
        this.data = playerProfile;
        Refresh();
    }
    void Refresh()
    {
        InfoText.text = "Info\n" +
        $"PlayerId: {this.data.PlayerId}\n" +
        $"Player name: {this.data.PlayerName}\n" +
        $"Country: {this.data.Country}\n" +
        $"GameId: {this.data.GameId}\n" +
        $"last Login: {this.data.LastLogin}\n" +
        $"Created: {this.data.Created}\n" +
        $"Ban: {this.data.Ban}\n";
    }
}
