# HollowKnight.CustomImage
A Hollow Knight Mod can change some sprites(for1.5.78)，need Satchel
## Adding custom Image
1. Install the mod
2. Have `.png` image files
3. Run the game once and use nail to hit what you want to change
    1. This generates a folder in Location where CustomImage installed named `CustomImage`.
4. Exit the game
5. Go to the `CustomImage` folder,and create a folder by yourself.(Yeah, like Custom Knight)
6. Move the image files here and rename them as `<gamobjectname>.png`
7. Start the game
8. if the sprite does not change ,click the `refresh image` button in mod menu.
9. for bossstatue,rename the image as `GG_Statue_<bossname>.png`,for some altbossstatue(like Nosk Hornet),rename the image as `GG_Statue_<bossname>-alt.png`
10. for Journal image, rename the image as `Journal <gameobjectname>(Clone).png`,for its icon,rename it as`Journal <gameobjectname>(Clone)-icon.png`
11. rename `grub` image as `Grub Bottle-grub.png`,and its bottle `Grub Bottle-bottle.png`
12. For some image in Equipment,such as Ore,Love Key,Simple Key and so on,rename the image as `<GameObjectname>-equip.png`, like `Rancid Egg-equip.png`
13. For change Nail image in Equipment,rename image as `Nail-item_(NailUpgrades).png`,like `Nail-item_4.png` for pure nail
14. For Mask image in Equipment,rename image as `Heart Pieces-item_(piecenumber).png`,for example,`Heart Pieces-item_4.png` for full mask.
15. For Soul Orb image in Equipment,rename image as `Soul Orb-item_(piecenumber).png`,for example,`Soul Orb-item_3.png` for full Vessel.
16. For something has component `InvItemDisplay`(like dreamnail,Nail art), rename image as `<GameObjectname>-item_1.png`(activeSprite) or `<GameObjectname>-item_0.png`(inactiveSprite)
16. For other InvItem, rename the image as `<GameObjectname>-item.png`. 

