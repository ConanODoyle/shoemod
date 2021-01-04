function registerShoeCustomizationVar(%varName)
{
	if (isFunction("RegisterPersistenceVar"))
	{
		RegisterPersistenceVar(%varName, false, "");
	}
}

function getValidPartList(%db)
{
	%scriptObj = getShoeScriptObject(%db);
	return %scriptObj.parts;
}

function isPartValid(%db, %part)
{
	%validParts = getValidPartList(%db);
	return strContainsWord(%db, %part);
}

function serverCmdSetShoeColor(%cl, %part, %r, %g, %b)
{
	//default to paintcan color if no color specified
	if (%r $= "")
	{
		%color = getColorIDTable(%cl.currentColor);
		
		%r = getWord(%color, 0); %g = getWord(%color, 1); %b = getWord(%color, 2);
	}

	if (%r > 1 || %g > 1 || %b > 1)
	{
		%r /= 255; %g /= 255; %b /= 255;
	}

	%r = getMax(getMin(%r, 1), 0);
	%g = getMax(getMin(%g, 1), 0);
	%b = getMax(getMin(%b, 1), 0);

	%rh = intToHex(mFloor(%r + 0.5));
	%bh = intToHex(mFloor(%g + 0.5));
	%gh = intToHex(mFloor(%b + 0.5));

	%cl.ShoeMod_[%part, "Color"] = %r SPC %g SPC %b SPC 1;
}