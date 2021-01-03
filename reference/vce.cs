
///////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////
////////Heads up! This file is only executed if VCE is enabled!////////
////////////////Be careful when using functions from it////////////////
///////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////


function Player::getMountedHat(%player) {
	%image = %player.getMountedImage(2);

	if(!isObject(%image))
		return "";

	%image = %image.getName();
	%image = getSubStr(%image, 3, strLen(%image)-7);
	%image = strReplace(%image, "_", " ");

	return %image;
}

function HatMod_VCE_getTotalHatCount(%id) {
	for(%i=0; %i < HatMod_HatSet.getCount(); %i++) {
		%hat = HatMod_HatSet.getObject(%i).hatName;

		%num += $HatMod::save::hats[%id, strReplace(%hat, " ", "_")];
	}
	return %num + 0;
}

function HatMod_VCE_getTotalUniqueHatCount(%id) {
	for(%i=0; %i < HatMod_HatSet.getCount(); %i++) {
		%hat = HatMod_HatSet.getObject(%i).hatName;

		if($HatMod::save::hats[%id, strReplace(%hat, " ", "_")] > 0)
			%num++;
	}
	return %num + 0;
}

function HatMod_VCE_getHatCount(%id, %hat) {
	return $HatMod::save::hats[%id, %hat] + 0;
}

function HatMod_VCE_hasHat(%id, %hat) {
	return $HatMod::save::hats[%id, %hat] > 0;
}


package HatMod_VCE_Package {
	function VCE_initServer() {
		Parent::VCE_initServer();

		registerSpecialVar(Player, "wornHat", "%this.getMountedHat()");


		registerSpecialVar(GameConnection, "totalHats", "HatMod_VCE_getTotalHatCount(%this.bl_id)");
		registerSpecialVar(GameConnection, "uniqueHats", "HatMod_VCE_getTotalUniqueHatCount(%this.bl_id)");

		registerSpecialVar(fxDTSBrick, "ownerTotalHats", "HatMod_VCE_getTotalHatCount(%this.getGroup().bl_id)");
		registerSpecialVar(fxDTSBrick, "ownerUniqueHats", "HatMod_VCE_getTotalUniqueHatCount(%this.getGroup().bl_id)");


		for(%i=0; %i < HatMod_HatSet.getCount(); %i++) {
			%hat = HatMod_HatSet.getObject(%i).hatName;
			registerSpecialVar(GameConnection, "hatCount_" @ strReplace(%hat, " ", "_"), "HatMod_VCE_getHatCount(%this.bl_id, " @ %hat @ ")");
			registerSpecialVar(GameConnection, "hasHat_" @ strReplace(%hat, " ", "_"), "HatMod_VCE_hasHat(%this.bl_id, " @ %hat @ ")");

			registerSpecialVar(fxDTSBrick, "ownerHatCount_" @ strReplace(%hat, " ", "_"), "HatMod_VCE_hasHat(%this.getGroup().bl_id, " @ %hat @ ")");
			registerSpecialVar(fxDTSBrick, "ownerHasHat_" @ strReplace(%hat, " ", "_"), "HatMod_VCE_hasHat(%this.getGroup().bl_id, " @ %hat @ ")");
		}



		registerSpecialVar("GLOBAL", "hatCount", "HatMod_HatSet.getCount()");

		registerSpecialVar("GLOBAL", "HatRandomTick", "$Pref::HatMod::RandomHats");
		registerSpecialVar("GLOBAL", "HatTickTime", "$Pref::HatMod::HatTickTime");
		registerSpecialVar("GLOBAL", "HatChance", "$Pref::HatMod::HatChance");
		registerSpecialVar("GLOBAL", "HatItems", "$Pref::HatMod::Items");
		registerSpecialVar("GLOBAL", "HatSharing", "$Pref::HatMod::HatSharing");
		registerSpecialVar("GLOBAL", "HatClearHats", "$Pref::HatMod::ClearHats");
		registerSpecialVar("GLOBAL", "HatDuplicateHats", "$Pref::HatMod::DuplicateHats");
		registerSpecialVar("GLOBAL", "HatAccess", "$Pref::HatMod::HatAccess");
		registerSpecialVar("GLOBAL", "HatForceRandom", "$Pref::HatMod::ForceRandom");
	}
};
activatePackage(HatMod_VCE_Package);