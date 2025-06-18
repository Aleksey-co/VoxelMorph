using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PortalWrapRenderer : MonoBehaviour
{
    public Material wrappedMaterial;
    public Renderer maskRenderer;

    private Vector3 maskMin;
    private Vector3 maskMax;
    private Vector3 maskSize;

    private Vector3 previousPosition;
    private bool shouldDrawWrapped;

    void Start()
    {
        previousPosition = transform.position;
        maskRenderer = GameObject.Find("Mask").GetComponent<MeshRenderer>();
        wrappedMaterial = GameObject.Find("Main Camera").GetComponent<OBJConverter>().materialP1;
    }

    void Update()
    {
        // Обновляем минимальные и максимальные границы маски
        var bounds = maskRenderer.bounds;
        maskMin = bounds.min;
        maskMax = bounds.max;
        maskSize = maskMax - maskMin;

        wrappedMaterial.SetVector("_MaskMin", new Vector4(maskMin.x, maskMin.y, maskMin.z, 0));
        wrappedMaterial.SetVector("_MaskMax", new Vector4(maskMax.x, maskMax.y, maskMax.z, 0));

        // Проверяем, если объект выходил за пределы маски
        Vector3 offset = Vector3.zero;
        if (transform.position.x < maskMin.x)
            offset = new Vector3(maskSize.x, 0, 0);
        else if (transform.position.x > maskMax.x)
            offset = new Vector3(-maskSize.x, 0, 0);
        else if (transform.position.y < maskMin.y)
            offset = new Vector3(0, maskSize.y, 0);
        else if (transform.position.y > maskMax.y)
            offset = new Vector3(0, -maskSize.y, 0);
        else if (transform.position.z < maskMin.z)
            offset = new Vector3(0, 0, maskSize.z);
        else if (transform.position.z > maskMax.z)
            offset = new Vector3(0, 0, -maskSize.z);

        // Флаг для отрисовки второй копии
        shouldDrawWrapped = offset != Vector3.zero;

        shouldDrawWrapped = true;
            OnRenderObject1();
        
    }

    void OnRenderObject1()
    {
        var mf = GetComponent<MeshFilter>();
        var mesh = mf.sharedMesh;
        var matrix = transform.localToWorldMatrix;

        // Рисуем основной объект
        Graphics.DrawMesh(mesh, matrix, wrappedMaterial, gameObject.layer);

        // Если нужно, рисуем сдвинутую копию
        if (shouldDrawWrapped)
        {
            Vector3 offset = Vector3.zero;

            // Проверяем выход за пределы маски
            if (transform.position.x < maskMin.x)
                offset = new Vector3(maskSize.x, 0, 0); 
            else if (transform.position.x > (maskMin.x+1))
                offset = new Vector3(-maskSize.x, 0, 0);
            else if (transform.position.y < maskMin.y)
                offset = new Vector3(0, maskSize.y, 0);
            else if (transform.position.y > (maskMin.y+1))
                offset = new Vector3(0, -maskSize.y, 0);
            else if (transform.position.z < maskMin.z)
                offset = new Vector3(0, 0, maskSize.z);
            else if (transform.position.z > (maskMin.z+1))
                offset = new Vector3(0, 0, -maskSize.z);

            // Если смещение есть, рисуем вторую копию
            if (offset != Vector3.zero)
            {
                var wrappedMatrix = Matrix4x4.TRS(
                    transform.position + offset,
                    transform.rotation,
                    transform.localScale
                );

                // Рисуем сдвинутую копию
                Graphics.DrawMesh(mesh, wrappedMatrix, wrappedMaterial, gameObject.layer);
            }
        }
    }
}
