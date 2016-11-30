using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WaypointBehaviour : MonoBehaviour {

    // Use this for initialization
    public int index = 0;
    MeshRenderer mesh;
    Vector3 scale;
	void Start () {
        mesh = GetComponent<MeshRenderer>();
        Hide();
        index = int.Parse(this.gameObject.name);
	}
    public Vector3 GetPoint() {        
        return transform.position;
    }
    public void Hide()
    {
        scale = transform.localScale;
        transform.DOScale(0, 0.1f);
    }
    public void Show(Color c)
    {
        transform.DOScale(scale, 0.1f);
        mesh.material.SetColor("_Color", c);
    }

}
