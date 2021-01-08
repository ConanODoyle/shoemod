package ShoeMod_Shoes
{
	function Armor::onDisabled(%this, %obj, %state)
	{
		if (%this.keepOnDeath)
		{
			return;
		}

		%ret = parent::onDisabled(%this, %obj, %state);
		ShoeMod_remountShoes(%obj);

		return %ret;
	}

	function Armor::onRemove(%this, %obj)
	{
		ShoeMod_deleteShoes(%obj);

		return parent::onRemove(%this, %obj, %state);
	}

	function Player::setScale(%pl, %scale)
	{
		%pl.unmountObject(%pl.dummyBot);
		%pl.unmountObject(%pl.lShoe);
		%pl.unmountObject(%pl.rShoe);

		parent::setScale(%pl, %scale);
		if (isObject(%pl.dummyBot))
		{
			%pl.dummyBot.setScale(%scale);
			%pl.mountObject(%pl.dummyBot, 2);
		}
		if (isObject(%pl.rShoe))
		{
			%pl.rShoe.setScale(%scale);
			%pl.mountObject(%pl.rShoe, 3);
		}
		if (isObject(%pl.lShoe))
		{
			%pl.lShoe.setScale(%scale);
			%pl.mountObject(%pl.lShoe, 4);
		}
	}

	function GameConnection::applyBodyParts(%cl)
	{
		%ret = parent::applyBodyParts(%cl);

		if (isObject(%cl.player) && isRegisteredShoe(%cl.getCurrentShoes()) && !%cl.skipWearShoes)
		{
			ShoeMod_wearShoes(%cl.player, %cl.getCurrentShoes(), %cl);
		}

		return %ret;
	}

	function Armor::onNewDatablock(%datablock, %obj)
	{
		%ret = parent::onNewDatablock(%datablock, %obj);

		ShoeMod_remountShoes(%obj);
		
		return %ret;
	}

	function GameConnection::onClientEnterGame(%cl)
	{
		if (%cl.bl_id !$= "")
		{
			%cl.importShoeSettings();
		}

		return parent::onClientEnterGame(%cl);
	}

	function GameConnection::spawnPlayer(%cl)
	{
		%ret = parent::spawnPlayer(%cl);

		if (isRegisteredShoe(%cl.shoeSettings.currentShoes))
		{
			%cl.wearShoes(%cl.shoeSettings.currentShoes);
		}

		return %ret;
	}

	function GameConnection::onRemove(%cl)
	{
		if (isObject(%cl.shoeSettings))
		{
			%cl.exportShoeSettings();
		}

		return parent::onRemove(%cl);
	}
};
activatePackage(ShoeMod_Shoes);






function ShoeMod_remountShoes(%obj)
{
	if (isObject(%obj.dummyBot))
	{
		%obj.mountObject(%obj.dummyBot, 2);
	}
	if (isObject(%obj.rShoe))
	{
		%obj.mountObject(%obj.rShoe, 3);
	}
	if (isObject(%obj.lShoe))
	{
		%obj.mountObject(%obj.lShoe, 4);
	}
}

function ShoeMod_deleteShoes(%obj)
{
	if (isObject(%obj.rShoe))
	{
		%obj.rShoe.delete();
	}
	if (isObject(%obj.lShoe))
	{
		%obj.lShoe.delete();
	}
	if (isObject(%obj.dummyBot))
	{
		%obj.dummyBot.delete();
	}
}

function ShoeMod_removeShoes(%obj, %cl)
{
	ShoeMod_deleteShoes(%obj);

	if (%cl.player == %obj && isFunction(%cl.getClassName(), "applyBodyParts"))
	{
		%cl.applyBodyParts();
	}
}

function ShoeMod_wearShoes(%obj, %shoeName, %cl)
{
	%safeShoeName = getSafeVariableName(%shoeName);
	%scriptObj = getShoeScriptObject(%shoeName);

	%ldb = %scriptObj.lShoeDB;
	%rdb = %scriptObj.rShoeDB;

	if (!isObject(%ldb) || !isObject(%rdb))
	{
		error("ShoeMod_wearShoes: ERROR - Shoe '" @ %shoeName @ "' is missing shoe datablocks!");
		return 0;
	}

	if (!isObject(%obj.dummyBot))
	{
		%dummyBot = new AIPlayer(Shoes) {dataBlock = %ldb; side = "l"; };
		%dummyBot.kill();
		%dummyBot.setScale(%obj.getScale());
		%dummyBot.hideNode("ALL");
		%obj.dummyBot = %dummyBot;
	}
	if (!isObject(%obj.rShoe))
	{
		%rShoe = new AIPlayer(Shoes) { dataBlock = %rdb; side = "r"; };
		%rShoe.kill();
		%obj.rShoe = %rShoe;
	}
	if (!isObject(%obj.lShoe))
	{
		%lShoe = new AIPlayer(Shoes) { dataBlock = %ldb; side = "l"; };
		%lShoe.kill();
		%obj.lShoe = %lShoe;
	}

	%rShoe = %obj.rShoe;
	%lShoe = %obj.lShoe;

	%rShoe.setDatablock(%rdb);
	%lShoe.setDatablock(%ldb);

	%rShoe.setScale(%obj.getScale());
	%lShoe.setScale(%obj.getScale());

	%obj.unmountObject(%obj.dummyBot);
	%obj.unmountObject(%rShoe);
	%obj.unmountObject(%lShoe);
	%obj.mountObject(%obj.dummyBot, 2);
	%obj.mountObject(%rShoe, 3);
	%obj.mountObject(%lShoe, 4);


	validateAvatarLegs(%obj, %shoeName, %cl);
	
	return 1;
}

function validateAvatarLegs(%obj, %shoeName, %cl)
{
	%scriptObj = getShoeScriptObject(%shoeName);

	if (%scriptObj.deleteIfShoe)
	{
		if (%cl.lleg == 0) { %obj.lShoe.delete(); }
		if (%cl.rleg == 0) { %obj.rShoe.delete(); }
	}
	if (%scriptObj.deleteIfPeg)
	{
		if (%cl.lleg == 1) { %obj.lShoe.delete(); }
		if (%cl.rleg == 1) { %obj.rShoe.delete(); }
	}
	if (%scriptObj.deleteIfSkirt)
	{
		if (%cl.hip == 1) { %obj.lShoe.delete(); %obj.rShoe.delete(); }
	}

	if (getWordCount(%scriptObj.hideNodeList) > 0)
	{
		for (%i = 0; %i < getWordCount(%scriptObj.hideNodeList); %i++)
		{
			%obj.hideNode(getWord(%scriptObj.hideNodeList, %i));
		}
	}
	if (getWordCount(%scriptObj.showNodeList) > 0)
	{
		for (%i = 0; %i < getWordCount(%scriptObj.showNodeList); %i++)
		{
			%obj.unhideNode(getWord(%scriptObj.showNodeList, %i));
		}
	}
	if (getWordCount(%scriptObj.showNodeList) == 0 && getWordCount(%scriptObj.hideNodeList) == 0)
	{
		%cl.skipWearShoes = 1;
		%cl.applyBodyParts();
		%cl.skipWearShoes = 0;
	}
}






function GameConnection::setCurrentShoes(%cl, %shoeName)
{
	if (!isRegisteredShoe(%shoeName) && %shoeName !$= "None")
	{
		return 0;
	}

	%cl.shoeSettings.currentShoes = %shoeName;
}

function GameConnection::wearShoes(%cl, %shoeName)
{
	if (!isObject(%pl = %cl.player) || 
		(!isRegisteredShoe(%shoeName) && %shoeName !$= "None"))
	{
		return 0;
	}

	if (%shoeName $= "None")
	{
		%cl.unwearShoes();
	}
	else
	{
		ShoeMod_wearShoes(%pl, %shoeName, %cl);
	}
}

function Player::wearShoes(%pl, %shoeName)
{
	ShoeMod_wearShoes(%pl, %shoeName, %pl.client);
}

function AIPlayer::wearShoes(%obj, %shoeName)
{
	ShoeMod_wearShoes(%obj, %shoeName, %obj.client);
}

function GameConnection::unwearShoes(%cl)
{
	if (!isObject(%pl = %cl.player))
	{
		return 0;
	}

	ShoeMod_removeShoes(%pl, %cl);
}

function Player::unwearShoes(%pl)
{
	ShoeMod_removeShoes(%pl, %pl.client);
}

function AIPlayer::unwearShoes(%pl)
{
	ShoeMod_removeShoes(%pl, %pl.client);
}

function GameConnection::getSavedShoes(%cl)
{
	return %cl.shoeSettings.currentShoes;
}

function GameConnection::getCurrentShoes(%cl)
{
	if (isObject(%pl = %cl.player))
	{
		return %pl.getCurrentShoes();
	}
	else
	{
		return %cl.shoeSettings.currentShoes;
	}
}

function Player::getCurrentShoes(%pl)
{
	if (isObject(%pl.lShoe))
	{
		return getShoeScriptObject(%pl.lShoe.getDatablock()).shoeName;
	}
	else if (isObject(%pl.rShoe))
	{
		return getShoeScriptObject(%pl.rShoe.getDatablock()).shoeName;
	}
	else
	{
		return 0;
	}
}

function AIPlayer::getCurrentShoes(%pl)
{
	if (isObject(%pl.lShoe))
	{
		return getShoeScriptObject(%pl.lShoe.getDatablock()).shoeName;
	}
	else if (isObject(%pl.rShoe))
	{
		return getShoeScriptObject(%pl.rShoe.getDatablock()).shoeName;
	}
	else
	{
		return 0;
	}
}

function GameConnection::exportShoeSettings(%cl)
{
	if (isObject(%cl.shoeSettings))
	{
		%cl.shoeSettings.setName("ShoeMod_ClientSettings");
		%cl.shoeSettings.save("config/server/ShoeMod/" @ %cl.bl_id @ ".cs");
		//scheduled delete just in case changing the scriptobject name then deleting triggers the hard crash bug with finding object by name
		%cl.shoeSettings.schedule(33, delete);
	}
}

function GameConnection::importShoeSettings(%cl)
{
	if (isObject(%cl.shoeSettings))
	{
		%cl.shoeSettings.delete();
	}
	%cl.shoeSettings = new ScriptObject();
	if (isFile("config/server/ShoeMod/" @ %cl.bl_id @ ".cs"))
	{
		//TODO: Replace with manual file loading as suggested by Eagle
		// exec("config/server/ShoeMod/" @ %cl.bl_id @ ".cs");
		%file = new FileObject();
		%file.openForRead("config/server/ShoeMod/" @ %cl.bl_id @ ".cs");

		while (!%file.isEOF())
		{
			%line = %file.readLine();
			if (strPos(%line, "      ") == 0) //variables start with six spaces indentation
			{
				%line = trim(%line);
				%varName = getWord(%line, 0);
				%rest = removeWord(removeWord(%line, 0), 0);
				//remove the " and ";
				%rest = getSubStr(%rest, 1, strLen(%rest) - 3);
				setObjectVariable(%cl.shoeSettings, %varName, %rest);
			}
		}

		%file.close();
		%file.delete();
	}
}

function Player::setShoes(%pl, %shoeName)
{
	%pl.wearShoes(%shoeName);
}

function AIPlayer::setShoes(%pl, %shoeName)
{
	%pl.wearShoes(%shoeName);
}

registerOutputEvent("Bot", "setShoes", "string 200 150");
registerOutputEvent("Player", "setShoes", "string 200 150");

function Player::removeShoes(%pl, %shoeName)
{
	%pl.unwearShoes(%shoeName);
}

function AIPlayer::removeShoes(%pl, %shoeName)
{
	%pl.unwearShoes(%shoeName);
}

registerOutputEvent("Bot", "removeShoes");
registerOutputEvent("Player", "removeShoes");