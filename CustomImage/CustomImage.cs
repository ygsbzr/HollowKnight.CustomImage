using UnityEngine.UI;
namespace CustomImage {
	public partial class CustomImage : Mod, ITogglableMod,ICustomMenuMod,IGlobalSettings<GlobalSetting> {
		public static CustomImage? Instance { get; private set; }


		private readonly Dictionary<string, Texture2D> textureDict = new();

		public static readonly string assetPath = Path.Combine(
			Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"CustomImage"
		);
		public static GlobalSetting globalSettings = new();
		public static string SkinPath
        {
            get
            {
				return Path.Combine(assetPath, CurrentSkin.GetId());
            }
        }
		public bool ToggleButtonInsideMenu => true;
		public MenuScreen GetMenuScreen(MenuScreen lastmenu,ModToggleDelegates? modToggle)
        {
			return ModMenu.GetMenu(lastmenu, modToggle);
        }
		public void OnLoadGlobal(GlobalSetting s) => globalSettings = s;
		public GlobalSetting OnSaveGlobal()
        {
			globalSettings.CurrentSkin = CurrentSkin.GetId();
			return globalSettings;
        }
		public override string GetVersion() => "1.6.0";

		public override void Initialize() {
			if (Instance != null) {
				return;
			}

			Instance = this;


			if (!Directory.Exists(assetPath)) {
				LogDebug("The CustomImage folder does not exist, creating");
				Directory.CreateDirectory(assetPath);
			}
			if (!Directory.Exists(Path.Combine(assetPath,"Default")))
			{
				LogDebug("The CustomImage Default folder does not exist, creating");
				Directory.CreateDirectory(Path.Combine(assetPath,"Default"));
			}
			GetSkinList();



			On.HeroController.Start += Load;
			USceneManager.activeSceneChanged += ChangeSpriteInScene;
			ModHooks.ObjectPoolSpawnHook += ChangeSprite;
			GameManager.instance.StartCoroutine(WaitForTitle());
			
		}
		private IEnumerator WaitForTitle()
        {
			yield return new WaitUntil(() => GameObject.Find("LogoTitle") != null);
            UIManager.EditMenus += EditText;

        }

        private void EditText()
        {
			GameObject btn = UIManager.instance.UICanvas.gameObject.FindGameObjectInChildren("ModListMenu").FindGameObjectInChildren("Content").FindGameObjectInChildren("ScrollMask").FindGameObjectInChildren("ScrollingPane").FindGameObjectInChildren($"{nameof(CustomImage)}_Settings");
			if (btn != null)
			{
				btn.FindGameObjectInChildren("Label")!.GetComponent<Text>().text = "Custom Image".Localize();
				btn.FindGameObjectInChildren("Description")!.GetComponent<Text>().text = $"v{GetVersion()}";
			}
        }

        private void Load(On.HeroController.orig_Start orig, HeroController self)
        {
			LoadAsset();
			orig(self);
		}

        public void Unload() {

			USceneManager.activeSceneChanged -= ChangeSpriteInScene;
			ModHooks.ObjectPoolSpawnHook -= ChangeSprite;
			On.HeroController.Start -= Load;
			
		}

		private Texture2D LoadTexture2D(string path) {
			var texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(File.ReadAllBytes(path), true);
			return texture2D;
		}

		public void LoadAsset() {
			textureDict.Clear();
			foreach (string file in Directory.GetFiles(SkinPath, "*.png")) {
				string filename = Path.GetFileNameWithoutExtension(file);
				textureDict[filename] = LoadTexture2D(file);

				LogDebug("Loaded " + filename);
			}

			Log("Asset loading done!");
		}


		private GameObject ChangeSprite(GameObject go) {
			Texture2D texture = textureDict
				.Where(pair => go.name.StartsWith(pair.Key))
				.FirstOrDefault()
				.Value;

			if (go.name.StartsWith("Grub Bottle"))
			{
				Texture2D bottleTex = textureDict.Where(pair => pair.Key.Equals("Grub Bottle-bottle")).FirstOrDefault().Value;
				GameObject grub = go.FindGameObjectInChildren("Grub");
				if (grub != null)
				{
					Texture2D grubTex = textureDict.Where(pair => pair.Key.Equals("Grub Bottle-grub")).FirstOrDefault().Value;
					if (grubTex != null)
					{
						tk2dSprite tkSprite = go.GetComponent<tk2dSprite>();
						if (tkSprite != null)
						{
							tkSprite.GetCurrentSpriteDef().material.mainTexture = grubTex;
						}
					}
				}
				SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
				if (renderer != null&&bottleTex!=null)
				{
					renderer.sprite = MakeSprite(bottleTex, renderer.sprite.pixelsPerUnit);
				}
				return go;

			}
				
			
			if (texture != null)
			{
				tk2dSprite tkSprite = go.GetComponent<tk2dSprite>();
				if (tkSprite != null)
				{
					tkSprite.GetCurrentSpriteDef().material.mainTexture = texture;
				}
				else
				{
					SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();

					if (renderer != null)
					{
						renderer.sprite = MakeSprite(texture, renderer.sprite.pixelsPerUnit);
					}


				}

				LogDebug($"Changed {go.name} Sprite in scene {go.scene.name}");
			}
			BossStatue bossStatue = go.GetComponent<BossStatue>();
				if (bossStatue != null)
				{
					GameObject statue = bossStatue.statueDisplay;
					SpriteRenderer alt = statue.GetComponentInChildren<SpriteRenderer>();
				if (texture != null)
				{
					alt.sprite = MakeSprite(texture, alt.sprite.pixelsPerUnit);
				}
					Texture2D alttex = textureDict
				.Where(pair => go.name.StartsWith(pair.Key.Replace("-alt",""))&&pair.Key.Contains("-alt"))                 
				.FirstOrDefault()
				.Value;
				if (alttex != null)
					{
						GameObject altstatue = bossStatue.statueDisplayAlt;
						SpriteRenderer altspriteRenderer = altstatue.GetComponentInChildren<SpriteRenderer>();
						altspriteRenderer.sprite = MakeSprite(alttex, altspriteRenderer.sprite.pixelsPerUnit);
					}
				
				}
			

			return go;
		}
		
		
		private void ChangeSpriteInScene(Scene prev, Scene next) {
			foreach (GameObject go in UObject.FindObjectsOfType<GameObject>()) {
				ChangeSprite(go);
			}
			ChangeSpriteInJournal();
			ChangeSpriteInEquip();

		}
		internal static void GetSkinList()
		{
			var dicts = Directory.GetDirectories(assetPath);
			SkinList = new();
			for (int i = 0; i < dicts.Length; i++)
			{
				string directoryname = new DirectoryInfo(dicts[i]).Name;
				SkinList.Add(new CIlist(directoryname));
			}
			CustomImage.CurrentSkin = ModMenu.GetSkinById(globalSettings.CurrentSkin);
			Modding.Logger.Log("Load Skinslist");
		}
		private Sprite MakeSprite(Texture2D tex, float ppu) =>
			Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
		public static List<ISelectableSkin>? SkinList;
		public static ISelectableSkin? CurrentSkin;
		public static ISelectableSkin? DefaultSkin;


	}
}
