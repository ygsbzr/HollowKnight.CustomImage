using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Modding;

using UnityEngine;
using UnityEngine.SceneManagement;

using USceneManager = UnityEngine.SceneManagement.SceneManager;

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


		public override string GetVersion() => "1.5.2";

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

			if (texture != null) {
				tk2dSprite tkSprite = go.GetComponent<tk2dSprite>();
				if (tkSprite != null) {
					tkSprite.GetCurrentSpriteDef().material.mainTexture = texture;
				} else {
					SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();

					if (renderer != null) {
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
			foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
				ChangeSprite(go);
			}
		}

		private Sprite MakeSprite(Texture2D tex, float ppu) =>
			Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);

		private void RecordGO(Collider2D collider, GameObject _) =>
			logWriter.WriteLine("Hit " + collider.gameObject.name);
	}
}
