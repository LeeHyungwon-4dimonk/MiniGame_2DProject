using TMPro;
using UnityEngine;

/// <summary>
/// �ΰ����� ������ �ǽð����� ǥ���ϴ� UI
/// </summary>
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
