function serverCmdListHats(%client) {
	%client.chatMessage("\c6     Registered Hats");

	%hatList = "\c6   None  \c7|\c6 ";
	%count = 1;
	for(%i=0; %i < HatMod_HatSet.getCount(); %i++) {
		%hat = HatMod_HatSet.getObject(%i).hatName;

		%hatList = trim(%hatList SPC %hat);
		%hatList = %hatList SPC " \c7|\c6 ";

		%count++;
		if(%count == 4) {
			%client.chatMessage(trim(getWords(%hatList, 0, getWordCount(%hatList)-2)));
			%hatList = "\c6  ";
			%count = 0;
		}
	}
	if(%hatList !$= "\c6  ") {
		%client.chatMessage(trim(getWords(%hatList, 0, getWordCount(%hatList)-2)));
	}
}

function serverCmdHatInfo(%client) {
	%client.chatMessage("\c6HatMod: By Boodals and JakeBlade");
	%client.chatMessage("\c6Server version: v" @ $HatMod::CurrentVersion);
	%client.chatMessage("\c6Latest version according to clients: v" @ $HatMod::LatestVersion);
	%client.chatMessage("\c6There are" SPC HatMod_HatSet.getCount() SPC "registered hats.");
}

function serverCmdHatHelp(%client) {
	%client.chatMessage("\c3/hat (Hat name)\c6: If \c3Hat name\c6 is given, and you have it, wear the hat. If \c3Hat name\c6 isn't given, then list all of your hats.");
	%client.chatMessage("\c3/giveHat Target Hat name\c6: Gives \c3Target\c6 one of your \c3Hat\c6s.");
	%client.chatMessage("\c3/listHats\c6: Lists all registed hats.");


	if(%client.isSuperAdmin) {
		%client.chatMessage("\c0SA: /grantHat Target Hat name\c6: Grants hat to \c3Target\c6.");
		%client.chatMessage("\c0SA: /ClearHats (Target)\c6: Clears all hats of \c3Target\c6, or yourself if \c3Target\c6 isn't given.");

		//Prefs
		%client.chatMessage("\c0SA: /HatTime Time\c6: Set time inbetween ticks in minutes, 5 - 60. Set to 0 to disable.");
		%client.chatMessage("\c0SA: /HatChance Time\c6: Set percent chance per tick for a client to get a random hat, 1% - 10%. Set to 0 to disable.");
		%client.chatMessage("\c0SA: /HatSharing\c6: Toggles the /giveHat command.");
		%client.chatMessage("\c0SA: /HatClearing\c6: Toggles the /clearHats command for non super admins.");
		%client.chatMessage("\c0SA: /DuplicateHats\c6: Toggles if you can have more than one of each hat.");
		
	} else {
		%client.chatMessage("\c3/ClearHats\c6: Clears all of your hats.");
	}
}

function serverCmdGrantHat(%client, %target, %na, %nb, %nc, %nd, %ne) {
	if(!%client.isSuperAdmin)
		return;

	if($Pref::HatMod::HatAccess) {
		%client.chatMessage("\c2Hat access is on. There is no need to grant hats.");
		return;
	}

	%hatName = trim(%na SPC %nb SPC %nc SPC %nd SPC %ne);

	%target = findClientByName(%target);
	if(!isObject(%target)) {
		%client.chatMessage("\c2Could not find a client with that name.");
		return;
	}

	if(%hatName $= "rand" || %hatName $= "random") {
		%hatName = HatMod_getRandomHat();
	}

	if(!isHat(%hatName)) {
		%client.chatMessage("\c2That hat doesnt exist!");
		return;
	}

	if(!$Pref::HatMod::DuplicateHats && %target.hasHat(%hatName)) {
		if(%target == %client)
			%client.chatMessage("\c2You already has the hat \c3" @ %hatName @ "\c2!");
		else
			%client.chatMessage("\c3" @ %target.name SPC "\c2already has the hat \c3" @ %hatName @ "\c2!");
		return;
	}

	HatMod_addHat(%target, %target.bl_id, %hatName, 1);

	//	%hatName = $HatMod::hats::name[$HatMod::hats::idx[strReplace(%hatName, " ", "_")]]; //Capitalises stuff

	if(%target != %client) {
		%target.chatMessage("\c3" @ %client.name SPC "\c2granted you a\c3" SPC %hatName @ "\c2! Type \c3/hat" SPC %hatName SPC "\c2to wear it.");
		%client.chatMessage("\c2You granted\c3" SPC %target.name SPC "\c2a\c3" SPC %hatName @ "\c2.");
	} else
		%client.chatMessage("\c2You granted yourself a\c3" SPC %hatName @ "\c2, cheater!");
}

function serverCmdGiveHat(%client, %target, %na, %nb, %nc, %nd, %ne) {
	if(!$Pref::HatMod::HatSharing) {
		%client.chatMessage("\c2Hat sharing has been disabled.");
		return;
	}

	if($Pref::HatMod::HatAccess) {
		%client.chatMessage("\c2Hat access is on. There is no need to give hats.");
		return;
	}

	%hatName = trim(%na SPC %nb SPC %nc SPC %nd SPC %ne);

	%target = findClientByName(%target);
	if(!isObject(%target)) {
		%client.chatMessage("\c2Could not find a player with that name. Usage: \c3/giveHat Target Hat name\c2.");
		return;
	}

	if(%hatName $= "random" || %hatName $= "rand")
		%hatName = %client.getRandomHat();

	if(!isHat(%hatName)) {
		%client.chatMessage("\c2That hat doesnt exist!");
		return;
	}

	if(%target == %client) {
		%client.chatMessage("\c2You cant give yourself a hat!");
		return;
	}

	if(!%client.hasHat(%hatName)) {
		%client.chatMessage("\c2You don't have that hat!");
		return;
	}

	//	%hatName = $HatMod::hats::name[(%idx = $HatMod::hats::idx[strReplace(%hatName, " ", "_")])]; //Capitalises stuff

	if(!$Pref::HatMod::DuplicateHats && %target.hasHat(%hatName)) {
		%client.chatMessage("\c3" @ %target.name SPC "\c2already has the hat \c3" @ %hatName @ "\c2!");
		return;
	}

	HatMod_addHat(%client, %client.bl_id, %hatName, -1);
	HatMod_addHat(%target, %target.bl_id, %hatName, 1);

	%target.chatMessage("\c3" @ %client.name SPC "\c2gave you a\c3" SPC %hatName @ "\c2! Type \c3/hat" SPC %hatName SPC "\c2to wear it.");
	%client.chatMessage("\c2You gave\c3" SPC %target.name SPC "\c2a\c3" SPC %hatName @ "\c2.");
}

function serverCmdClearHats(%client, %name) {
	if(!$Pref::HatMod::ClearHats && !%client.isSuperAdmin) {
		%client.chatMessage("\c2Hat clearing is disabled.");
		
		cancel(%client.hatMod_confirmClearHats);
		%client.hatMod_clearHatTarget = "";
		return;
	}

	if(isEventPending(%client.hatMod_confirmClearHats)) {

		if(%client.isSuperAdmin && %client.hatMod_clearHatTarget !$= "") {
			%target = %client.hatMod_clearHatTarget;
			if(!isObject(%target)) {
				%client.chatMessage("\c2Cant find player. Usage: \c3/clearHats [Target]\c2."); //Just incase they disconnect or something

				cancel(%client.hatMod_confirmClearHats);
				%client.hatMod_clearHatTarget = "";

				return;
			}
		} else
			%target = %client;


		%target.clearAllHats();

		%count = ClientGroup.getCount();
		for(%i=0; %i < %count; %i++) {
			%c = ClientGroup.getObject(%i);
			if(%c != %target) {
				if(%client != %target)
					%c.chatMessage("\c3" @ %client.name SPC "\c2cleared all of\c3" SPC %target.name @ "\c2's hats!");
				else
					%c.chatMessage("\c3" @ %client.name SPC "\c2cleared all of their hats!");
			}
		}
		if(%client != %target)
			%target.chatMessage("\c3" @ %client.name SPC "\c2cleared all of your hats!");
		else
			%client.chatMessage("\c2You cleared all of your hats!");

		cancel(%client.hatMod_confirmClearHats);
		%client.hatMod_clearHatTarget = "";


		return;
	}

	if(%client.isSuperAdmin && %name !$= "") {
		%target = findClientByName(%name);
		if(!isObject(%target)) {
			%client.chatMessage("\c2Cant find player. Usage: \c3/clearHats [Target]\c2.");
			return;
		}
	} else
		%target = %client;

	%client.hatMod_confirmClearHats = %client.schedule(5000, chatMessage, "\c3Canceled clearing hats.");
	%client.hatMod_clearHatTarget = %target;

	if(%target != %client)
		%client.chatMessage("\c2Say \c3/clearHats\c2 again to clear \c0ALL\c2 of\c3" SPC %target.name @ "\c2's hats");
	else
		%client.chatMessage("\c2Say \c3/clearHats\c2 again to clear \c0ALL\c2 of your hats");
}

function serverCmdHats(%client, %na, %nb, %nc, %nd, %ne) {
	serverCmdHat(%client, %na, %nb, %nc, %nd, %ne);
}

function serverCmdHat(%client, %na, %nb, %nc, %nd, %ne) {
	%hat = trim(%na SPC %nb SPC %nc SPC %nd SPC %ne);

	if(%hat $= "none" || %hat $= "off") {
		if($Pref::HatMod::ForceRandom) {
			%client.chatMessage("\c2Force random hats is enabled. You can't take off your hat.");
			return;
		}

		%client.unmountHat();
		return;
	}

	if(%hat $= "random" || %hat $= "rand") {
		if(isObject(%player = %client.player))
			%player.wearRandomHat();

		$HatMod::save::wornHat[%client.bl_id] = "random";
		return;
	}

	if(isHat(%hat) && %client.hasHat(%hat)) {
		if($Pref::HatMod::ForceRandom) {
			%client.chatMessage("\c2Force random hats is enabled. You can't change your hat.");
			return;
		}
		if(isObject(%player = %client.player))
			%player.mountHat(%hat);

		$HatMod::save::wornHat[%client.bl_id] = strReplace(%hat, " ", "_");
	} else {
		%client.chatMessage("\c6     Your Hats");
		%client.chatMessage("\c6     Say \c3/hat Hat name\c6 to wear a hat");
		%hatList = "\c6   None  \c7|\c6 ";
		%count = 1;
		for(%i=0; %i < HatMod_HatSet.getCount(); %i++) {
			%hat = HatMod_HatSet.getObject(%i).hatName;
			if((%num = $HatMod::save::hats[%client.bl_id, strReplace(%hat, " ", "_")]) > 0 || $Pref::HatMod::HatAccess) {
				%hatList = trim(%hatList SPC %hat @ (%num != 1 && $Pref::HatMod::DuplicateHats && !$Pref::HatMod::HatAccess ? " (" @ %num @ ")" : ""));
				%hatList = %hatList SPC " \c7|\c6 ";

				%count++;
				if(%count % 4 == 0) {
					%client.chatMessage(trim(getWords(%hatList, 0, getWordCount(%hatList)-2)));
					%hatList = "\c6  ";
				}
			}
		}
		if(%hatList !$= "\c6  ") {
			%client.chatMessage(trim(getWords(%hatList, 0, getWordCount(%hatList)-2)));
		}
	}
	HatMod_save();
}