using UnityEngine;

public class AutoTile : MonoBehaviour
{
    [SerializeField]
    private Renderer tileRenderer;

    private float xScale = 0f;
    private float yScale = 0f;

    void Start()
    {
        xScale = this.gameObject.transform.lossyScale.x;
        yScale = this.gameObject.transform.lossyScale.y;
    }

    /*
      private void OnDrawGizmos()
    {
        if (this.tileRenderer != null)
        {
            xScale = this.gameObject.transform.lossyScale.x;
            yScale = this.gameObject.transform.lossyScale.y;

            this.tileRenderer.material.mainTextureScale = new Vector2(xScale, yScale);
        }
    }
    */
    void Update()
    {
        this.tileRenderer.material.mainTextureScale = new Vector2(xScale, yScale);
    }
}