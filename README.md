# shoemod
Shoemod for Blockland

# How to create shoes
Create a .zip containing an empty `server.cs` and a `description.txt` with any contents.
Place an lShoe.dts and rShoe.dts file inside it (along with any necessary textures), and an optional `settings.txt` file.

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
  larm
  larmColor
  lhandColor
  llegColor
  packColor
  rarmColor
  rhandColor
  rlegColor
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