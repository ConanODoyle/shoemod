function createItemDatablock(%datablockName, %shapeFileDir)
{
	if (isObject(%datablockName))
	{
		return %datablockName.getID();
	}

	%evalString = "" SPC 
	"datablock ItemData(" @ %datablockName @ ")" SPC
	"{" SPC
		"shapeFile = \"" @ %shapeFileDir @ "\";" SPC
		"mass = 1;" SPC
		"density = 0.2;" SPC
		"elasticity = 0.2;" SPC
		"friction = 0.6;" SPC
		"emap = 1;" SPC
		"canDrop = 1;" SPC
	"};";

	eval(%evalString);
	return %datablockName.getID();
}

function createPlayerDatablock(%datablockName, %shapeFileDir)
{
	if (isObject(%datablockName))
	{
		return %datablockName.getID();
	}

	%evalString = "" SPC 
	"datablock PlayerData(" @ %datablockName @ ")" SPC
	"{" SPC
		"emap = 0;" SPC
		"className = \"Armor\";" SPC
		"shapeFile = \"" @ %shapeFileDir @ "\";" SPC
		"jetEmitter = playerJetEmitter;" SPC
		"jetGroundEmitter = playerJetGroundEmitter;" SPC
		"splashEmitter[0] = PlayerFoamDropletsEmitter;" SPC
		"splashEmitter[1] = PlayerFoamEmitter;" SPC
		"splashEmitter[2] = PlayerBubbleEmitter;" SPC
	"};";

	eval(%evalString);
	return %datablockName.getID();
}