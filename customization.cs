
package ShoeMod_Customization
{
	function registerShoeScriptObjectVar(%scriptObj, %varName, %value)
	{
		if (getWord(%value, 0) $= "nodes")
		{
			for (%i = 1; %i < getWordCount(%value); %i++)
			{
				$ShoeSet.nodeTable_[getWord(%value, %i)] = 1;
			}
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

function serverCmdSetShoeColor(%cl, %node, %r, %g, %b)
{
	//default to paintcan color if no color specified
	if (%r $= "")
	{
		%color = getColorIDTable(%cl.currentColor);
		
		%r = getWord(%color, 0); %g = getWord(%color, 1); %b = getWord(%color, 2);
	}

	%cl.setShoeColor(%node, %r, %g, %b);
}

function GameConnection::setShoeColor(%cl, %node, %r, %g, %b)
{
	if (%r > 1 || %g > 1 || %b > 1)
	{
		%r /= 255; %g /= 255; %b /= 255;
	}

	%scriptObj = %cl.getCurrentShoes();

	%r = getMax(getMin(%r, 1), 0);
	%g = getMax(getMin(%g, 1), 0);
	%b = getMax(getMin(%b, 1), 0);

	%rh = intToHex(mFloor(%r + 0.5));
	%bh = intToHex(mFloor(%g + 0.5));
	%gh = intToHex(mFloor(%b + 0.5));

	%cl.ShoeMod_[%node, "Color"] = %r SPC %g SPC %b SPC 1;
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