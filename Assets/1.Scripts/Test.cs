using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Color _gizmoColor = Color.blue;

    public int playerCount = 13;
    public int columns = 4;          // 가로로 몇 칸씩
    public int rows = 4;             // 세로로 몇 칸씩
    public float cellWidth = 2f;
    public float cellHeight = 2f;
    public Vector3 startOffset = Vector3.zero;

    private void OnDrawGizmos()
    {
        int columns = 3;
        int totalRows = Mathf.CeilToInt(playerCount / (float)columns);
        for (int i = 0; i < playerCount; i++)
        {
            int row = i / columns;
            int indexInRow = i % columns;

            // X축 정렬 (가운데 기준)
            int itemsInThisRow = Mathf.Min(columns, playerCount - row * columns);
            float centerOffsetX = (itemsInThisRow - 1) / 2f;
            float x = (indexInRow - centerOffsetX) * cellWidth;

            // Y축 정렬 (가운데 기준)
            float centerOffsetY = (totalRows - 1) / 2f;
            float y = (row - centerOffsetY) * cellHeight;

            Debug.DrawRay(new Vector3(x, 0, y), Vector3.up);
        }
    }
}
