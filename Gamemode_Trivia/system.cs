if(!isObject(TriviaSystem)) {
	new ScriptObject(TriviaSystem);
}

function TriviaSystem::startRound(%this) {
	%mg = $DefaultMinigame;
	
	setTriviaQuestion();

	%alive = 0;
	for(%i=0;%i<%mg.numMembers;%i++) {
		%client = %mg.member[%i];
		if(isObject(%client.player)) {
			%client.player.delete();
			%client.spawnPlayer();
			%client.player.hasAnswered = 0;
			%alive++;
		} else {
			%client.setCameraBrick("camera_general", "camera_general_aim");
		}
	}

	if(%mg.numMembers > 1) {
		%percent = (%alive/%mg.numMembers)*100;
		if(%alive > 1) {
			if(%percent < 25) { updateTriviaMusic("Level3Music"); }
			if(%percent >= 25 && %percent < 50) { updateTriviaMusic("Level2Music"); }
			if(%percent >= 50) { updateTriviaMusic("Level1Music"); }
		} else {
			updateTriviaMusic(-1);
			// need an end round sound

			%mg.schedule(5000, reset);
			messageAll('', "\c2The game will reset in 5 seconds.");
		}
	} else {
		%client = %mg.member[0];
		if(isObject(%client.player)) {
			%client.player.delete();
		}
		%client.spawnPlayer();

		updateTriviaMusic("Level1Music");
	}
}

function TriviaSystem::doAnswer(%this, %which) {
	%mg = $DefaultMinigame;
	for(%i=0;%i<%mg.numMembers;%i++) {
		%client = %mg.member[%i];
		%client.setCameraBrick("camera" @ %which, "floor" @ %which);
	}
	serverPlay2D(SwitchAnswer);
	%this.schedule(3000, checkAnswer, %which);
}
function TriviaSystem::checkAnswer(%this, %which) {
	%choices = $Trivia::CurrentChoices;
	for(%i=0;%i<getWordCount(%choices);%i++) {
		if(getWord(%choices, %i) == $Trivia::CorrectAnswer) {
			%correct = %i;
			break;
		}
	}

	%obj = ("_floor" @ %which).getID();
	if(%which != %i) {
		%obj.disappear(4);
		serverPlay2D(IncorrectAnswer);
	} else {
		serverPlay2D(CorrectAnswer);
		%obj.setColorFX(3);
		%obj.schedule(500, setColorFX, 0);
		%obj.schedule(1000, setColorFX, 3);
		%obj.schedule(1500, setColorFX, 0);
	}
}

function TriviaSystem::doAnswers(%this) {
	%mg = $DefaultMinigame;
	for(%i=0;%i<%mg.numMembers;%i++) {
		%client = %mg.member[%i];
		if(isObject(%client.player)) {
			%player = %client.player;
			if(!%player.hasAnswered) {
				%player.kill();
			} else {
				%player.changeDatablock(PlayerFrozenArmor);
				%player.setVelocity("0 0 0");
			}
			updateTriviaMusic("WaitingMusic");
		}
		%client.setCameraBrick("camera_general", "camera_general_aim");
	}

	serverPlay2D(SwitchAnswer);

	if(getWordCount($Trivia::CurrentChoices) < 4) {
		%blank = 4 - $Trivia::CurrentChoices;
		for(%i=3;%i>=%blank;%i--) {
			%obj = ("_floor" @ %i).getID();
			%obj.disappear(4);
		}
	}

	%delay = 7000;
	for(%i=0;%i<getWordCount($Trivia::CurrentChoices);%i++) {
		%this.schedule(3000+(%delay*%i), doAnswer, %i);
	}
	%this.schedule((%delay*%i)+(%delay/2)+3000, startRound);
}