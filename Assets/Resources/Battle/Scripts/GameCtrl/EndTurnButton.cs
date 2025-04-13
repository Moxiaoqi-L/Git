using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public Button endTurnButton;

    private void Start()
    {
        endTurnButton.onClick.AddListener(TurnManager.Instance.EndPlayerTurn);
    }
}