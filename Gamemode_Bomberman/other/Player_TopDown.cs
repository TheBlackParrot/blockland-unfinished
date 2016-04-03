//quakePlayer.cs

//a new player datablock with quake-like movement



datablock PlayerData(PlayerTopDownArmor : PlayerStandardArmor)
{
	maxBackwardSpeed = 10;
	maxForwardSpeed = 10;
	maxSideSpeed = 10;

	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	
	cameramaxdist = 30;
	cameramindist = 0;
	maxlookangle = 0;
	cameratilt = 1.5;
	maxlookangle = 0;
	minlookangle = 0;
	cameraMaxFov = 75;
	thirdpersononly = 1;

	uiName = "Top Down Player";
	showEnergyBar = false;
};