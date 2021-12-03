using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Modding;
using UnityEngine;
using System.Reflection;
using System.IO;
namespace CustomImage
{
    public class CustomImage:Mod,ITogglableMod
    {
        Dictionary<string, Sprite> customobjects = new Dictionary<string, Sprite>();
        Dictionary<string, Texture2D> customspirte = new Dictionary<string, Texture2D>();
        static string DATA_DIR = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "/Mods";
        static string objectfolder = "CustomImage";
        static string objectpath = Path.Combine(DATA_DIR, objectfolder);
        static string textname = "Objectlist.txt";
        static string textpath = Path.Combine(objectpath, textname);
        FileStream newfile = null;
        StreamWriter writer;
        string[] files;
        

        
        private Sprite LoadSprite(string path)
        {
            byte[] texByte = File.ReadAllBytes(path);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(texByte, true);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        public void Checkpath()
        {
            if(!Directory.Exists(objectpath))
            {
                Log("You have not the CustomImage Directory,now create one");
                Directory.CreateDirectory(objectpath);
            }
           if(!File.Exists(textpath))
            {
                newfile = new FileStream(textpath, FileMode.CreateNew, FileAccess.Write);
                writer = new StreamWriter(newfile);
            }
            else
            {
                writer = new StreamWriter(textpath, false);
            }
        }
        private Texture2D LoadTex(string path)
        {
            byte[] texByte = File.ReadAllBytes(path);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(texByte, true);
            return texture2D;
        }
        public override void Initialize()
        {
            Checkpath();
            files = Directory.GetFiles(objectpath, "*.png");
            ModHooks.NewGameHook += Preload;
            ModHooks.AfterSavegameLoadHook += Preload2;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += Change;
            ModHooks.SlashHitHook += this.Record;
            ModHooks.BeforeSavegameSaveHook += CloseText;
        }

        private void Record(UnityEngine.Collider2D otherCollider, GameObject slash)
        {
            writer.WriteLine("you hit "+otherCollider.gameObject.name);
            
        }

        private void CloseText(SaveGameData obj)
        {
            writer.Flush();
            writer.Close();
        }

        

        public override string GetVersion()
        {
            return "1.5";
        }

        private void Preload2(SaveGameData obj)
        {
            Preload();
        }

        private void Preload()
        {
            foreach(var file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                customobjects.Add(filename, LoadSprite(file));
                customspirte.Add(filename, LoadTex(file));
                Log("Preload " + filename);
            }
            Log("Preload done!");
            ModHooks.ObjectPoolSpawnHook += ChangeSprite;
            
        }

        private void Change(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            foreach (GameObject gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
               
                foreach (var key in customobjects.Keys)
                {
                    
                    if (gameObject.name.Contains(key))
                    {
                       if(gameObject.GetComponent<tk2dSprite>()!=null)
                        {
                            gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = customobjects[key].texture;
                        }
                        else
                        {
                            Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
                            Sprite newsp = Sprite.Create(customspirte[key], new Rect(0f, 0f, (float)customspirte[key].width, (float)customspirte[key].height), new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
                            gameObject.GetComponent<SpriteRenderer>().sprite = newsp;
                        
                        }
                        Log("Change" + gameObject.name + " Sprite by scene");
                        break;
                    }
                }
            }
        }
        

        private GameObject ChangeSprite(GameObject arg)
        {
            foreach(var key in customobjects.Keys)
            {
                if(arg.name.Contains(key))
                {
                    if (arg.GetComponent<tk2dSprite>() != null)
                    {
                        arg.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = customobjects[key].texture;
                    }
                    else
                    {
                        Sprite sprite = arg.GetComponent<SpriteRenderer>().sprite;
                        Sprite newsp = Sprite.Create(customspirte[key], new Rect(0f, 0f, (float)customspirte[key].width, (float)customspirte[key].height), new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
                        arg.GetComponent<SpriteRenderer>().sprite = newsp;
                    }

                    Log("Change" + arg.name + " Sprite");
                    break;
                }
            }
            return arg;
        }
        public void Unload()
        {
            ModHooks.NewGameHook -= Preload;
            ModHooks.AfterSavegameLoadHook -= Preload2;
            ModHooks.ObjectPoolSpawnHook -= ChangeSprite;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= Change;
            ModHooks.SlashHitHook -= this.Record;
            ModHooks.BeforeSavegameSaveHook -= CloseText;

        }
    }
}
