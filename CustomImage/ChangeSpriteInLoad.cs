
namespace CustomImage
{
    public partial class CustomImage
    {
        public void ChangeSpriteInLoad()
        {
           /* GameObject LoadingCanvas = UIManager.instance.gameObject.FindGameObjectInChildren("LoadingCanvas");
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
                                ReflectionHelper.SetField(checkpoint, "startSprites", Extension.MakeOrigSpriteArray(start, textureicon));
                                Sprite[] end = ReflectionHelper.GetField<CheckpointSprite, Sprite[]>(checkpoint, "endSprites");
                                ReflectionHelper.SetField(checkpoint, "endSprites", Extension.MakeOrigSpriteArray(end,textureicon));
                                Sprite[] loop = ReflectionHelper.GetField<CheckpointSprite, Sprite[]>(checkpoint, "loopSprites");
                                ReflectionHelper.SetField(checkpoint, "loopSprites", Extension.MakeOrigSpriteArray(loop, textureicon));
                            }
                        }
                      
                    }
                }
            }*/
        }
    }
}
