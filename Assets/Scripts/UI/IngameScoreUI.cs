using TMPro;
using UnityEngine;

public class IngameScoreUI : MonoBehaviour
{
    private int score;
    [SerializeField] TMP_Text text;
    private void Update()
    {
        score = GameManager.Instance.GetScore();
        text.text = $"���� : {score.ToString("000")}";
    }
}
