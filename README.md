# ShoeMod
An add-on for Blockland that provides a framework for adding and wearing recolorable shoes.

## Usage

Download this project as a .zip and rename it to `Server_ShoeMod`, then place it in your add-ons folder. **Unlike HatMod, this support mod AND ShoeMod add-ons must be enabled for shoes to show up ingame** - this allows you to pick and choose which shoes to have ingame without needing to remove or add files to your add-ons folder.

## How to create shoes

### Required files

Shoes require 3 files: `lShoe.dts`, `rShoe.dts`, and `settings.txt`.
- `lShoe.dts` and `rShoe.dts` correspond to the models used for the left and right shoes, respectively. They can be untextured or textured.
- `settings.txt` contains configuration information for the shoes, such as shoe nodes, colors, and player node hiding. The Shoe Settings section contains more information.

### Packaging

To package the shoe add-on, create a .zip containing an empty `server.cs`, and a `description.txt`, similar to `Print_` add-ons. This is required for the add-on to show up in the add-ons list.

ShoeMods with **only one shoe** should have a name formatted like `ShoeMod_Name`. Place the required files directly in the folder (not in a subfolder). The shoe will show up ingame with the add-on's name. For example, `ShoeMod_Tennis_Shoes`, if enabled, will be loaded ingame as `Tennis Shoes`

ShoeMods with **multiple shoes** should have the required files for each shoe in subfolders within the ShoeMod add-on.
(ex: `ShoeMod_Shoe_Pack/Military Boots/` or `ShoeMod_Shoe_Pack/Sports/Nike Jordans/`)
Place the required files for each shoe directly inside its subfolder. The shoes will show up ingame with the containing folder's name (ex: `Military Boots`, `Nike Jordans`).

### Settings

Settings files support the following special tags. Each needs to be on its own line.
Type | Description
---- | -----------
`nodes` | A space-delimited list of shoe node names. Only nodes in this list can be recolored ingame or have its color set using the `color` setting. <br> <br> Example: `nodes shoe sole shoelaces`
`hideNodeList` | A space-delimited list of player node names to hide on wearing the shoes. A full list of player nodes can be found [here](#node-list). <br> <br> Example: `hideNodeList lShoe rShoe hip`
`showNodeList` | A space-delimited list of player node names to forcibly unhide on wearing the shoes. A full list of player nodes can be found [here](#node-list). <br> <br> Example: `showNodeList skirtHip` unhides the upper skirt node.
`deleteIf[Shoe/Peg/Skirt]` | If the [Shoe/Peg/Skirt] player node(s) are unhidden, the corresponding shoe(s) will be deleted. <br> <br> Examples: `deleteIfPeg` will delete shoes mounted on a peg leg; `deleteIfSkirt` will delete both shoes if the player has a skirt on.
`color` | Determines the default color a shoe node will have. First word must be a node name in the nodes list, and the following 1-3 words specify the color. Color formats supported are RGB, percent RGB, hex, and [avatar color](#avatar-colors). <br> <br> Examples: <br> `color shoe 0.2 0.2 0.2` sets the `shoe` node to dark gray. <br> `color shoelaces 255 200 0` sets the `shoelace` node to gold. <br> `color sole 1a003d` sets the `sole` node to dark purple.
`nodeTransparency` | Determines the transparency of a given node. Shoe color customization (as well as `color` tags) do not allow players to set transparency. Automatically sets the `hasTransparency` tag internally. <br> <br> Example: `nodeTransparency jar 0.25` renders the jar node at 25% opacity.
`hasTransparency` | Required tag if there are any textured nodes with transparency. Used if certain nodes are not intended to be recolorable or have a custom texture, and are also set to be transparent in the model during export.

#### :memo: For scripters:
Any tags not part of the above list will still be loaded from the `settings.txt` file, to make it easier to extend/add support packages to this mod. If you would like to package over the settings registration process, package the `registerShoeScriptObjectVar` function in `register.cs`. The shoe color system in `customization.cs` is structured to be an example of how to utilize this.

Settings are stored as fields on the shoe script object, which you can get with `getShoeScriptObject(%var)` where `%var` is the shoe name or a shoe datablock.

### Avatar Colors

Using these keywords as the specified color in a `color` setting will turn the specified node into the color used by the client for that part of their avatar. For example, `color shoe legColor` will make the default color for the left shoe the client's left shoe color (as well as the right shoe and the client's right shoe color).
```
  accentColor
  chestColor
  hatColor
  headColor
  hipColor
  packColor
  armColor //uses the color of the side the shoe is on
  handColor //uses the color of the side the shoe is on
  legColor //uses the color of the side the shoe is on
  secondPackColor
  ```

### Node list

A list of default player nodes for reference

- `headSkin` 
  - `helmet`, `copHat`, `knitHat`, `scoutHat`, `bicorn`, `pointyHelmet`, `flareHelmet`
    - `plume`, `triplume`, `septplume`, `visor`
- `chest` / `femchest` 
  - `armor`, `pack`, `quiver`, `tank`, `bucket`, `cape` 
    - `epaulets`, `epauletsRankA`, `epauletsRankB`, `epauletsRankC`, `epauletsRankD`, `ShoulderPads` 
- `rArm` / `rArmSlim`, `lArm` / lArmSlim
  - `rHand` / `rHook`, `lHand` / `lHook` 
- `pants` 
  - `rShoe` / `rPeg`, `lShoe` / `lPeg` 
- `skirtHip`                          (This replaces the pants node)
  - `SkirtTrimRight`, `SkirtTrimLeft` (These replace the respective leg nodes)