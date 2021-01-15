if (!isObject($ShoeSet))
{
	$ShoeSet = new SimSet(ShoeSet);
}

package ShoeSetDestroy
{
	function destroyServer ()
	{
		deleteShoeSet();
		return parent::destroyServer();
	}
};
activatePackage(ShoeSetDestroy);






function deleteShoeSet()
{
	if (isObject($ShoeSet))
	{
		$ShoeSet.deleteAll();
		$ShoeSet.delete();
	}
}

function getShoeName(%directory)
{
	%start = strPos(strLwr(%directory), "add-ons/shoemod_");
	if (%start == 0)
	{
		%name = getSubStr(%directory, %start + 16, strLen(%directory));
	}
	else
	{
		%name = getSubStr(%directory, strPos(%directory, "_"), strLen(%directory));
	}

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

	%db.isShoeDB = 1;

	%db.canJet = 0;
	%db.runForce = 0;
	%db.jumpForce = 0;

	%db.skipMountSound = 1; //used in lib/disableMountSound.cs

	return %db;
}

function getShoeScriptObject(%shoeName)
{
	//support passing in a shoe datablock
	if (isObject(%shoeName) && %shoeName.getClassName() $= "PlayerData")
	{
		return %shoeName.shoeScriptObj;
	}
	
	%safeShoeName = getSafeVariableName(%shoeName);
	
	if (isObject($ShoeSet.shoeTable_[%safeShoeName]))
	{
		return $ShoeSet.shoeTable_[%safeShoeName];	
	}
	else if (!isObject("ShoeMod_" @ %safeShoeName @ "Data"))
	{
		return new ScriptObject("ShoeMod_" @ %safeShoeName @ "Data");
	}
	return ("ShoeMod_" @ %safeShoeName @ "Data").getID();
}

function parseShoeSettings(%scriptObj, %directory)
{
	if (!isFile(%directory @ "settings.txt"))
	{
		warn("            No settings.txt file found, skipping...");
		return 0;
	}

	%file = new FileObject();
	%file.openForRead(%directory @ "settings.txt");

	while (!%file.isEOF())
	{
		%line = trim(%file.readLine());
		%varName = getSafeVariableName(getWord(%line, 0));
		%rest = getWords(%line, 1, getWordCount(%line));
		if (%varName !$= "" && %rest !$= "")
		{
			registerShoeScriptObjectVar(%scriptObj, %varName, %rest);
		}
		%settings++;
	}

	%file.close();
	%file.delete();

	echo("            Parsed " @ %settings @ " settings");
	return %scriptObj;
}

function registerShoeScriptObjectVar(%scriptObj, %varname, %value)
{
	setObjectVariable(%scriptObj, %varname, %value);
}






function isRegisteredShoe(%shoeName)
{
	return isObject($ShoeSet.shoeTable_[getSafeVariableName(%shoeName)])
		|| isObject("ShoeMod_" @ getSafeVariableName(%shoeName));
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
		warn("ShoeMod_registerShoe: ERROR - " @ %directory @ " is missing " @ strReplace(%missing, " ", " and ") @ "!");
		return 0;
	}

	%safeShoeName = getSafeVariableName(%shoeName);

	%lDBName = %safeShoeName @ "LShoeArmor";
	%rDBName = %safeShoeName @ "RShoeArmor";

	if (isObject(%lDBName)) { echo("    datablock " @ %lDBName @ " already exists, skipping..."); }
	else { createShoeDatablock(%lDBName, %lShoeDTS); }

	if (isObject(%rDBName)) { echo("    datablock " @ %rDBName @ " already exists, skipping..."); }
	else { createShoeDatablock(%rDBName, %rShoeDTS); }

	%scriptObj = ShoeMod_registerShoeSettings(%directory, %shoeName);

	%scriptObj.lShoeDB = %lDBName;
	%scriptObj.rShoeDB = %rDBName;
	%lDBName.shoeScriptObj = %scriptObj;
	%rDBName.shoeScriptObj = %scriptObj;

	$ShoeSet.add(%scriptObj);
	$ShoeSet.shoeTable_[%safeShoeName] = %scriptObj;

	return %scriptObj;
}

function ShoeMod_registerShoeSettings(%directory, %shoeName)
{
	%safeShoeName = getSafeVariableName(%shoeName);
	%scriptObj = getShoeScriptObject(%safeShoeName);

	%scriptObj.shoeName = %shoeName;
	%scriptObj.safeShoeName = %safeShoeName;
	%scriptObj.directory = %directory;

	parseShoeSettings(%scriptObj, %directory);

	if (!$ShoeSet.isMember(%scriptObj))
	{
		$ShoeSet.add(%scriptObj);
	}
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

	$ShoeMod::SearchPattern[$ShoeMod::SearchPatternCount + 0] = %string;
	$ShoeMod::SearchPatternCount++;

	echo("Registered ShoeMod search pattern \"" @ %string @ "\"");
}

function ShoeMod_registerAllShoes()
{
	echo("Registering shoes...");
	ShoeMod_registerSearchPattern("Add-ons/ShoeMod_*/*.dts");
	// ShoeMod_registerSearchPattern("Add-ons/ShoeMod_*/*/*.dts"); //first pattern already covers cases of multiple slashes

	$ShoeSet.clear();

	for (%i = 0; %i < $ShoeMod::SearchPatternCount; %i++)
	{
		%pattern = $ShoeMod::SearchPattern[%i];
		%checkForEnabled = (getSubStr(%pattern, 0, 16) $= "Add-ons/ShoeMod_");

		echo("Checking pattern: " @ %pattern);
		if (%checkForEnabled)
		{
			echo("    Add-on must be enabled to be registered");
		}
		for (%dir = findFirstfile(%pattern); %dir !$= ""; %dir = findNextFile(%pattern))
		{
			%lastSlash = getLastStrPos(%dir, "/");
			
			%directory = getSubStr(%dir, 0, %lastSlash + 1);
			%shoeName = getShoeName(%directory);
			%safeShoeName = getSafeVariableName(%shoeName);

			//check if already added/checked
			if (%visitedDirectory[getSafeVariableName(%directory)])
			{
				continue;
			}
			%visitedDirectory[getSafeVariableName(%directory)] = 1;

			if (%checkForEnabled)
			{
				%addonName = getSubStr(%dir, 8, strPos(%dir, "/", 9) - 8);
				if ($ADDON__[%addonName] <= 0)
				{
					if (!%echo[%addonName])
					{
						echo("            Skipping registering " @ %addonName @ " - add-on is disabled");
					}
					%echo[%addonName] = 1;
					continue;
				}
			}

			echo("        Registering '" @ %shoeName @ "' in " @ %directory);
			// echo("    Checking directory: " @ %directory @ " file: " @ %dir);
			if (isRegisteredShoe(%shoeName))
			{
				echo("            Already registered '" @ %shoeName @ "'! Updating settings...");
				//re-register shoe settings only, in case this is manually called to update shoes
				ShoeMod_registerShoeSettings(%directory, %shoeName);
				continue;
			}

			//check if enabled, if it is a normal shoemod
			
			ShoeMod_registerShoe(%directory, %shoeName);
			%registeredCount++;
		}
	}
	echo("Registered " @ %registeredCount + 0 @ " new shoes");
}

schedule(1, 0, ShoeMod_registerAllShoes);