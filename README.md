# ShoeMod
An add-on for Blockland, the game where you build stuff.

# How to create shoes

---

Shoes require 3 files: `lShoe.dts`, `rShoe.dts`, and `settings.txt`.
- `lShoe.dts` and `rShoe.dts` correspond to the models used for the left and right shoes, respectively. They can be untextured or textured.
- `settings.txt` contains configuration information for the shoes, such as shoe nodes, colors, and player node hiding. The Shoe Settings section contains more information.

### Packaging

---

To **package** the shoe add-on, create a .zip containing an empty `server.cs`, and a `description.txt`, similar to `Print_` add-ons. ShoeMod add-ons, as well as this support mod, must be enabled in the add-on list in order to show up ingame.

ShoeMods with **only one shoe** should have its name formatted after `ShoeMod_Name`. Place the required files directly in the folder (not in a subfolder). The shoe will show up ingame with the add-on's name.
`ShoeMod_Tennis_Shoes`, when properly packaged and enabled, will be loaded ingame as `Tennis Shoes`

ShoeMods with **multiple shoes** should have the required files for each shoe in subfolders within the ShoeMod add-on.
(ex: `ShoeMod_Shoe_Pack/Military Boots/` or `ShoeMod_Shoe_Pack/Sports/Nike Jordans/`)
Place the required files for each shoe directly inside its subfolder. The shoes will show up ingame with the containing folder's name (ex: `Military Boots`, `Nike Jordans`).

### Shoe settings

---

Settings files support the following special tags:
Type | Description
---- | -----------
`nodes` | A space-delimited list of shoe node names. Only nodes in this list can be recolored ingame or have its color set using the `color` setting.
`hideNodeList` | A space-delimited list of player node names to hide on wearing the shoes.
`showNodeList` | A space-delimited list of player node names to forcibly unhide on wearing the shoes.
`deleteIf[Shoe/Peg/Skirt]` | If the [Shoe/Peg/Skirt] player node(s) are unhidden, the corresponding shoe will be deleted. Used if for example a shoe should only be equipped on a given leg if the player has a peg leg.
`color` | a
`hasTransparency` | a

Parts:
`nodes primary secondary accent shoelaces`
`hideNodeList`
`showNodeList`
`deleteifshoe`
`deleteifpeg`
`deleteifskirt`

`color nodename r g b`
`color nodename clientcolor`
`hasTransparency`

clientcolors:
```
  accentColor
  chestColor
  hatColor
  headColor
  hipColor
  packColor
  armColor //uses same side the shoe is on
  handColor //uses same side the shoe is on
  legColor //uses same side the shoe is on
  secondPackColor
  ```


node list:
```
headSkin
  - helmet, copHat, knitHat, scoutHat, bicorn, pointyHelmet, flareHelmet
    -- plume, triplume, septplume, visor

chest / femchest
  - armor, pack, quiver, tank, bucket, cape
    -- epaulets, epauletsRankA, epauletsRankB, epauletsRankC, epauletsRankD, ShoulderPads

RArm / RArmSlim, LArm / LArmSlim
  - RHand / RHook, LHand / LHook

pants
  - RShoe / RPeg, LShoe / LPeg
    -- RSki* / LSki*

skirtHip                          (This replaces the pants node.)
  - SkirtTrimRight, SkirtTrimLeft (These replace their respective leg nodes.)
```