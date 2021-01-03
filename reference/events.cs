registerOutputEvent(GameConnection, "grantHat", "string 25 100\tbool\tint -1000 10 1", 0);
registerOutputEvent(Bot, "setHat", "string 100 100");
registerOutputEvent(Player, "setHat", "string 100 100");

function GameConnection::grantHat(%client, %hatName, %onlyOne, %amount) {
	//	%hatName = $HatMod::hats::name[$HatMod::hats::idx[strReplace(%hatName, " ", "_")]]; //Capitalises stuff

	if(!$Pref::HatMod::DuplicateHats && $HatMod::save::hats[%client.bl_id, strReplace(%hat, " ", "_")] > 0 && %amount > 0)
		return;

	if(%hatName $= "random" || %hatName $= "rand")
		%hatName = HatMod_GetRandomHat();

	if(!isObject(%client) || !isObject(%client.player) || %hatName $= "" || !isHat(%hatName) || ($HatMod::save::hats[%client.bl_id, strReplace(%hatName, " ", "_")] > 0 && %onlyOne))
		return;

	if(%amount == 1)
		%client.chatMessage("\c1You got the hat\c3" SPC %hatName @ "\c1! Type \c3/hat" SPC %hatName SPC "\c1to wear it.");
	else if(%amount > 0)
		%client.chatMessage("\c1You got" SPC %amount SPC "of the hats\c3" SPC %hatName @ "\c1! Type \c3/hat" SPC %hatName SPC "\c1to wear one.");
	else if(%amount == -1) {
		if($HatMod::save::hats[%client.bl_id, strReplace(%hatName, " ", "_")] > 1)
			%client.chatMessage("\c1You lost a\c3" SPC %hatName SPC "\c1hat!");
		else
			%client.chatMessage("\c1You lost your only\c3" SPC %hatName SPC "\c1hat!");
	} else if(%amount < 0)
		%client.chatMessage("\c1You lost" SPC %amount * -1 SPC "of your\c3" SPC %hatName SPC "\c1hats! Type \c3/hat" SPC %hatName SPC "\c1to wear one.");

	HatMod_addHat(%client, %client.bl_id, %hatName, %amount);
}

function AIPlayer::setHat(%bot, %hatName) {
	if(%hatName $= "None" || %hatName $= "Off") {
		%bot.removeHat();
		return;
	}

	if(%hatName $= "random" || %hatName $= "rand")
		%hatName = HatMod_GetRandomHat();

	if(!isObject(%bot) || %hatName $= "" || !isHat(%hatName))
		return;

	%bot.mountHat(%hatName);
}

function Player::setHat(%player, %hatName) {
	if(%hatName $= "None" || %hatName $= "Off") {
		%player.removeHat();
		return;
	}

	if(%hatName $= "random" || %hatName $= "rand")
		%hatName = HatMod_GetRandomHat();

	if(!isObject(%player) || %hatName $= "" || !isHat(%hatName))
		return;

	%player.mountHat(%hatName);
}

