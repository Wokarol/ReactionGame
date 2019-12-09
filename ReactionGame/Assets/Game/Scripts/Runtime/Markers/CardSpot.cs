using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpot : MonoBehaviour
{

    [SerializeField] private float scale = 1;
    [SerializeField] private float positionRange = 0.2f;
    [SerializeField] private float angle = 5;

    public float Scale => scale;
    public float PositionRange => positionRange;
    public Vector3 Position => transform.position;

    public (Vector3 pos, Quaternion rot, float scale) GetRandom()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        return (randomDirection * PositionRange + Position, Quaternion.Euler(0, 0, Random.Range(-angle, angle)), Scale);
    }

    #region GIZMOS STUFF
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1.48f, 0.1f) * (4 * scale));
        Gizmos.DrawWireSphere(transform.position, positionRange);
        float step = 1;
        for (float a = -angle; a < angle; a += step) {
            Gizmos.DrawLine(
                transform.position + Quaternion.Euler(0, 0, a) * new Vector2(0, 2),
                transform.position + Quaternion.Euler(0, 0, Mathf.Clamp(a + step, -angle, angle)) * new Vector2(0, 2));
        }
        Gizmos.DrawLine(transform.position + Quaternion.Euler(0, 0, -angle) * new Vector2(0, 2), transform.position);
        Gizmos.DrawLine(transform.position + Quaternion.Euler(0, 0, +angle) * new Vector2(0, 2), transform.position);
    } 
    #endregion
}
