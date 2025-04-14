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
        int count = 0;

        int halfColumns = columns / 2;
        int halfRows = rows / 2;

        for (int i = 0; i < playerCount; i++)
        {
            int xIndex = count % columns - halfColumns;
            int zIndex = count / columns - halfRows;

            Vector3 localOffset = new Vector3(xIndex * cellWidth, 0, zIndex * cellHeight);

            Debug.DrawRay(transform.position + startOffset + localOffset, Vector3.up, _gizmoColor);
            count++;
        }
    }
}
