//has dependencies in owning.cs
package ShoeMod_Shoes
{
	function Armor::onDisabled(%this, %obj, %state)
	{
		if (%this.keepOnDeath)
		{
			return;
		}

		return parent::onDisabled(%this, %obj, %state);
	}

	function Armor::doDismount(%this, %rider, %forced)
	{
		if (%this.isShoeDB)
		{
			return;
		}
		return parent::doDismount(%this, %rider, %forced);
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

		// ShoeMod_remountShoes(%obj);
		
		return %ret;
	}

	function GameConnection::AutoAdminCheck(%cl)
	{
		if (%cl.bl_id !$= "")
		{
			%cl.importShoeSettings();
		}

		return parent::AutoAdminCheck(%cl);
	}

	function GameConnection::spawnPlayer(%cl)
	{
		%ret = parent::spawnPlayer(%cl);

		if ($Pref::Server::ShoeMod::ForceRandom || %cl.getSavedShoes() $= "Random")
		{
			%cl.wearShoes("Random");
		}
		else if (isRegisteredShoe(%cl.getSavedShoes()))
		{
			if (!$Pref::Server::ShoeMod::ShoeAccess && !%cl.ownsShoe(%cl.getSavedShoes()))
			{
				return %ret;
			}
			%cl.wearShoes(%cl.getSavedShoes());
		}

		return %ret;
	}

	function GameConnection::onDrop(%cl)
	{
		if (isObject(%cl.shoeSettings))
		{
			%cl.exportShoeSettings();
		}

		return parent::onDrop(%cl);
	}
};
activatePackage(ShoeMod_Shoes);






function getRandomShoe(%obj)
{
	%settings = isObject(%obj.shoeSettings) ? %obj.shoeSettings : %obj.client.shoeSettings;
	if (!isObject(%settings))
	{
		if ($ShoeSet.getCount() <= 0)
		{
			return "";
		}
		else
		{
			return $ShoeSet.getObject(getRandom($ShoeSet.getCount() - 1)).shoeName;
		}
	}
	else
	{
		//select from all available shoes if access is enabled
		if ($Pref::Server::ShoeMod::ShoeAccess)
		{
			return $ShoeSet.getObject(getRandom($ShoeSet.getCount() - 1)).shoeName;
		}
		else //select from shoes player owns
		{
			for (%i = 0; %i < %settings.ownedShoeListCount; %i++)
			{
				%set = %set SPC %i;
			}
			%set = trim(%set);

			while (getWordCount(%set) > 0)
			{
				%idx = getRandom(getWordCount(%set) - 1);
				%select = getWord(%set, %idx);
				%set = removeWord(%select, %idx);

				%shoeName = getField(%settings.ownedShoeList[%select], 0);
				%count = getField(%settings.ownedShoeList[%select], 1);
				if (%count <= 0)
				{
					continue;
				}
				else if (isRegisteredShoe(%shoeName))
				{
					return %shoeName;
				}
			}
		}
	}
	return "";
}

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
	if (%shoeName $= "Random")
	{
		%shoeName = getRandomShoe(%cl);
	}

	if (%shoeName $= "")
	{
		return 0;
	}

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
		if (%obj.isNodeVisible("lShoe")) { %obj.lShoe.delete(); }
		if (%obj.isNodeVisible("rShoe")) { %obj.rShoe.delete(); }
	}
	if (%scriptObj.deleteIfPeg)
	{
		if (%obj.isNodeVisible("lPeg")) { %obj.lShoe.delete(); }
		if (%obj.isNodeVisible("rPeg")) { %obj.rShoe.delete(); }
	}
	if (%scriptObj.deleteIfSkirt)
	{
		if (%obj.isNodeVisible("skirtHip")) { %obj.lShoe.delete(); %obj.rShoe.delete(); }
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
	if (!isRegisteredShoe(%shoeName) && %shoeName !$= "None" && %shoeName !$= "Random")
	{
		return 0;
	}

	%cl.shoeSettings.currentShoes = %shoeName;
}

function GameConnection::wearShoes(%cl, %shoeName)
{
	if (!isObject(%pl = %cl.player) || 
		(!isRegisteredShoe(%shoeName) && %shoeName !$= "None" && %shoeName !$= "Random"))
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
		echo("--- EXPORTING SHOE SETTINGS " @ %cl.name);
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
		echo("--- IMPORTING SHOE SETTINGS " @ %cl.name);
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
	if (%pl.getCurrentShoes() $= %shoeName)
	{
		return;
	}

	%pl.wearShoes(%shoeName);
}

function AIPlayer::setShoes(%pl, %shoeName)
{
	if (%pl.getCurrentShoes() $= %shoeName)
	{
		return;
	}
	
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






function serverCmdListShoes(%cl)
{
	if (!%cl.isAdmin)
	{
		return;
	}

	messageClient(%cl, '', "\c4List of all shoes:");
	for (%i = 0; %i < $ShoeSet.getCount(); %i++)
	{
		%subList = %subList TAB $ShoeSet.getObject(%i).shoeName;
		if (strLen(%subList) > 50)
		{
			%subList = strReplace(trim(%subList), "\t", " | ");
			messageClient(%cl, '', "\c6" @ %subList);
			%subList = "";
		}
	}
	if (strLen(%subList) > 0)
	{
		%subList = strReplace(trim(%subList), "\t", " | ");
		messageClient(%cl, '', "\c6" @ %subList);
		%subList = "";
	}
}

function serverCmdListShoe(%cl)
{
	serverCmdListShoes(%cl);
}