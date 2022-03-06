
namespace CustomImage
{
    public partial class CustomImage
    {
        public void ChangeSpriteInLoad()
        {
            GameObject LoadingCanvas = GameObject.Find("_UIManager").FindGameObjectInChildren("LoadingCanvas");
            if(LoadingCanvas != null)
            {
                foreach(GameObject Spinner in LoadingCanvas.GetAllGameobjectsInChildren())
                {
                    Texture2D textureicon = textureDict
                    .Where(pair => Spinner.name.StartsWith(pair.Key.Replace("-load", "")) && pair.Key.Contains("-load"))
                    .FirstOrDefault()
                    .Value;
                    if(textureicon != null)
                    {
                        if (Spinner.name == "CheckpointIcon")
                        {
                            CheckpointSprite checkpoint = Spinner.GetComponent<CheckpointSprite>();
                            if (checkpoint != null)
                            {
                                Sprite[] start = ReflectionHelper.GetField<CheckpointSprite, Sprite[]>(checkpoint, "startSprites");
                                ReflectionHelper.SetField(checkpoint, "startSprites", Extension.MakeOrigSpriteArray(start, textureicon,new(100f,100f)));
                                Sprite[] end = ReflectionHelper.GetField<CheckpointSprite, Sprite[]>(checkpoint, "endSprites");
                                ReflectionHelper.SetField(checkpoint, "endSprites", Extension.MakeOrigSpriteArray(end,textureicon, new(100f, 100f)));
                                Sprite[] loop = ReflectionHelper.GetField<CheckpointSprite, Sprite[]>(checkpoint, "loopSprites");
                                ReflectionHelper.SetField(checkpoint, "loopSprites", Extension.MakeOrigSpriteArray(loop, textureicon, new(100f, 100f)));
                            }
                        }
                    }
                }
            }
        }
        public static Sprite MakeOrigSprite(Texture2D origtex, Rect origrect,Vector2 origpv, float ppu) => Sprite.Create(origtex, origrect,origpv, ppu);
    }
}
