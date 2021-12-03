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


		public override string GetVersion() => "1.5";

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


			ModHooks.NewGameHook += LoadAsset;
			ModHooks.AfterSavegameLoadHook += LoadAsset2;

			USceneManager.activeSceneChanged += ChangeSpriteInScene;
			ModHooks.ObjectPoolSpawnHook += ChangeSprite;

			ModHooks.SlashHitHook += RecordGO;
		}

		public void Unload() {
			ModHooks.NewGameHook -= LoadAsset;
			ModHooks.AfterSavegameLoadHook -= LoadAsset2;

			USceneManager.activeSceneChanged -= ChangeSpriteInScene;
			ModHooks.ObjectPoolSpawnHook -= ChangeSprite;

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
				textureDict.Add(filename, LoadTexture2D(file));

				LogDebug("Loaded " + filename);
			}

			Log("Asset loading done!");
		}

		private void LoadAsset2(SaveGameData _) => LoadAsset();

		private GameObject ChangeSprite(GameObject go) {
			Texture2D texture = textureDict
				.Where(pair => go.name.Contains(pair.Key))
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
