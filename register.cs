if (!isObject($ShoeSet))
{
	$ShoeSet = new SimSet(ShoeSet);
}






function getShoeName(%directory)
{
	%start = strPos(strLwr(%directory), "add-ons/shoemod_");
	if (%start < 0)
	{
		warn ("getShoeName: ERROR - invalid directory!");
		return "";
	}
	%name = getSubStr(%directory, %start + 16, strLen(%directory));

	//check if there are subfolders and use the last folder's name
	//eg. Add-ons/ShoeMod_Cleats/lShoe.dts >> "Cleats"
	//eg. Add-ons/ShoeMod_CustomPack/Timbs/rShoe.dts >> "Timbs"
	//eg. Add-ons/ShoeMod_CustomPack/Sports/Nike/Air Jordans/lShoe.dts >> "Air Jordans"
	%lastSlash = getLastStrPos(%name, "/");
	if (%lastSlash == 0) 
	{
		//eg. Add-ons/ShoeMod_/lShoe.dts
		warn("getShoeName: ERROR - invalid shoe addon name 'ShoeMod_'!");
		%name = "";
	}
	else if (%lastSlash > 0)
	{
		%secondLastSlash = getLastStrPos(getSubStr(%name, 0, %lastSlash), "/");
		%name = getSubStr(%name, %secondLastSlash + 1, %lastSlash - %secondLastSlash - 1);
	}
	return %name;
}

function createShoeDatablock(%datablockName, %shapeFileDir)
{
	if (!isFile(%shapeFileDir))
	{
		return 0;
	}
	else if (isObject(%datablockName))
	{
		return %datablockName.getID();
	}

	%db = createPlayerDatablock(%datablockName, %shapeFileDir);

	%db.boundingBox = vectorScale("20 20 20", 4);
	%db.crouchBoundingBox = vectorScale("20 20 20", 4);
	%db.keepOnDeath = 1;

	%db.canJet = 0;
	%db.runForce = 0;
	%db.jumpForce = 0;

	%db.skipMountSound = 1; //used in lib/disableMountSound.cs

	return %db;
}

function getShoeScriptObject(%shoeName)
{
	if (isObject("Shoemod_" @ %shoeName))
	{
		("ShoeMod_" @ %shoeName).delete();
	}
	%obj = new ScriptObject("Shoemod_" @ %shoeName);
	return "Shoemod_" @ %shoeName;
}

function parseShoeSettings(%scriptObj, %directory)
{
	if (!isFile(%directory @ "settings.txt"))
	{
		warn("    No settings.txt file found, skipping...");
		return;
	}

	%file = new FileObject();
	%file.openForRead(%directory @ "settings.txt");

	while (!%file.isEOF())
	{
		%line = trim(%file.readLine());
		%varName = getWord(%line, 0);
		registerShoeScriptObjectVar(%scriptObj, %varName, getWords(%line, 1, getWordCount(%line)));
		%settings++;
	}

	%file.close();
	%file.delete();

	echo("    Parsed " @ %settings @ " settings");
	return %scriptObj;
}

function registerShoeScriptObjectVar(%scriptObj, %varname, %value)
{
	%varname = getSafeVariableName(%varname);
	setObjectVariable(%scriptObj, %varname, %value);
}










function ShoeMod_registerShoe(%directory, %shoeName)
{
	%lShoeDTS = %directory @ "lShoe.dts";
	%rShoeDTS = %directory @ "rShoe.dts";

	if (!(%lShoePresent = isFile(%lShoeDTS)) || !(%rShoePresent = isFile(%rShoeDTS)))
	{
		if (!%lShoePresent)
		{
			%missing = "lShoe.dts";
		}
		if (!%rShoePresent)
		{
			%missing = trim(%missing SPC "rShoe.dts");
		}
		warn("Shoemod_registerShoe: ERROR - " @ %directory @ " is missing " @ strReplace(%missing, " ", " and ") @ "!");
		return 0;
	}

	%lDBName = getSafeVariableName(%shoeName) @ "LShoeArmor";
	%rDBName = getSafeVariableName(%shoeName) @ "RShoeArmor";

	if (isObject(%lDBName)) { echo("    datablock " @ %lDBName @ " already exists, skipping..."); }
	else { createShoeDatablock(%lDBName, %lShoeDTS); }

	if (isObject(%rDBName)) { echo("    datablock " @ %rDBName @ " already exists, skipping..."); }
	else { createShoeDatablock(%rDBName, %rShoeDTS); }

	%scriptObj = getShoeScriptObject(getSafeVariableName(%shoeName));
	parseShoeSettings(%scriptObj, %directory);

	%scriptObj.lShoeDB = %lDBName;
	%scriptObj.rShoeDB = %rDBName;
	%scriptObj.shoeName = %shoeName;

	$ShoeSet.add(%scriptObj);

	return %scriptObj;
}

function ShoeMod_clearSearchPatterns()
{
	deleteVariables("$Shoemod::SearchPattern*");
	$ShoeMod::SearchPatternCount = 0;
}

function ShoeMod_registerSearchPattern(%string)
{
	for (%i = 0; %i < $ShoeMod::SearchPatternCount; %i++)
	{
		if ($ShoeMod::SearchPattern[%i] $= %string)
		{
			warn("ShoeMod_registerSearchPattern: ERROR - search pattern already registered!");
			return;
		}
	}

	$ShoeMod::SearchPattern[$ShoeMod::SearchPatternCount] = %string;
	$ShoeMod::SearchPatternCount++;

	echo("Registered ShoeMod search pattern \"" @ %string @ "\"");
}

function ShoeMod_registerAllShoes()
{
	echo("Registering shoes...");
	ShoeMod_registerSearchPattern("Add-ons/ShoeMod_*/*.dts");
	ShoeMod_registerSearchPattern("Add-ons/ShoeMod_*/*/*.dts");

	$ShoeSet.clear();

	for (%i = 0; %i < $ShoeMod::SearchPatternCount; %i++)
	{
		%pattern = $ShoeMod::SearchPattern[%i];
		%checkForEnabled = (getSubStr(%pattern, 0, 16) $= "Add-ons/ShoeMod_");

		for (%dir = findFirstfile(%pattern); %dir !$= ""; %dir = findNextFile(%pattern))
		{
			%lastSlash = getLastStrPos(%dir, "/");
			
			%directory = getSubStr(%dir, 0, %lastSlash + 1);
			%shoeName = getShoeName(%directory);
			%safeShoeName = getSafeVariableName(%shoeName);

			//check if already added
			if (%checkedShoeName[%safeShoeName])
			{
				continue;
			}

			//check if enabled, if it is a normal shoemod
			if (%checkForEnabled)
			{
				%addonName = getSubStr(%dir, 8, strPos(%dir, "/", 9) - 8);
				if ($ADDON__[%addonName] < 0)
				{
					if (!%echo[%addonName])
					{
						echo("Skipping registering " @ %addonName @ " - add-on is disabled");
					}
					%echo[%addonName] = 1;
					continue;
				}
			}

			%checkedShoeName[%safeShoeName] = 1;
			ShoeMod_registerShoe(%directory, %shoeName);
		}
	}
}

schedule(1, 0, ShoeMod_registerAllShoes);