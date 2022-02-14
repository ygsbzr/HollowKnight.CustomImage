using Satchel.BetterMenus;
using UnityEngine;
using System.Collections;
namespace CustomImage
{
    public interface ISelectableSkin
    {
        /// <summary>
        ///  GetId
        /// </summary>
        /// <returns>The unique id of the skin as a <c>string</c></returns>
        public string GetId();

        /// <summary>
        ///  GetName
        /// </summary>
        /// <returns>The Name to be displayed in the menu as a <c>string</c></returns>
        public string GetName();
    }
    internal class CIlist:ISelectableSkin
    {
        internal static Menu? MenuRef;
        internal static MenuScreen? lastMenu;
        private static bool applying = false;
        public string SkinDirectory = "";
        public CIlist(string DirectoryName)
        {
            SkinDirectory = DirectoryName;
        }
        public string GetId() => SkinDirectory;
        public string GetName() => SkinDirectory;
        internal static Menu PrepareMenu()
        {
            var menu = new Menu("Select a image directory".Localize(), new Element[]{
                new TextPanel("Select the directory to Apply".Localize(),Id:"helptext"),
                new TextPanel("Applying image...".Localize(),Id:"applying"){isVisible=false}
            });
            for (var i = 0; i <CustomImage.SkinList.Count; i++)
            {
                menu.AddElement(ApplySkinButton(i));
            }

            return menu;
        }
        internal static string MaxLength(string skinName, int length)
        {
            return skinName.Length <= length ? skinName : skinName.Substring(0, length - 3) + "...";
        }
        internal static Satchel.BetterMenus.MenuButton ApplySkinButton(int index)
        {

            var ButtonText = MaxLength(CustomImage.SkinList[index].GetName(), CustomImage.globalSettings.NameLength);
            return new MenuButton(ButtonText, "", (mb) =>
            {
                if (!applying)
                {
                    applying = true;
                    // apply the skin
                    ModMenu.selectedSkin = index;
                    GameManager.instance.StartCoroutine(applyAndGoBack());
                }
            }, Id: $"skinbutton{CustomImage.SkinList[index].GetId()}");

        }
        private static void setSkinButtonVisibility(bool isVisible)
        {
            for (var i = 0; i < CustomImage.SkinList.Count; i++)
            {
                var btn = MenuRef?.Find($"skinbutton{CustomImage.SkinList[i].GetId()}");
                if (btn != null)
                {
                    btn.isVisible = isVisible;
                }
            }
            MenuRef.Update();
        }
        private static IEnumerator applyAndGoBack()
        {
            //update menu ui
            MenuRef.Find("helptext").isVisible = false;
            MenuRef.Find("applying").isVisible = true;
            setSkinButtonVisibility(false);
            yield return new WaitForSecondsRealtime(0.2f);
            ModMenu.MenuRef?.Find("SelectSkinOption")?.updateAfter(_ => ModMenu.RefreshImage());
            yield return new WaitForSecondsRealtime(0.2f);

            UIManager.instance.UIGoToDynamicMenu(lastMenu);
            yield return new WaitForSecondsRealtime(0.2f);

            //menu ui initial state
            MenuRef.Find("helptext").isVisible = true;
            MenuRef.Find("applying").isVisible = false;
            setSkinButtonVisibility(true);
        }
        internal static MenuScreen GetMenu(MenuScreen lastMenu)
        {
            if (MenuRef == null)
            {
                MenuRef = PrepareMenu();
            }

            applying = false;
            CIlist.lastMenu = lastMenu;
            return MenuRef.GetMenuScreen(lastMenu);
        }

    }
}
