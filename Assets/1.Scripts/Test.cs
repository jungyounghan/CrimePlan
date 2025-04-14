using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Color _gizmoColor = Color.blue;

    public int playerCount = 13;
    public int columns = 4;          // ���η� �� ĭ��
    public int rows = 4;             // ���η� �� ĭ��
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

            // X�� ���� (��� ����)
            int itemsInThisRow = Mathf.Min(columns, playerCount - row * columns);
            float centerOffsetX = (itemsInThisRow - 1) / 2f;
            float x = (indexInRow - centerOffsetX) * cellWidth;

            // Y�� ���� (��� ����)
            float centerOffsetY = (totalRows - 1) / 2f;
            float y = (row - centerOffsetY) * cellHeight;

            Debug.DrawRay(new Vector3(x, 0, y), Vector3.up);
        }
    }
}
