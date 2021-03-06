using Satchel.BetterMenus;
namespace CustomImage
{
    ////Thank CutomKnight menu example:https://github.com/PrashantMohta/HollowKnight.CustomKnight/blob/moreskin/CustomKnight/Menu/BetterMenu.cs
    public class ModMenu
    {
        private static FileStream? logFile;
        private static StreamWriter? logWriter;
        internal static Menu? MenuRef;
        public static int selectedSkin = 0;
        public static MenuScreen GetMenu(MenuScreen lastmenu,ModToggleDelegates? toggleDelegates)
        {
            if(MenuRef==null)
            {
                if(!CustomImage.CheckCK())
                {
                    MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
                    MenuRef.OnBuilt += (_, Element) =>
                    {


                        if (CustomImage.CurrentSkin != null)
                        {
                            ModMenu.SelectedSkin(CustomImage.CurrentSkin.GetId());
                        }
                    };
                }
                else
                {
                    MenuRef = PrepareMenuHaveCK();
                }
                
            }
            return MenuRef.GetMenuScreen(lastmenu);
        }
        public static Menu PrepareMenuHaveCK()
        {
            return new Menu("Custom Image".Localize(),new Element[]
            {
                new TextPanel("You install CustomKnight,Put images to CustomImage directory in Swap directory of skin you use".Localize()),
                new MenuButton("Open CustomImage folder".Localize(), "", (_) =>
                {
                    IoUtils.OpenDefault(CustomImage.CKSkinPath());
                }),
                new MenuButton("Open CustomImage Global Folder".Localize(), "", (_) =>
                {
                     IoUtils.OpenDefault(Path.Combine(CustomImage.assetPath,"Global"));
                })
            }
            );
        }
        internal static string[] getSkinNameArray()
        {
            return CustomImage.SkinList.Select(s => CIlist.MaxLength(s.GetName(),CustomImage.globalSettings.NameLength)).ToArray();
        }
        public static Menu PrepareMenu(ModToggleDelegates toggleDelegates)
        {
            return new Menu("Custom Image".Localize(), new Element[]
            {
                Blueprints.CreateToggle(toggleDelegates,"Mod Switch".Localize(),"Open or Close".Localize(),"On".Localize(),"Off".Localize()),
                new HorizontalOption(
                    "Select Image".Localize(), "The Image will be used for current".Localize(),
                    getSkinNameArray(),
                    (setting) => { selectedSkin = setting; },
                    () => selectedSkin,
                    Id:"SelectSkinOption"),
                new MenuRow(
                    new List<Element>{
                        Blueprints.NavigateToMenu( "Image List".Localize(),"Opens a list of image".Localize(),()=> CIlist.GetMenu(MenuRef.menuScreen)),
                         new MenuButton("Refresh Image".Localize(),"Refresh image in Directory".Localize(),(_)=> RefreshImage()),
                    },
                    Id:"ApplyButtonGroup"
                ){ XDelta = 400f},
                 new MenuButton("Open CustomImage folder".Localize(), "", (_) =>
                {
                    IoUtils.OpenDefault(CustomImage.assetPath);
                })
                /*new MenuButton("Start Record".Localize(), "Record what gameobject you hit by nail,write to objectlist.txt".Localize(), (mb) =>
                {
                   CreateWriter();
                    ModHooks.SlashHitHook+=RecordGO;
                }),
                new MenuButton("End Record".Localize(), "", (mb) =>
                {
                     ModHooks.SlashHitHook-=RecordGO;
                     CloseWriter();
                   
                })*/
            }) ;
        }

       /* private static void RecordGO(UnityEngine.Collider2D otherCollider, UnityEngine.GameObject slash)
        {
            logWriter.WriteLine("Hit " + otherCollider.gameObject.name);
        }*/

        public static void RefreshImage()
        {
           
            if (CustomImage.Instance!=null)
            {
                var skinToApply = CustomImage.SkinList[selectedSkin];
                ModMenu.SetSkinById(skinToApply.GetId());
                CustomImage.Instance.LoadAsset();
                GameManager.instance.StartCoroutine(CustomImage.Instance.ChangeSpriteInScene());
            }
        }
        /*private static void CreateWriter()
        {
            logFile = new FileStream(
                Path.Combine(assetPath, "Objectlist.txt"),
                FileMode.Create,
                FileAccess.Write
            );
            logWriter = new StreamWriter(logFile);
        }
        private static void CloseWriter()
        {
            if(logWriter != null)
            {
                logWriter.Flush();
                logWriter.Close();
                logWriter = null;
            }
           
        }*/
        internal static void SelectedSkin(string skinId)
        {
            selectedSkin = CustomImage.SkinList.FindIndex(skin => skin.GetId() == skinId);
        }
        public static ISelectableSkin GetSkinById(string id)
        {
            return CustomImage.SkinList.Find(skin => skin.GetId() == id) ?? GetDefaultSkin();
        }
        public static ISelectableSkin GetDefaultSkin()
        {
            if (CustomImage.DefaultSkin == null)
            {
                CustomImage.DefaultSkin = GetSkinById("Default");
            }
            return CustomImage.DefaultSkin;
        }
        public static void SetSkinById(string id)
        {
            var Skin = GetSkinById(id);
            if (CustomImage.CurrentSkin.GetId() == Skin.GetId()) { return; }
            CustomImage.CurrentSkin = Skin;
        }
    }
}
