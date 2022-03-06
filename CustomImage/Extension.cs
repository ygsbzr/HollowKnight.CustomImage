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
        public static Sprite[] MakeOrigSpriteArray(Sprite[] origSprite,Texture2D origtex,Vector2 Piv)
        {
            List<Sprite> list = new();
            foreach(Sprite sprite in origSprite)
            {
                Sprite replacesprite = CustomImage.MakeOrigSprite(origtex, sprite.rect,Piv,sprite.pixelsPerUnit);
                list.Add(replacesprite);
            }
            return list.ToArray();
        }
    }
}
