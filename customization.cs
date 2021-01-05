
package ShoeMod_Customization
{
	function registerShoeScriptObjectVar(%scriptObj, %varName, %value)
	{
		if (%varName $= "color")
		{
			%node = getWord(%value, 0);
			%firstWord = getWord(%value, 1);
			if (%firstWord + 0 !$= %firstWord)
			{
				//is a client color name
				setObjectVariable("DefaultColor_" @ %node, %firstWord);
			}
			else
			{
				//is a color vector
				setObjectVariable("DefaultColor_" @ %node, clampRGBColorToPercent(getWords(%value, 2, 4)));
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
			applyShoeColors(%obj.lShoe, %shoeName, %cl);
		}
		if (isObject(%obj.rShoe))
		{
			applyShoeColors(%obj.rShoe, %shoeName, %cl);
		}

		return %ret;
	}
};
activatePackage(ShoeMod_Customization);






function getValidShoeNodeList(%shoeName)
{
	%scriptObj = getShoeScriptObject(%shoeName);
	return %scriptObj.nodes;
}

function isShoeNodeValid(%shoeName, %node)
{
	%validParts = getValidShoeNodeList(%shoeName);
	return strContainsWord(%validParts, %node);
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
	%cl.saveShoeNodeColor(%node, %r SPC %g SPC %b);
}

function GameConnection::saveShoeNodeColor(%cl, %shoeName, %node, %color)
{
	%cl.shoeSettings.ShoeMod_[getSafeVariableName(%shoeName), %node, "Color"] = %color;
}

function GameConnection::loadShoeNodeColor(%cl, %shoeName, %node)
{
	%clientColors = "accentColor" SPC 
					"chestColor" SPC 
					"hatColor" SPC 
					"headColor" SPC 
					"hipColor" SPC 
					"larm" SPC 
					"larmColor" SPC 
					"lhandColor" SPC 
					"llegColor" SPC 
					"packColor" SPC 
					"rarmColor" SPC 
					"rhandColor" SPC 
					"rlegColor" SPC 
					"secondPackColor";
	%color = %cl.shoeSettings.ShoeMod_[getSafeVariableName(%shoeName), %node, "Color"];
	if (getWordCount(%color) == 1 && strContainsWord(%clientColors, %color))
	{
		%color = getObjectVariable(%cl, %color);
	}
	return getWords(%color, 0, 2);
}

function GameConnection::setShoeNodeColor(%cl, %node, %color)
{
	%color = clampRGBColorToPercent(%color);
	%hex = hexFromRGB(%color);

	if (isObject(%pl = %cl.player))
	{
		%pl.setShoeNodeColor(%node, %color);
	}
}

function Player::setShoeNodeColor(%pl, %node, %color)
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

function AIPlayer::setShoeNodeColor(%obj, %node, %color)
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






function applyShoeColors(%shoeBot, %shoeName, %cl)
{
	%nodeList = getValidShoeNodeList(%shoeName);
	%scriptObj = getShoeScriptObject(%shoeName);

	for (%i = 0; %i < getWordCount(%nodeList); %i++)
	{
		%node = getWord(%nodeList, %i);
		%color = %cl.loadShoeNodeColor(%shoeName, %node);
		if (%color $= "")
		{
			%color = "1 1 1";
		}
		if (%shoeBot.isNodeVisible(%node))
		{
			%shoeBot.setNodeColor(%node, %color SPC 1);
		}
	}

	if (%scriptObj.hasTransparency !$= "")
	{
		%shoeBot.startFade(0, 0, %scriptObj.hasTransparency);
	}
}

function setShoeDefaultColors(%cl, %shoeName)
{
	%nodeList = getValidShoeNodeList(%shoeName);
	%scriptObj = getShoeScriptObject(%shoeName);
	
	for (%i = 0; %i < getWordCount(%nodeList); %i++)
	{
		%node = getWord(%nodeList, %i);
		if ((%color = %scriptObj.DefaultColor_[%node]) !$= ""
			&& %cl.loadShoeNodeColor(%shoeName, %node) $= "")
		{
			%cl.saveShoeNodeColor(%shoeName, %node, %color);
		}
	}
}

function resetShoeColors(%cl, %shoeName)
{
	%nodeList = getValidShoeNodeList(%shoeName);
	%scriptObj = getShoeScriptObject(%shoeName);
	
	for (%i = 0; %i < getWordCount(%nodeList); %i++)
	{
		%node = getWord(%nodeList, %i);
		if ((%color = %scriptObj.DefaultColor_[%node]) !$= "")
		{
			%cl.saveShoeNodeColor(%shoeName, %node, %color);
		}
		else
		{
			%cl.saveShoeNodeColor(%shoeName, %node, "");
		}
	}
}