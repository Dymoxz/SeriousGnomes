using UnityEngine;

public class Card : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    [Header("Instellingen")]
    public float fixedY = 0.5f; // De hoogte waarop het object moet blijven
    public float gridSize = 1.0f; // De grootte van je grid (bijv. 1.0 of 0.5)

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        // 1. Bereken de ruwe nieuwe positie
        Vector3 rawPosition = GetMouseWorldPos() + mOffset;

        // 2. Lock de Y-as
        float newY = fixedY;

        // 3. Snap de X en Z naar het grid
        // We delen door de gridsize, ronden af, en doen het weer keer de gridsize
        float newX = Mathf.Round(rawPosition.x / gridSize) * gridSize;
        float newZ = Mathf.Round(rawPosition.z / gridSize) * gridSize;

        // 4. Pas de positie toe
        transform.position = new Vector3(newX, newY, newZ);
    }
}
