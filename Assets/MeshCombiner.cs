using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class BakeMicroscript : MonoBehaviour
{
    public bool combineOnStart = true;
    public bool destroyCombinedChildren = true;
    public bool generateColliders = true; // Нужно ли создавать коллайдеры?

    void Start()
    {
        if (combineOnStart) CombineMeshes();
    }

    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
        Dictionary<Material, List<CombineInstance>> materialGroups = new Dictionary<Material, List<CombineInstance>>();

        // Временно сбрасываем трансформы родителя
        Vector3 parentPos = transform.position;
        Quaternion parentRot = transform.rotation;
        Vector3 parentScale = transform.localScale;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Группируем меши по материалам
        foreach (var filter in meshFilters)
        {
            if (filter.transform == transform) continue;

            MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            Material mat = renderer.sharedMaterial;
            if (!materialGroups.ContainsKey(mat))
                materialGroups[mat] = new List<CombineInstance>();

            CombineInstance ci = new CombineInstance
            {
                mesh = filter.sharedMesh,
                transform = filter.transform.localToWorldMatrix
            };
            materialGroups[mat].Add(ci);
        }

        // Восстанавливаем трансформы родителя
        transform.position = parentPos;
        transform.rotation = parentRot;
        transform.localScale = parentScale;

        // Создаём объединённые меши и коллайдеры
        foreach (var entry in materialGroups)
        {
            GameObject combinedPart = new GameObject("Combined_" + entry.Key.name);
            combinedPart.transform.SetParent(transform);
            combinedPart.transform.localPosition = Vector3.zero;
            combinedPart.transform.localRotation = Quaternion.identity;
            combinedPart.transform.localScale = Vector3.one;

            // Добавляем MeshFilter + MeshRenderer
            MeshFilter mf = combinedPart.AddComponent<MeshFilter>();
            MeshRenderer mr = combinedPart.AddComponent<MeshRenderer>();

            Mesh partMesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            partMesh.CombineMeshes(entry.Value.ToArray());
            mf.mesh = partMesh;
            mr.material = entry.Key;

            // Добавляем MeshCollider (если включено)
            if (generateColliders)
            {
                MeshCollider collider = combinedPart.AddComponent<MeshCollider>();
                collider.sharedMesh = partMesh;
                collider.convex = false; // Важно для сложных мешей!
            }
        }

        // Удаляем дочерние объекты (если нужно)
        if (destroyCombinedChildren)
        {
            foreach (var filter in meshFilters)
            {
                if (filter.transform != transform)
                    Destroy(filter.gameObject);
            }
        }
    }
}
