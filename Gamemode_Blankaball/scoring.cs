function fxDTSBrick::doBlankaScore(%this, %client) {
	%mg = $DefaultMinigame;

	if(%client.scored) {
		return;
	}
	if(!isObject(%client.minigame)) {
		return;
	}

	%mg.messageAll('', "\c3" @ %client.name SPC "\c5scored a\c3" SPC %this.value);
	%client.score += %this.value;

	if(isObject(%client.player)) {
		%client.player.delete();
	}

	%this.setColorFX(3);
	%this.schedule(150, setColorFX, 0);

	%this.playSound(Beep_Checkout_Sound);

	$DefaultMinigame.checkRemaining();
}