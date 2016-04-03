function MinigameSO::checkRemaining(%this) {
	if(!%this.rounds) {
		%this.rounds = 1;
	}

	%count = 0;
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			%count++;
		}
	}

	if(%count) {
		return;
	}

	%this.messageAll('MsgProcessComplete', "\c5End of \c3round" SPC %this.rounds @ "!");

	cancel(%this.forceEndSched);
	%this.endDecision = %this.schedule(3000, makeEndDecision);
}

function MinigameSO::makeEndDecision(%this) {
	if(%this.rounds % 3 == 0) {
		%this.endGame();
	} else {
		%this.startRound();
	}
}

function MinigameSO::startRound(%this) {
	if(isEventPending(%this.forceEndSched)) {
		return;
	}

	generateBlankaBoard();

	%this.respawnAll();
	%this.rounds++;

	%this.messageAll('', "\c5Starting \c3round" SPC %this.rounds);

	%this.forceEndSched = %this.schedule(45000, forceEnd);
}

function MinigameSO::forceEnd(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			%client.player.schedule(33, delete);
		}
		if(isObject(%client.blanka)) {
			%client.blanka.explode();
		}
	}

	%this.schedule(33, checkRemaining);
}

function MinigameSO::endGame(%this) {
	if(!%this.games) {
		%this.games = 1;
	}

	%highest[score] = 0;

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(%client.score > %highest[score]) {
			%highest[score] = %client.score;
			%highest[client] = %client;
		}

		%client.score = 0;
	}

	%this.messageAll('', "\c3" @ %highest[client].name SPC "\c5won \c3game" SPC %this.games SPC "\c5with\c3" SPC %highest[score] SPC "points!");

	%this.games++;
	%this.rounds = 0;

	%this.schedule(8000, startRound);
}