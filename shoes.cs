package ShoeMod_Shoes
{
	function Armor::onDisabled(%this, %obj, %state)
	{
		if (%this.keepOnDeath)
		{
			return;
		}

		ShoeMod_remountShoes(%obj);

		return parent::onDisabled(%this, %obj, %state);
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

		if (isObject(%cl.player) && isRegisteredShoe(%cl.getCurrentShoes()))
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
		%ret = parent::onClientEnterGame(%cl);

		if (isFile("config/server/ShoeMod/" @ %cl.bl_id @ ".cs"))
		{
			//TODO: Replace with manual file loading as suggested by Eagle
			exec("config/server/ShoeMod/" @ %cl.bl_id @ ".cs");
			%cl.shoeSettings = ShoeMod_ClientSettings.getID();
			%cl.shoeSettings.setName("");
		}
		return %ret;
	}

	function GameConnection::onClientLeaveGame(%cl)
	{
		%ret = parent::onClientLeaveGame(%cl);

		if (isObject(%cl.shoeSettings))
		{
			%cl.shoeSettings.setName("ShoeMod_ClientSettings");
			%cl.shoeSettings.save("config/server/ShoeMod/" @ %cl.bl_id @ ".cs");
			//scheduled delete just in case changing the scriptobject name then deleting triggers the hard crash bug with finding object by name
			%cl.shoeSettings.schedule(33, delete);
		}

		return %ret;
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

	if (isObject(%obj.rShoe))
	{
		%obj.rShoe.delete();
	}
	if (isObject(%obj.lShoe))
	{
		%obj.lShoe.delete();
	}
	if (!isObject(%obj.dummyBot))
	{
		%dummyBot = new AIPlayer(Shoes) {dataBlock = %ldb;};
		%dummyBot.kill();
		%dummyBot.setScale(%obj.getScale());
		%dummyBot.hideNode("ALL");
		%obj.mountObject(%dummyBot, 2);
		%obj.dummyBot = %dummyBot;
	}

	%rShoe = new AIPlayer(Shoes) {dataBlock = %rdb;};
	%lShoe = new AIPlayer(Shoes) {dataBlock = %ldb;};

	%rShoe.setScale(%obj.getScale());
	%lShoe.setScale(%obj.getScale());

	%rShoe.kill();
	%lShoe.kill();

	%obj.mountObject(%rShoe, 3);
	%obj.mountObject(%lShoe, 4);

	%obj.rShoe = %rShoe;
	%obj.lShoe = %lShoe;

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
}






function GameConnection::setCurrentShoes(%cl, %shoeName)
{
	if (!isRegisteredShoe(%shoeName))
	{
		return 0;
	}

	%cl.shoeSettings.currentShoes = %shoeName;
}

function GameConnection::wearShoes(%cl, %shoeName)
{
	if (!isObject(%pl = %cl.player) || !isRegisteredShoe(%shoeName))
	{
		return 0;
	}

	ShoeMod_wearShoes(%pl, %shoeName, %cl);
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