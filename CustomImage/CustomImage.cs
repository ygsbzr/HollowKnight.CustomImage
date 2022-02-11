using System.Reflection;
using Modding;
using Satchel;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;
using UnityEngine.UI;

namespace CustomImage {
	public class CustomImage : Mod, ITogglableMod,ICustomMenuMod,IGlobalSettings<GlobalSetting> {
		public static CustomImage? Instance { get; private set; }


		private readonly Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();

		public static readonly string assetPath = Path.Combine(
			Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"CustomImage"
		);
		public static GlobalSetting globalSettings = new();
		public static string SkinPath
        {
            get
            {
				return Path.Combine(assetPath, CustomImage.Instance.CurrentSkin.GetId());
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
				
				return go;
			
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
		public void ChangeSpriteInJournal()
        {
			GameObject Journallist = GameObject.Find("_GameCameras").FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").FindGameObjectInChildren("Journal").FindGameObjectInChildren("Enemy List");
			foreach(GameObject Journal in Journallist.GetAllGameobjectsInChildren())
            {
				Texture2D texturefull = textureDict
				.Where(pair => Journal.name.StartsWith(pair.Key.Replace("-icon",""))&&!pair.Key.Contains("-icon"))
				.FirstOrDefault()
				.Value;
				Texture2D textureicon = textureDict
				.Where(pair => Journal.name.StartsWith(pair.Key.Replace("-icon", "")) && pair.Key.Contains("-icon"))
				.FirstOrDefault()
				.Value;
				if(texturefull != null)
                {
					JournalEntryStats journalEntry = Journal.GetComponent<JournalEntryStats>();
					if(journalEntry != null)
					journalEntry.sprite = MakeSprite(texturefull, journalEntry.sprite.pixelsPerUnit);
					LogDebug($"Change Sprite of{Journal.name} in Journal");
				}
				if(textureicon != null)
                {
					GameObject Portrait = Journal.FindGameObjectInChildren("Portrait");
					if(Portrait != null)
                    {
						SpriteRenderer iconsprite=Portrait.GetComponent<SpriteRenderer>();
						if(iconsprite != null)
                        {
							iconsprite.sprite = MakeSprite(textureicon, iconsprite.sprite.pixelsPerUnit);
                        }
						LogDebug($"Change iconSprite of{Journal.name} in Journal");
					}
                }

			}
		}
		public void ChangeSpriteInEquip()
        {
			GameObject Inv = GameObject.Find("_GameCameras").FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").FindGameObjectInChildren("Inv");
			GameObject Equipment = Inv.FindGameObjectInChildren("Equipment");
			GameObject InvItems = Inv.FindGameObjectInChildren("Inv_Items");
			foreach(var item in Equipment.GetAllGameobjectsInChildren())
            {
				Texture2D itemTex = textureDict
				.Where(pair => item.name.StartsWith(pair.Key.Replace("-equip", "")) && pair.Key.Contains("-equip"))
				.FirstOrDefault()
				.Value;
				if(itemTex != null)
                {
					SpriteRenderer itemsprite=item.GetComponent<SpriteRenderer>();
					if (itemsprite != null)
                    {
						itemsprite.sprite=MakeSprite(itemTex, itemsprite.sprite.pixelsPerUnit);
                    }
					LogDebug($"Change Equipment sprite of{item.name}");
                }
			}
			foreach(var item in InvItems.GetAllGameobjectsInChildren())
            {
                switch (item.name)
                {
					case "Nail":
						InvNailSprite nailSprite = item.GetComponent<InvNailSprite>();
						Texture2D NailTex1 = textureDict
					.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0"))
					.FirstOrDefault()
					.Value;
						if (NailTex1 != null)
						{
							nailSprite.level1 = MakeSprite(NailTex1, nailSprite.level1.pixelsPerUnit);
						}
						Texture2D NailTex2 = textureDict
					.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1"))
					.FirstOrDefault()
					.Value;
						if (NailTex2 != null)
						{
							nailSprite.level2 = MakeSprite(NailTex2, nailSprite.level2.pixelsPerUnit);
						}
						Texture2D NailTex3 = textureDict
					.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_2", "")) && pair.Key.Contains("-item_2"))
					.FirstOrDefault()
					.Value;
						if (NailTex3 != null)
						{
							nailSprite.level3 = MakeSprite(NailTex3, nailSprite.level3.pixelsPerUnit);
						}
						Texture2D NailTex4 = textureDict
					.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_3", "")) && pair.Key.Contains("-item_3"))
					.FirstOrDefault()
					.Value;
						if (NailTex4 != null)
						{
							nailSprite.level4 = MakeSprite(NailTex4, nailSprite.level4.pixelsPerUnit);
						}
						Texture2D NailTex5 = textureDict
					.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_5", "")) && pair.Key.Contains("-item_5"))
					.FirstOrDefault()
					.Value;
						if (NailTex5 != null)
						{
							nailSprite.level5 = MakeSprite(NailTex5, nailSprite.level5.pixelsPerUnit);
						}
						break;
					case "Heart Pieces":
						GameObject p1 = item.FindGameObjectInChildren("Pieces 1");
						GameObject p2 = item.FindGameObjectInChildren("Pieces 2");
						GameObject p3 = item.FindGameObjectInChildren("Pieces 3");
						GameObject p4 = item.FindGameObjectInChildren("Pieces 4");
						Texture2D maskbgTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0")).FirstOrDefault().Value;
						SpriteRenderer bgrender=item.GetComponent<SpriteRenderer>();
						if(bgrender != null&&maskbgTex!=null)
                        {
							bgrender.sprite=MakeSprite(maskbgTex,bgrender.sprite.pixelsPerUnit);
                        }
						Texture2D mask1Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
						SpriteRenderer maskrender1 = p1.GetComponent<SpriteRenderer>();
						if (maskrender1 != null&&mask1Tex!=null)
						{
							maskrender1.sprite = MakeSprite(mask1Tex, maskrender1.sprite.pixelsPerUnit);
						}
						Texture2D mask2Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_2", "")) && pair.Key.Contains("-item_2")).FirstOrDefault().Value;
						SpriteRenderer maskrender2 = p2.GetComponent<SpriteRenderer>();
						if (maskrender2 != null&&mask2Tex!=null)
						{
							maskrender2.sprite = MakeSprite(mask2Tex, maskrender2.sprite.pixelsPerUnit);
						}
						Texture2D mask3Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_3", "")) && pair.Key.Contains("-item_3")).FirstOrDefault().Value;
						SpriteRenderer maskrender3 = p3.GetComponent<SpriteRenderer>();
						if (maskrender3 != null&&mask3Tex!=null)
						{
							maskrender3.sprite = MakeSprite(mask3Tex, maskrender3.sprite.pixelsPerUnit);
						}
						Texture2D mask4Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_4", "")) && pair.Key.Contains("-item_4")).FirstOrDefault().Value;
						SpriteRenderer maskrender4 = p4.GetComponent<SpriteRenderer>();
						if (mask4Tex != null&&maskrender4!=null)
						{
							maskrender4.sprite = MakeSprite(mask4Tex, maskrender4.sprite.pixelsPerUnit);
						}
						break;
					case "Soul Orb":
						InvVesselFragments vesselFragments=item.GetComponent<InvVesselFragments>();
						if(vesselFragments!=null)
                        {
							Texture2D velbgTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0")).FirstOrDefault().Value;
							if(velbgTex!=null)
                            {
								vesselFragments.backboardSprite= MakeSprite(velbgTex,vesselFragments.backboardSprite.pixelsPerUnit);
                            }
							Texture2D vel1Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
							if (vel1Tex!=null)
                            {
								vesselFragments.singlePieceSprite=MakeSprite(vel1Tex,vesselFragments.singlePieceSprite.pixelsPerUnit);
                            }
							Texture2D vel2Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_2", "")) && pair.Key.Contains("-item_2")).FirstOrDefault().Value;
							if (vel2Tex != null)
							{
								vesselFragments.doublePieceSprite= MakeSprite(vel2Tex, vesselFragments.doublePieceSprite.pixelsPerUnit);
							}
							Texture2D vel3Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_3", "")) && pair.Key.Contains("-item_3")).FirstOrDefault().Value;
							if (vel3Tex != null)
							{
								vesselFragments.fullSprite = MakeSprite(vel3Tex, vesselFragments.fullSprite.pixelsPerUnit);
							}

						}
						break;
					
					default:
						InvItemDisplay itemDisplay=item.GetComponent<InvItemDisplay>();
						if (itemDisplay!=null)
                        {
							Texture2D activeTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
							Texture2D inactiveTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0")).FirstOrDefault().Value;
							if(activeTex!=null)
                            {
								itemDisplay.activeSprite = MakeSprite(activeTex, itemDisplay.activeSprite.pixelsPerUnit);
                            }
							if(inactiveTex!=null)
                            {
								itemDisplay.inactiveSprite=MakeSprite(inactiveTex, itemDisplay.inactiveSprite.pixelsPerUnit);
                            }
							break;
						}
						SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
						if(spriteRenderer!=null)
                        {
							Texture2D SpriteTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item", "")) && pair.Key.EndsWith("-item")).FirstOrDefault().Value;
							if(SpriteTex!=null)
                            {
								spriteRenderer.sprite = MakeSprite(SpriteTex, spriteRenderer.sprite.pixelsPerUnit);
                            }
						}
						break;







				}
                
				
			}

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
			CustomImage.Instance.CurrentSkin = ModMenu.GetSkinById(globalSettings.CurrentSkin);
			Modding.Logger.Log("Load Skinslist");
		}
		private Sprite MakeSprite(Texture2D tex, float ppu) =>
			Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
		public static List<ISelectableSkin>? SkinList;
		public ISelectableSkin? CurrentSkin;
		public ISelectableSkin? DefaultSkin;


	}
}
