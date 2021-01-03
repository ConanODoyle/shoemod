if(isFile("add-ons/system_returntoblockland/server.cs")) {
	if(!$RTB::Hooks::ServerControl)
		exec("Add-Ons/System_ReturnToBlockland/hooks/ServerControl.cs");
	//					name						cat			pref							vartype			Mod					default		restart		hostOnly	callback
	RTB_registerPref("Save file name",				"Hat Mod",	"$Pref::HatMod::SaveLoc",		"string 128",	"Server_HatMod",	"HatMod",	0,			1,			"HatMod_Save");
	RTB_registerPref("Random hats",					"Hat Mod",	"$Pref::HatMod::RandomHats",	"bool",			"Server_HatMod",	1,			0,			0,			"HatMod_TickCheck");
	RTB_registerPref("Minutes between ticks",		"Hat Mod",	"$Pref::HatMod::HatTickTime",	"int 1 60",		"Server_HatMod",	10,			0,			0);
	RTB_registerPref("Chance to get hat (%)",		"Hat Mod",	"$Pref::HatMod::HatChance",		"int 1 20",		"Server_HatMod",	5,			0,			0);
	RTB_registerPref("Hat items (must restart)",	"Hat Mod",	"$Pref::HatMod::Items",			"bool",			"Server_HatMod",	1,			1,			1);
	RTB_registerPref("/giveHat command",			"Hat Mod",	"$Pref::HatMod::HatSharing",	"bool",			"Server_HatMod",	1,			0,			0);
	RTB_registerPref("/clearHats command",			"Hat Mod",	"$Pref::HatMod::ClearHats",		"bool",			"Server_HatMod",	1,			0,			0);
	RTB_registerPref("Enable duplicate hats",		"Hat Mod",	"$Pref::HatMod::DuplicateHats",	"bool",			"Server_HatMod",	1,			0,			0);
	RTB_registerPref("Access to all hats",			"Hat Mod",	"$Pref::HatMod::HatAccess",		"bool",			"Server_HatMod",	0,			0,			0,			"HatMod_TickCheck");
	RTB_registerPref("Force wear random hat",		"Hat Mod",	"$Pref::HatMod::ForceRandom",	"bool",			"Server_HatMod",	0,			0,			0,			"HatMod_ForceRandom");
} else {
	if($Pref::HatMod::SaveLoc		$= "")	$Pref::HatMod::SaveLoc = "HatMod";
	if($Pref::HatMod::RandomHats	$= "")	$Pref::HatMod::RandomHats = 1;
	if($Pref::HatMod::HatTickTime	$= "")	$Pref::HatMod::HatTickTime = 10;
	if($Pref::HatMod::HatChance		$= "")	$Pref::HatMod::HatChance = 5;
	if($Pref::HatMod::Items			$= "")	$Pref::HatMod::Items = 1;
	if($Pref::HatMod::HatSharing	$= "")	$Pref::HatMod::HatSharing = 1;
	if($Pref::HatMod::ClearHats		$= "")	$Pref::HatMod::ClearHats = 1;
	if($Pref::HatMod::DuplicateHats	$= "")	$Pref::HatMod::DuplicateHats = 1;
	if($Pref::HatMod::HatAccess		$= "")	$Pref::HatMod::HatAccess = 0;
	if($Pref::HatMod::ForceRandom	$= "")	$Pref::HatMod::ForceRandom = 0;
}

function HatMod_ForceRandom() {
	if($Pref::HatMod::ForceRandom)
		return;

	%count = ClientGroup.getCount();
	for(%i=0; %i < %count; %i++) {
		%client = ClientGroup.getObject(%i);
		serverCmdHat(%client, "random");
	}
}

function serverCmdForceRandomHats(%client) {
	if(!%client.isSuperAdmin)
		return;

	if(!$Pref::HatMod::ForceRandom)
		HatMod_ForceRandom();

	$Pref::HatMod::ForceRandom = !$Pref::HatMod::ForceRandom;

	if($Pref::HatMod::ForceRandom)
		messageAll('', "\c3" @ %client.name SPC "\c2enabled forced random hats.");
	else
		messageAll('', "\c3" @ %client.name SPC "\c2disabled forced random hats.");
}

function serverCmdHatItems(%client) {
	if(%client.isLocalConnection() && !($Server::LAN && $Server::Dedicated) && %client.getBLID() !$= getNumKeyID())
		return;

	$Pref::HatMod::Items = !$Pref::HatMod::Items;
	if($Pref::HatMod::Items)
		%client.chatMessage("\c2Enabled hat items. You will need to restart the server for this to take effect.");
	else
		%client.chatMessage("\c2Disabled hat items. You will need to restart the server for this to take effect.");
}

function serverCmdHatAccess(%client) {
	if(!%client.isSuperAdmin)
		return;

	$Pref::HatMod::HatAccess = !$Pref::HatMod::HatAccess;
	if($Pref::HatMod::HatAccess)
		messageAll('', "\c3" @ %client.name SPC "\c2enabled access to all hats.");
	else
		messageAll('', "\c3" @ %client.name SPC "\c2disabled access to all hats.");

	HatMod_TickCheck(); //This handles stuff like enabling/disabling random hat tick
}

function serverCmdHatSharing(%client) {
	if(!%client.isSuperAdmin)
		return;

	$Pref::HatMod::HatSharing = !$Pref::HatMod::HatSharing;
	if($Pref::HatMod::HatSharing)
		messageAll('', "\c3" @ %client.name SPC "\c2enabled hat sharing.");
	else
		messageAll('', "\c3" @ %client.name SPC "\c2disabled hat sharing.");
}

function serverCmdHatClearing(%client) {
	if(!%client.isSuperAdmin)
		return;

	$Pref::HatMod::ClearHats = !$Pref::HatMod::ClearHats;
	if($Pref::HatMod::ClearHats)
		messageAll('', "\c3" @ %client.name SPC "\c2enabled hat clearing.");
	else
		messageAll('', "\c3" @ %client.name SPC "\c2disabled hat clearing.");
}

function serverCmdDuplicateHats(%client) {
	if(!%client.isSuperAdmin)
		return;

	$Pref::HatMod::DuplicateHats = !$Pref::HatMod::DuplicateHats;
	if($Pref::HatMod::DuplicateHats)
		messageAll('', "\c3" @ %client.name SPC "\c2enabled being able to get duplicate hats.");
	else
		messageAll('', "\c3" @ %client.name SPC "\c2disabled being able to get duplicate hats.");
}

function serverCmdHatTime(%client, %time) {
	if(!%client.isSuperAdmin)
		return;

	if(%time == 0) {
		$Pref::HatMod::RandomHats = 0;
		cancel($HatMod::tick);
		messageAll('', "\c3" @ %client.name SPC "\c2disabled random hats.");
		return;
	}

	%time = mClamp(mFloor(%time), 1, 60);
	$Pref::HatMod::HatTickTime = %time;

	if(!$Pref::HatMod::RandomHats) {
		messageAll('', "\c3" @ %client.name SPC "\c2enabled random hats.");
		$Pref::HatMod::RandomHats = 1;
		HatMod_TickCheck();
	}

	messageAll('', "\c3" @ %client.name SPC "\c2set the Hat Mod tick time to\c3" SPC %time SPC "\c2minutes.");
}

function serverCmdHatChance(%client, %chance) {
	if(!%client.isSuperAdmin)
		return;

	if(%chance == 0) {
		$Pref::HatMod::RandomHats = 0;
		cancel($HatMod::tick);
		messageAll('', "\c3" @ %client.name SPC "\c2disabled random hats.");
		return;
	}

	%chance = mClamp(mFloor(%chance), 1, 20);
	$Pref::HatMod::HatChance = %chance;

	if(!$Pref::HatMod::RandomHats) {
		messageAll('', "\c3" @ %client.name SPC "\c2enabled random hats.");
		$Pref::HatMod::RandomHats = 1;
		HatMod_TickCheck();
	}
	
	messageAll('', "\c3" @ %client.name @ "\c2 set the Hat Mod chance per tick to\c3" SPC %chance @ "%\c2.");
}