exec("./version.cs");

function clientCmdHatMod_CheckVersion(%version) {
	if(%version - $HatMod::CurrentVersion < 3 && %version > $HatMod::LatestVersion && mFloatLength(%version, 2)+0 $= %version) {
		if(!$HatMod::HasMessagedUpdate) { //Just some simple logic to make sure that your only given the message once
			clientCmdMessageBoxOk("HatMod is outdated!", "It has been detected that you do not have the latest version of HatMod!<br>Check the topic <a:forum.blockland.us/index.php?topic=254646.0>here</a>.");
			$HatMod::HasMessagedUpdate = 1;
		}
		$HatMod::LatestVersion = %version;
	}

	commandToServer('HatMod_ReturnVersion', $HatMod::LatestVersion);
}