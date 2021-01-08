exec("./lib/Support_CenterprintMenuSystem.cs");
exec("./lib/datablockCreation.cs");
exec("./lib/disableMountSound.cs");

exec("./util/colorUtils.cs");
exec("./util/getLastStrPos.cs");
exec("./util/hexToInt.cs");
exec("./util/objectVariables.cs");
exec("./util/strContains.cs");

exec("./register.cs");
exec("./shoes.cs");
exec("./customization.cs");
exec("./interface.cs");

//development
function serverCmdReExecShoeMod(%cl)
{
	if (%cl.isSuperAdmin)
	{
		setmodpaths(getmodpaths());
		exec("Add-ons/Server_ShoeMod/server.cs");
	}
}