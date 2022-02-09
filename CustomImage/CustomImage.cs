using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Modding;
using Satchel;
using UnityEngine;
using UnityEngine.SceneManagement;

using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;
namespace CustomImage {
	public class CustomImage : Mod, ITogglableMod {
		public static CustomImage Instance { get; private set; }


		private readonly Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();

		private static readonly string assetPath = Path.Combine(
			Path.GetDirectoryName(Assembly.GetCallingAssembly().Location),
			"Mods",
			"CustomImage"
		);

		private FileStream logFile;
		private StreamWriter logWriter;


		public override string GetVersion() => "1.5.6";

		public override void Initialize() {
			if (Instance != null) {
				return;
			}

			Instance = this;


			if (!Directory.Exists(assetPath)) {
				LogDebug("The CustomImage folder does not exist, creating");
				Directory.CreateDirectory(assetPath);
			}

			logFile = new FileStream(
				Path.Combine(assetPath, "Objectlist.txt"),
				FileMode.Create,
				FileAccess.Write
			);
			logWriter = new StreamWriter(logFile);


            On.HeroController.Start += Load;
			USceneManager.activeSceneChanged += ChangeSpriteInScene;
			ModHooks.ObjectPoolSpawnHook += ChangeSprite;
            ModHooks.BeforeSavegameSaveHook += CloseWriter;
			ModHooks.SlashHitHook += RecordGO;
		}

        private void CloseWriter(SaveGameData obj)
        {
			logWriter.Flush();
			logWriter.Close();
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
			ModHooks.BeforeSavegameSaveHook -= CloseWriter;
			ModHooks.SlashHitHook -= RecordGO;

			logFile.Close();
			logWriter.Close();

			logFile = null;
			logWriter = null;

			Instance = null;
		}

		private Texture2D LoadTexture2D(string path) {
			var texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(File.ReadAllBytes(path), true);
			return texture2D;
		}

		private void LoadAsset() {
			foreach (string file in Directory.GetFiles(assetPath, "*.png")) {
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
				Texture2D bottleTex = textureDict["Grub Bottle"];
				GameObject grub = go.FindGameObjectInChildren("Grub");
				if (grub != null)
				{
					Texture2D grubTex = textureDict["Grub"];
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
			if(go.name.StartsWith("Grub Mimic Top"))
            {
				Texture2D bottleTex = textureDict["Grub Bottle"];
				GameObject minic = go.FindGameObjectInChildren("Grub Mimic 1");
				if (minic != null)
				{
					Texture2D minicTex = textureDict["Minic"];
					if (minicTex != null)
					{
						tk2dSprite tkSprite = go.GetComponent<tk2dSprite>();
						if (tkSprite != null)
						{
							tkSprite.GetCurrentSpriteDef().material.mainTexture = minicTex;
						}
					}
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
		private void ChangeSpriteInJournal()
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
		private void ChangeSpriteInEquip()
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
				if(item.name.Equals("Nail"))
                {
					InvNailSprite nailSprite = item.GetComponent<InvNailSprite>();
					Texture2D NailTex1 = textureDict
				.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0"))
				.FirstOrDefault()
				.Value;
					if(NailTex1 != null)
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
				}
                else
                {
					Texture2D ItemTex = textureDict
				.Where(pair => item.name.StartsWith(pair.Key.Replace("-item", "")) && pair.Key.EndsWith("-item"))
				.FirstOrDefault()
				.Value;
					InvItemDisplay itemDisplay=item.GetComponent<InvItemDisplay>();
					if(ItemTex != null)
                    {
						if(itemDisplay != null)
                        {
							itemDisplay.activeSprite = MakeSprite(ItemTex, itemDisplay.activeSprite.pixelsPerUnit);
                        }
                        else
                        {
							SpriteRenderer sprite=item.GetComponent<SpriteRenderer>();
							if(sprite != null)
                            {
								sprite.sprite=MakeSprite(ItemTex, sprite.sprite.pixelsPerUnit);
                            }
                        }
                    }
                }
				
			}

		}
		private void ChangeSpriteInScene(Scene prev, Scene next) {
			foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
				ChangeSprite(go);
			}
			ChangeSpriteInJournal();
			ChangeSpriteInEquip();

		}

		private Sprite MakeSprite(Texture2D tex, float ppu) =>
			Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);

		private void RecordGO(Collider2D collider, GameObject _) =>
			logWriter.WriteLine("Hit " + collider.gameObject.name);
	}
}
