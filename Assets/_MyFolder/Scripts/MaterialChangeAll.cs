using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeAll : MonoBehaviour
{
    [SerializeField] private Material m;

    public void ChangeAll()
    {
        var allObjectInChild = GetAll(gameObject);

        foreach (var child in allObjectInChild)
        {
            if (child.TryGetComponent(out Renderer r))
            {
                r.sharedMaterial = m;
            }
        }
    }
    
    private List<GameObject>  GetAll (GameObject obj)
    {
        List<GameObject> allChildren = new List<GameObject> ();
        GetChildren (obj, ref allChildren);
        return allChildren;
    }

    //子要素を取得してリストに追加
    private void GetChildren (GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform> ();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach (Transform ob in children) {
            allChildren.Add (ob.gameObject);
            GetChildren (ob.gameObject, ref allChildren);
        }
    }
}
