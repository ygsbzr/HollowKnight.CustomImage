using UnityEngine;
namespace CustomImage
{
    public static class Extension
    {
        public static List<GameObject> GetAllGameobjectsInChildren(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            List<GameObject> list = new List<GameObject>();
            Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (Transform transform in componentsInChildren)
            {
                    list.Add(transform.gameObject);
            }

            return list;
        }
    }
}
