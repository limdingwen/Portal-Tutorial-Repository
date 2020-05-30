using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformTreeClone : AbstractClone
{
    public Transform generateTarget;
    public TransformTreeItem[] items;

#if UNITY_EDITOR
    [ContextMenu("Generate Tree")]
    public void EditorGenerateTree()
    {
        var itemsList = new List<TransformTreeItem>();
        var targetRootPath = AnimationUtility.CalculateTransformPath(generateTarget, null);

        TreeRecurse(transform);
        items = itemsList.ToArray();

        void TreeRecurse(Transform current)
        {
            var childPath = AnimationUtility.CalculateTransformPath(current, transform);
            var fullPath = targetRootPath + "/" + childPath;
            Debug.Log("Targeting " + fullPath);
            var childTarget = GameObject.Find(fullPath).transform;

            itemsList.Add(new TransformTreeItem
            {
                ourTransform = current,
                targetTransform = childTarget
            });

            foreach (Transform child in current)
            {
                TreeRecurse(child);
            }
        }
    }
#endif

    public override void OnCloneUpdate(Portal sender, Portal destination)
    {
        foreach (var item in items)
        {
            item.ourTransform.SetPositionAndRotation(
                Portal.TransformPositionBetweenPortals(sender, destination, item.targetTransform.position), 
                Portal.TransformRotationBetweenPortals(sender, destination, item.targetTransform.rotation));
        }
    }

    [Serializable]
    public class TransformTreeItem
    {
        public Transform ourTransform;
        public Transform targetTransform;
    }
}