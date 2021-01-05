
package ShoeMod_Customization
{
	function registerShoeScriptObjectVar(%scriptObj, %varName, %value)
	{
		if (%varName $= "nodes")
		{
			for (%i = 0; %i < getWordCount(%value); %i++)
			{
				$ShoeSet.nodeTable_[getWord(%value, %i)] = 1;
			}
		}
		else if (%varName $= "color")
		{
			%node = getWord(%value, 0);
			%firstWord = getWord(%value, 1);
			if (%firstWord + 0 !$= %firstWord)
			{
				//is a client color name
				setObjectVariable(%node @ "DefaultColor", %firstWord);
			}
			else
			{
				//is a color vector
				setObjectVariable(%node @ "DefaultColor", clampRGBColorToPercent(getWords(%value, 2, 4)));
			}
			return; //already set variable, do not need to call parent
		}
		return parent::registerShoeScriptObjectVar(%scriptObj, %varName, %value);
	}

	function ShoeMod_wearShoes(%obj, %shoeName, %cl)
	{
		%ret = parent::ShoeMod_wearShoes(%obj, %shoeName, %cl);

		if (isObject(%obj.lShoe))
		{
			applyShoeColors(%obj.lShoe, %cl);
		}
		if (isObject(%obj.rShoe))
		{
			applyShoeColors(%obj.rShoe, %cl);
		}

		return %ret;
	}
};
activatePackage(ShoeMod_Customization);






function registerShoeCustomizationVar(%varName)
{
	if (isFunction("RegisterPersistenceVar"))
	{
		RegisterPersistenceVar(%varName, false, "");
	}
}

function getValidShoeNodeList(%db)
{
	%scriptObj = getShoeScriptObject(%db);
	return %scriptObj.nodes;
}

function isShoeNodeValid(%db, %node)
{
	%validParts = getValidShoeNodeList(%db);
	return strContainsWord(%db, %node);
}

function isShoeNodePresent(%node)
{
	return $ShoeSet.nodeTable_[%node];
}

function serverCmdSetShoeNodeColor(%cl, %node, %r, %g, %b)
{
	//default to paintcan color if no color specified
	if (%r $= "")
	{
		%color = getColorIDTable(%cl.currentColor);
		
		%r = getWord(%color, 0); %g = getWord(%color, 1); %b = getWord(%color, 2);
	}

	%cl.setShoeNodeColor(%node, %r SPC %g SPC %b);
}

function GameConnection::setShoeNodeColor(%cl, %node, %color)
{
	%color = clampRGBColorToPercent(%color);
	%hex = hexFromRGB(%color);

	%scriptObj = %cl.getCurrentShoes();

	%cl.ShoeMod_[%node, "Color"] = %color SPC 1;
}

function Player::setShoePartColor(%pl, %node, %color)
{
	if (isObject(%pl.lShoe) && %pl.lShoe.isNodeVisible(%node))
	{
		%pl.lShoe.setNodeColor(%node, %color);
	}
	if (isObject(%pl.rShoe) && %pl.rShoe.isNodeVisible(%node))
	{
		%pl.rShoe.setNodeColor(%node, %color);
	}
}

function AIPlayer::setShoePartColor(%obj, %node, %color)
{
	if (isObject(%pl.lShoe) && %pl.lShoe.isNodeVisible(%node))
	{
		%pl.lShoe.setNodeColor(%node, %color);
	}
	if (isObject(%pl.rShoe) && %pl.rShoe.isNodeVisible(%node))
	{
		%pl.rShoe.setNodeColor(%node, %color);
	}
}






function applyShoeColors(%shoeBot, %cl)
{
	
}