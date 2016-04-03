datablock AudioProfile(CorrectAnswer)
{
	filename = "./sounds/correct.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(SwitchAnswer:CorrectAnswer) { filename = "./sounds/answer.wav"; };
datablock AudioProfile(IncorrectAnswer:CorrectAnswer) { filename = "./sounds/incorrect.wav"; };
datablock AudioProfile(NewQuestion:CorrectAnswer) { filename = "./sounds/new.wav"; };

datablock AudioDescription(AudioMusicLooping2d:AudioMusicLooping3d) {
	is3d = 0;
	referenceDistance = 999999;
	maxDistance = 999999;
	impactImpulse = 2600;
};
datablock AudioProfile(WaitingMusic) {
	filename = "./music/waiting.ogg";
	description = AudioMusicLooping2d;
	preload = 1;
	uiName = "Waiting Music";
};
datablock AudioProfile(Level1Music:WaitingMusic) { filename = "./music/level1.ogg"; uiName = "Level 1 Music"; };
datablock AudioProfile(Level2Music:WaitingMusic) { filename = "./music/level2.ogg"; uiName = "Level 2 Music"; };
datablock AudioProfile(Level3Music:WaitingMusic) { filename = "./music/level3.ogg"; uiName = "Level 3 Music"; };

if(!isObject(TriviaMusic)) {
	new AudioEmitter(TriviaMusic) {
		is3D = 0;
		profile = "WaitingMusic";
		referenceDistance = 999999;
		maxDistance = 999999;
		volume = 0.8;
		position = "0 0 0";
	};
}

// can't link it to a parent T_T
function updateTriviaMusic(%profile) {
	if(isObject(TriviaMusic)) {
		TriviaMusic.delete();
	}

	if(%profile == -1) {
		return;
	}

	if(!isObject(%profile)) {
		talk(%profile SPC "is not an object.");
	}

	new AudioEmitter(TriviaMusic) {
		is3D = 0;
		profile = %profile;
		referenceDistance = 999999;
		maxDistance = 999999;
		volume = 0.8;
		position = "0 0 0";
	};
}