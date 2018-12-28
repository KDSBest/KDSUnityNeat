using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LineUiRender : MonoBehaviour
{
    public Vector2 PointA = new Vector3(0, 0, 0);
    public Vector2 PointB = new Vector3(60, 60, 0);
    public Color Color = UnityEngine.Color.green;

    public float LineWidth = 1;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 differenceVector = PointB - PointA;
        RectTransform imageRectTransform = (RectTransform)this.transform;
        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, LineWidth);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.anchorMax = new Vector2(0, 1);
        imageRectTransform.anchorMin = new Vector2(0, 1);
        imageRectTransform.anchoredPosition = PointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        this.GetComponent<Image>().color = Color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
