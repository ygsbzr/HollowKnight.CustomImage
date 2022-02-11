using Satchel.BetterMenus;
using Modding;
using System.Reflection;

namespace CustomImage
{
   public class ModMenu
    {
        private static FileStream? logFile;
        private static StreamWriter? logWriter;
        internal static Menu? MenuRef;
        private static readonly string assetPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetCallingAssembly().Location),
            "Mods",
            "CustomImage"
        );
        public static MenuScreen GetMenu(MenuScreen lastmenu,ModToggleDelegates? toggleDelegates)
        {
            if(MenuRef==null)
            {
                MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
            }
            return MenuRef.GetMenuScreen(lastmenu);
        }
        public static Menu PrepareMenu(ModToggleDelegates toggleDelegates)
        {
            return new Menu("Custom Image".Localize(), new Element[]
            {
                Blueprints.CreateToggle(toggleDelegates,"Mod Switch".Localize(),"Open or Close".Localize(),"On".Localize(),"Off".Localize()),
                new MenuButton("Refresh Image".Localize(),"Reload your image".Localize(), (mb) =>
                {
                    RefreshImage();
                }),
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
           if(CustomImage.Instance!=null)
            {
                CustomImage.Instance.LoadAsset();
                CustomImage.Instance.ChangeSpriteInEquip();
                CustomImage.Instance.ChangeSpriteInJournal();
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
    }
}
