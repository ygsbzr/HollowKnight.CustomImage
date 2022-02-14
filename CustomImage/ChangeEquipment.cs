
namespace CustomImage
{
    public partial class CustomImage
    {
		public void ChangeSpriteInJournal()
		{
			GameObject Journallist = GameObject.Find("_GameCameras").FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").FindGameObjectInChildren("Journal").FindGameObjectInChildren("Enemy List");
			foreach (GameObject Journal in Journallist.GetAllGameobjectsInChildren())
			{
				Texture2D texturefull = textureDict
				.Where(pair => Journal.name.StartsWith(pair.Key.Replace("-icon", "")) && !pair.Key.Contains("-icon"))
				.FirstOrDefault()
				.Value;
				Texture2D textureicon = textureDict
				.Where(pair => Journal.name.StartsWith(pair.Key.Replace("-icon", "")) && pair.Key.Contains("-icon"))
				.FirstOrDefault()
				.Value;
				if (texturefull != null)
				{
					JournalEntryStats journalEntry = Journal.GetComponent<JournalEntryStats>();
					if (journalEntry != null)
						journalEntry.sprite = MakeSprite(texturefull, journalEntry.sprite.pixelsPerUnit);
					LogDebug($"Change Sprite of{Journal.name} in Journal");
				}
				if (textureicon != null)
				{
					GameObject Portrait = Journal.FindGameObjectInChildren("Portrait");
					if (Portrait != null)
					{
						SpriteRenderer iconsprite = Portrait.GetComponent<SpriteRenderer>();
						if (iconsprite != null)
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
			foreach (var item in Equipment.GetAllGameobjectsInChildren())
			{
				Texture2D itemTex = textureDict
				.Where(pair => item.name.StartsWith(pair.Key.Replace("-equip", "")) && pair.Key.Contains("-equip"))
				.FirstOrDefault()
				.Value;
				if (itemTex != null)
				{
					SpriteRenderer itemsprite = item.GetComponent<SpriteRenderer>();
					if (itemsprite != null)
					{
						itemsprite.sprite = MakeSprite(itemTex, itemsprite.sprite.pixelsPerUnit);
					}
					LogDebug($"Change Equipment sprite of{item.name}");
				}
			}
			foreach (var item in InvItems.GetAllGameobjectsInChildren())
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
						SpriteRenderer bgrender = item.GetComponent<SpriteRenderer>();
						if (bgrender != null && maskbgTex != null)
						{
							bgrender.sprite = MakeSprite(maskbgTex, bgrender.sprite.pixelsPerUnit);
						}
						Texture2D mask1Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
						SpriteRenderer maskrender1 = p1.GetComponent<SpriteRenderer>();
						if (maskrender1 != null && mask1Tex != null)
						{
							maskrender1.sprite = MakeSprite(mask1Tex, maskrender1.sprite.pixelsPerUnit);
						}
						Texture2D mask2Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_2", "")) && pair.Key.Contains("-item_2")).FirstOrDefault().Value;
						SpriteRenderer maskrender2 = p2.GetComponent<SpriteRenderer>();
						if (maskrender2 != null && mask2Tex != null)
						{
							maskrender2.sprite = MakeSprite(mask2Tex, maskrender2.sprite.pixelsPerUnit);
						}
						Texture2D mask3Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_3", "")) && pair.Key.Contains("-item_3")).FirstOrDefault().Value;
						SpriteRenderer maskrender3 = p3.GetComponent<SpriteRenderer>();
						if (maskrender3 != null && mask3Tex != null)
						{
							maskrender3.sprite = MakeSprite(mask3Tex, maskrender3.sprite.pixelsPerUnit);
						}
						Texture2D mask4Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_4", "")) && pair.Key.Contains("-item_4")).FirstOrDefault().Value;
						SpriteRenderer maskrender4 = p4.GetComponent<SpriteRenderer>();
						if (mask4Tex != null && maskrender4 != null)
						{
							maskrender4.sprite = MakeSprite(mask4Tex, maskrender4.sprite.pixelsPerUnit);
						}
						break;
					case "Soul Orb":
						InvVesselFragments vesselFragments = item.GetComponent<InvVesselFragments>();
						if (vesselFragments != null)
						{
							Texture2D velbgTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0")).FirstOrDefault().Value;
							if (velbgTex != null)
							{
								vesselFragments.backboardSprite = MakeSprite(velbgTex, vesselFragments.backboardSprite.pixelsPerUnit);
							}
							Texture2D vel1Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
							if (vel1Tex != null)
							{
								vesselFragments.singlePieceSprite = MakeSprite(vel1Tex, vesselFragments.singlePieceSprite.pixelsPerUnit);
							}
							Texture2D vel2Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_2", "")) && pair.Key.Contains("-item_2")).FirstOrDefault().Value;
							if (vel2Tex != null)
							{
								vesselFragments.doublePieceSprite = MakeSprite(vel2Tex, vesselFragments.doublePieceSprite.pixelsPerUnit);
							}
							Texture2D vel3Tex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_3", "")) && pair.Key.Contains("-item_3")).FirstOrDefault().Value;
							if (vel3Tex != null)
							{
								vesselFragments.fullSprite = MakeSprite(vel3Tex, vesselFragments.fullSprite.pixelsPerUnit);
							}

						}
						break;

					default:
						InvItemDisplay itemDisplay = item.GetComponent<InvItemDisplay>();
						if (itemDisplay != null)
						{
							Texture2D activeTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_1", "")) && pair.Key.Contains("-item_1")).FirstOrDefault().Value;
							Texture2D inactiveTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item_0", "")) && pair.Key.Contains("-item_0")).FirstOrDefault().Value;
							if (activeTex != null)
							{
								itemDisplay.activeSprite = MakeSprite(activeTex, itemDisplay.activeSprite.pixelsPerUnit);
							}
							if (inactiveTex != null)
							{
								itemDisplay.inactiveSprite = MakeSprite(inactiveTex, itemDisplay.inactiveSprite.pixelsPerUnit);
							}
							break;
						}
						SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
						if (spriteRenderer != null)
						{
							Texture2D SpriteTex = textureDict.Where(pair => item.name.StartsWith(pair.Key.Replace("-item", "")) && pair.Key.EndsWith("-item")).FirstOrDefault().Value;
							if (SpriteTex != null)
							{
								spriteRenderer.sprite = MakeSprite(SpriteTex, spriteRenderer.sprite.pixelsPerUnit);
							}
						}
						break;
				}
			}
		}
	}
}
