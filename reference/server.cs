exec("./version.cs");

function HatMod_save() {
	//Strip illegal chars (according to win7)
	//If they still manage to fuck something up, its their own fault. Its host only.
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "/", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "\\", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "\"", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, ":", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "*", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "?", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "|", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, "<", "");
	$Pref::HatMod::SaveLoc = strReplace($Pref::HatMod::SaveLoc, ">", "");

	export("$HatMod::save::*", "Config/Server/" @ $Pref::HatMod::SaveLoc @ ".cs");
}

////////////////////////////////////////////
///////////////Tick Functions///////////////
////////////////////////////////////////////

function HatMod_canTick() {
	return $Pref::HatMod::RandomHats && $Pref::HatMod::HatTickTime > 0 && $Pref::HatMod::HatChance > 0 && !$Pref::HatMod::HatAccess;
}

function HatMod_TickCheck() {
	if(!HatMod_canTick()) {
		cancel($HatMod::tick);
		return;
	}

	if(isEventPending($HatMod::tick))
		return;


	//cancel($HatMod::tick);
	$HatMod::tick = schedule($Pref::HatMod::HatTickTime * 60 * 1000, 0, HatMod_tick);
}

function HatMod_tick() {
	cancel($HatMod::tick);

	if(!HatMod_canTick())
		return;

	%count = ClientGroup.getCount();
	for(%i=0; %i < %count; %i++) {
		%client = ClientGroup.getObject(%i);

		%hat = "";

		if(isObject(%player = %client.player) && %client.HatModNotAFK !$= %player.getVelocity() && getRandom(0, 100) <= $Pref::HatMod::HatChance) {
			%breaker = 0;
			while(($Pref::HatMod::DuplicateHats || $HatMod::save::hats[%client.bl_id, strReplace(%hat, " ", "_")] <= 0) && !isHat(%hat)) {
				%hat = getRandom(1, HatMod_HatSet.getCount());
				%hat = HatMod_HatSet.getObject(%i).hatName;

				if(%breaker++ > 100)
					break;
			}
			if(%breaker > 100)
				continue;

			if(trim(%hat) $= "" || !isHat(%hat))
				continue; //Just in case

			HatMod_addHat(%client, %client.bl_id, %hat, 1);

			for(%j=0; %j < %count; %j++) {
				%c = ClientGroup.getObject(%j);
				if(%c != %client)
					%c.chatMessage("\c3" @ %client.name SPC "\c2found the\c3" SPC %hat SPC "\c2hat!");
			}
			//Schedule to make sure it always appears after everyone else
			%client.schedule(0, chatMessage, "\c2You found the\c3" SPC %hat SPC "\c2hat!");
		}

		if(isObject(%player))
			%client.HatModNotAFK = %player.getVelocity(); //Set in package.cs
		else
			%client.HatModNotAFK = 0;
	}

	$HatMod::tick = schedule($Pref::HatMod::HatTickTime * 60 * 1000, 0, HatMod_tick);
}

////////////////////////////////////////////
/////////////////Modularity/////////////////
////////////////////////////////////////////

function isHat(%hat) {
	return isObject(strReplace("Hat" @ %hat @ "Data", " ", "_"));
}

function HatMod_addHat(%client, %bl_id, %hat, %amount) {
	if(isObject(%client)) {
		if(strLen(%bl_id))
			%id = %bl_id;
		else
			%id = %client.bl_id;
	} else {
		if(strLen(%bl_id))
			%id = %bl_id;
		else
			return;
	}

	%hat = strReplace(%hat, " ", "_");
	$HatMod::save::hats[%id, %hat] += %amount;
	if($HatMod::save::hats[%id, %hat] < 0)
		$HatMod::save::hats[%id, %hat] = 0;
	HatMod_save();

	if(isObject(%client) && $BRPG::save::wornHat[%client.bl_id] == %hat && !%client.hasHat(%hat)) {
		%client.unmountHat();
	}
}

function GameConnection::hasHat(%client, %hat) {
	%hat = strReplace(%hat, " ", "_");
	if(isHat(%hat) && ($HatMod::save::hats[%client.bl_id, %hat] >= 1 || $Pref::HatMod::HatAccess))
		return 1;
	return 0;
}

function HatMod_getRandomHat() {
	return HatMod_HatSet.getObject(getRandom(0, HatMod_HatSet.getCount()-1)).hatName;
}

function GameConnection::getRandomHat(%client) {
	for(%hatsDone=0; %hatsDone < HatMod_HatSet.getCount(); %hatsDone++) {
		%hat = HatMod_getRandomHat();

		if(%hasTried[strReplace(%hat, " ", "_")]) {
			%hatsDone--;
			continue;
		}

		if(%client.hasHat(%hat)) {
			%hatFound = 1;
			break;
		}

		%hasTried[strReplace(%hat, " ", "_")] = 1;

		if(%breaker++ > 150)
			return 0;
	}
	if(!%hatFound)
		return 0;
	return %hat;
}

function Player::wearRandomHat(%player) {
	if(isObject(%client = %player.client)) {
		%hat = %client.getRandomHat();
		if(!isHat(%hat))
			return 0;
	} else
		%hat = HatMod_getRandomHat();

	%player.mountHat(%hat);
}

function Player::mountHat(%player, %hat) {
	if(!isHat(%hat)) {
		error("Error: Player::mountHat: Hat does not exist!" SPC %player SPC %hat);
		return;
	}

	%player.mountImage(("Hat" @ strReplace(%hat, " ", "_") @ "Data"), 2);

	//Hide all hat and accent nodes
	for(%i=0; $hat[%i] !$= ""; %i++)
		%player.hideNode($hat[%i]);

	for(%i=0; $accent[%i] !$= ""; %i++)
		%player.hideNode($accent[%i]);
}

function Player::removeHat(%player) {
	%player.unmountImage(2);
	if(isObject(%client = %player.client)) {
		%client.applyBodyParts();
		%client.applyBodyColors();
	}
}

function GameConnection::clearAllHats(%client) {
	deleteVariables("$HatMod::save::hats" @ %client.bl_id @ "_*");
	HatMod_save();
	%client.unmountHat();
}

function GameConnection::unmountHat(%client) {
	$HatMod::save::wornHat[%client.bl_id] = "";
	if(isObject(%player = %client.player))
		%player.removeHat();
}

////////////////////////////////////////////
/////////////Hat registeration//////////////
////////////////////////////////////////////

function HatMod_registerHat(%name, %dir, %offset, %eyeOffset) {
	if(isHat(%name)) {
		echo("Error: Hat already exists! (" @ strReplace(%name, "_", " ") @ ")");
		HatMod_HatSet.add("Hat" @ %name @ "Data");
		return 0;
	}

	%evalString =	"datablock ShapeBaseImageData(Hat" @ %name @ "Data) {" SPC
						"shapeFile = \"" @ %dir @ "\";" SPC
						"mountPoint = $HeadSlot;" SPC
						"offset = \"" @ %offset @ "\";" SPC
						"eyeOffset = \"" @ %eyeOffset @ "\";" SPC
						"rotation = \"" @ eulerToMatrix("0 0 0") @ "\";" SPC
						"scale = \"0.1 0.1 0.1\";" SPC
						"doColorShift = false;" SPC
						"hatName = \"" @ %name @ "\";" SPC
					"};";

	eval(%evalString);
	HatMod_HatSet.add("Hat" @ %name @ "Data");

	if($Pref::HatMod::Items) {
		%evalString = 	"datablock ItemData(Hat" @ %name @ "ItemData) {" SPC
							"category = \"Hat\";" SPC
							"className = \"Hat\";" SPC
							"shapeFile = \"" @ %dir @ "\";" SPC
							"rotate = false;" SPC
							"mass = 1;" SPC
							"density = 0.2;" SPC
							"elasticity = 0.2;" SPC
							"friction = 0.6;" SPC
							"emap = true;" SPC
							"uiName = \"Hat" SPC strReplace(%name, "_", " ") @ "\";" SPC
							"iconName = \"Add-ons/Weapon_Bow/icon_bow\";" SPC
							"doColorShift = false;" SPC
							"image = Hat" @ %name @ "Data;" SPC
							"canDrop = true;" SPC
						"};";

		eval(%evalString); //We dont need to store these
	}

	return 1;
}

function HatMod_registerAllHats() {
	echo("Searching for Hats...");

	if(isObject(HatMod_HatSet))
		HatMod_HatSet.clear();

	%loc = "Add-ons/HatMod_*/*/*.dts";
	for(%dir = findFirstFile(%loc); %dir !$= ""; %dir = findNextFile(%loc)) {
		//echo("DIR:" SPC %dir);
		%pos1 = strLastPos(%dir, "/")-1;
		%pos2 = strLastPos(getSubStr(%dir, 0, %pos1), "/");
		%name = getSubStr(%dir, 1+%pos2, %pos1-%pos2);
		//echo("NAME:" SPC %name);

		//echo("   Found hat:" SPC strReplace(%name, "_", " ") SPC "at dir" SPC %dir);

		%configDir = getSubStr(%dir, 0, strLastPos(%dir, "/")+1) @ %name @ ".txt";
		if(isFile(%configDir)) {
			echo("   Reading file for calibration.");

			%config = HatMod_GetProperties(%configDir);
		} else
			%config = HatMod_GetProperties("Default");

		%offset = getField(%config, 0);
		%minVer = getField(%config, 1);
		%eyeOffset = getField(%config, 2);

		if(%minVer > $HatMod::CurrentVersion) {
			echo("");
			warn("ERROR: Hat requires update to HatMod! (Required version" SPC %minVer @ ", current version" SPC $HatMod::CurrentVersion @ ")");
			echo("");
			continue;
		}

		if(HatMod_registerHat(%name, %dir, %offset, %eyeOffset)) {
			echo("   Hat" SPC strReplace(%name, "_", " ") SPC "registered successfully.");
			%count++;
		}
	}

	%loc = "Add-ons/HatMod_*/*.dts";
	for(%dir = findFirstFile(%loc); %dir !$= ""; %dir = findNextFile(%loc)) {
		//echo("DIR:" SPC %dir);
		if(strPosCount(%dir, "/") != 2) //This filters out any that are Add-ons/HatMod_Whatever/Folder/File.dts
			continue;

		%name = getSubStr(%dir, 1+(%pos = strLastPos(%dir, "/")), strLen(%dir)-%pos-5);
		//echo("NAME:" SPC %name);

		//echo("   Found hat:" SPC strReplace(%name, "_", " ") SPC "at dir" SPC %dir);

		%configDir = getSubStr(%dir, 0, strLastPos(%dir, "/")+1) @ %name @ ".txt";
		if(isFile(%configDir)) {
			echo("   Reading file for calibration.");

			%config = HatMod_GetProperties(%configDir);
		} else
			%config = HatMod_GetProperties("Default");

		%offset = getField(%config, 0);
		%minVer = getField(%config, 1);
		%eyeOffset = getField(%config, 2);

		if(%minVer > $HatMod::CurrentVersion) {
			echo("");
			warn("ERROR: Hat requires update to HatMod! (Required version" SPC %minVer @ ", current version" SPC $HatMod::CurrentVersion @ ")");
			echo("");
			continue;
		}

		if(HatMod_registerHat(%name, %dir, %offset, %eyeOffset)) {
			echo("   Hat" SPC strReplace(%name, "_", " ") SPC "registered successfully.");
			%count++;
		}
	}

	%loc = "Add-ons/SimpleWell_*/ITEMS/SHAPES/*.dts";
	for(%dir = findFirstFile(%loc); %dir !$= ""; %dir = findNextFile(%loc)) {
		//echo("DIR:" SPC %dir);
		%name = getSubStr(%dir, 1+(%pos = strLastPos(%dir, "/")), strLen(%dir)-%pos-5);
		//echo("NAME:" SPC %name);

		//echo("   Found SW hat:" SPC strReplace(%name, "_", " ") SPC "at dir" SPC %dir);

		%configDir = getSubStr(%dir, 0, strLastPos(%dir, "/")-6) @ %name @ ".cs";

		if(isFile(%configDir)) {
			echo("   Reading file for calibration.");

			%config = HatMod_GetProperties(%configDir);
		} else
			%config = HatMod_GetProperties("Default");

		%offset = getField(%config, 0);
		%minVer = getField(%config, 1);
		%eyeOffset = getField(%config, 2);

		if(%minVer > $HatMod::CurrentVersion) {
			echo("");
			warn("ERROR: Hat requires update to HatMod! (Required version" SPC %minVer @ ", current version" SPC $HatMod::CurrentVersion @ ")");
			echo("");
			continue;
		}

		if(HatMod_registerHat(%name, %dir, %offset, %eyeOffset)) {
			echo("   Hat" SPC strReplace(%name, "_", " ") SPC "registered successfully.");
			%count++;
		}
	}

	echo("Hat search done. Found" SPC %count+0 SPC "new hats.");
}

function HatMod_GetProperties(%configDir) {
	%offset = "0 0 0";
	%minVer = "0";
	%eyeOffset = "0 0 -1000";

	if(%configDir !$= "Default") {
		%file = new fileObject();
		%file.openForRead(%configDir);

		while(!%file.isEOF()) {
			%line = trim(%file.readLine());

			if(%line $= "")
				continue;

			%firstWord = getWord(%line, 0);
			%wordCount = getWordCount(%line);

			switch$(%firstWord) {
			case "offset":
				%offset = getWords(%line, %wordCount-3, %wordCount-1);
				%offset = strReplace(%offset, "\"", "");
				%offset = strReplace(%offset, ";", "");
				%offset = vectorAdd(%offset, "0 0 0"); //Forces it to be a 3d vector

			case "minVer":
				%minVer = getWord(%line, getWordCount(%line)-1);

			case "eyeOffset":
				%eyeOffset = getWords(%line, %wordCount-3, %wordCount-1);
				%eyeOffset = strReplace(%eyeOffset, "\"", "");
				%eyeOffset = strReplace(%eyeOffset, ";", "");
				%eyeOffset = vectorAdd(%eyeOffset, "0 0 0"); //Forces it to be a 3d vector
			}
		}

		%file.close();
		%file.delete();
	}

	return %offset TAB %minVer TAB %eyeOffset;
}

////////////////////////////////////////////
/////////////////Resources//////////////////
////////////////////////////////////////////

function strLastPos(%str, %search, %offset) {
	if(%offset > 0)
		%pos = %offset;
	else {
		%str = getSubStr(%str, 0, strLen(%str) + %offset);
		%pos = 0;
	}
	%lastPos = -1;
	while((%pos = strPos(%str, %search, %pos+1)) > 0) {
		%lastPos = %pos;
		if(%break++ >= 500) //Pretty sure strings have a max length shorter than this, should be fine
			return -2;
	}
	return %lastPos;
}

function strPosCount(%str, %search, %offset) {
	if(getSubStr(%str, %offset, strLen(%search)) $= %search) //Manual search for the first one
		%count = 1;

	if(%offset $= "")
		%offset = 0;

	%pos = %offset;

	while((%pos = strPos(%str, %search, %pos+1)) > 0)
		%count++;
	return %count;
}

////////////////////////////////////////////
//////////////Update Detector///////////////
////////////////////////////////////////////

function serverCmdHatMod_ReturnVersion(%client, %version) {
	%client.hatModLatestVersion = %version;

	if(%version - $HatMod::CurrentVersion < 3 && %version > $HatMod::LatestVersion && mFloatLength(%version, 2)+0 $= %version) {
		%count = ClientGroup.getCount();
		for(%i=0; %i < %count; %i++) {
			%c = ClientGroup.getObject(%i);
			if(%c == %client)
				continue;
			if(%client.hatModLatestVersion $= "") //Chances are, they dont have HatMod, dont bother telling them to update
				continue;
			if(%version <= %client.hatModLatestVersion)
				continue;
			if(%c.isLocalConnection()) { //Is hosting server (non-dedi only)
				//Copy pasted from client.cs, should probably make it a function
				if(!$HatMod::HasMessagedUpdate) {
					clientCmdMessageBoxOk("HatMod is outdated!", "It has been detected that you do not have the latest version of HatMod!<br>Check the topic <a:forum.blockland.us/index.php?topic=254646.0>here</a>.");
					$HatMod::HasMessagedUpdate = 1;
				}
				continue;
			}
			commandToClient(%c, 'HatMod_CheckVersion', %version); //Even if the message isnt displayed, we want the clients to know what the latest version is
		}
		$HatMod::LatestVersion = %version;
	}

}





exec("./prefs.cs");

if(isFile("Config/Server/" @ $Pref::HatMod::SaveLoc @ ".cs"))
	exec("Config/Server/" @ $Pref::HatMod::SaveLoc @ ".cs");

exec("./package.cs");
exec("./commands.cs");
exec("./events.cs");



if(!isObject(HatMod_HatSet)) {
	new SimSet(HatMod_HatSet);
	missionCleanup.add(HatMod_HatSet);
}
if(!$HatMod::Debug) //Just something so i can reload the addon without re-registering all the hats.
	HatMod_registerAllHats();


//This has to be executed after the hats are registered
if($AddOn__Event_Variables)
	exec("./vce.cs");