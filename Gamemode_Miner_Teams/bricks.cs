datablock AudioProfile(minePlaceSound1)
{
	filename = "./sounds/place1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(minePlaceSound2 : minePlaceSound1) { filename = "./sounds/place2.wav"; };
datablock AudioProfile(minePlaceSound3 : minePlaceSound1) { filename = "./sounds/place3.wav"; };
datablock AudioProfile(mineBombBeep : minePlaceSound1) { filename = "./sounds/beep.wav"; };
datablock AudioProfile(mineExplosionSound : minePlaceSound1) { filename = "./sounds/explosion.wav"; };

function placeMiningBrick(%x, %y, %z, %teamID) {
	if($Mining::Brick[%x,%y,%z]) {
		return;
	}

	%mg = $DefaultMinigame;

	%inLimits = withinLimits(%x SPC %y SPC %z);

	%bomb = 0;
	%colorFX = 0;

	if(%inLimits) {
		if(%teamID $= "") {
			%color = $Mining::DefaultColor;
		} else {
			if(%teamID > %mg.teamCount-1) {
				%color = $Mining::DefaultColor;
			} else {
				%color = %mg.team[%teamID, colorID];
			}
		}

		if(!getRandom(0, 500) && %teamID < 20 && %teamID !$= "") {
			%team = 20;
			%bomb = 1;
			%colorFX = 4;
		}
	} else {
		%color = 22;
		%teamID = -1;
	}

	%brick = new fxDTSBrick(MineBrick) {
		angleID = 0;
		client = -1;
		colorFxID = %colorFX;
		colorID = %color;
		dataBlock = "brick4xCubeData";
		isBasePlate = 1;
		isPlanted = 1;
		position = %x SPC %y SPC %z;
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = 888888;
		team = %teamID;
		bomb = %bomb;
	};

	%brick.plant();
	%brick.setTrusted(1);

	BrickGroup_888888.add(%brick);

	$Mining::Brick[%x,%y,%z] = %brick;

	if(%teamID < 20) {
		$DefaultMinigame.team[%teamID, score]++;
	}

	%sound = getRandom(1, 3);
	serverPlay3D("minePlaceSound" @ %sound, %brick.getPosition());
}

function placeSurroundingBricks(%x, %y, %z, %team) {
	placeMiningBrick(%x+2, %y, %z, %team);
	placeMiningBrick(%x-2, %y, %z, %team);
	placeMiningBrick(%x, %y+2, %z, %team);
	placeMiningBrick(%x, %y-2, %z, %team);
	placeMiningBrick(%x, %y, %z+2, %team);
	placeMiningBrick(%x, %y, %z-2, %team);
}

function fxDTSBrick::placeSurroundingBricks(%this, %client) {
	%x = getWord(%this.getPosition(), 0);
	%y = getWord(%this.getPosition(), 1);
	%z = getWord(%this.getPosition(), 2);

	placeSurroundingBricks(%x, %y, %z, %client.team);
}

function fxDTSBrick::onMineBrick(%this, %client) {
	%pos = %this.getPosition();
	%mg = $DefaultMinigame;

	if(%this.team < 0) {
		return;
	}

	if(%this.bomb) {
		if(%this.bombSched $= "") {
			%this.bombLoop();
		}
		return;
	}

	placeSurroundingBricks(getWord(%pos, 0), getWord(%pos, 1), getWord(%pos, 2), %client.team);
	%mg.team[%this.team, score]--;

	if(isObject(%client)) {
		%client.updateBottomPrint();
	}

	%this.delete();
}

function fxDTSBrick::bombLoop(%this) {
	cancel(%this.bombSched);
	%this.bombSched = %this.schedule($Mining::ActiveLoopData.tempoMS*2, bombLoop);

	if(%this.beeps == 4) {
		serverPlay3D(mineExplosionSound, %this.getPosition());
		%this.schedule(10, mineExplode, getRandom(20, 30));
		return;
	}

	switch(%this.beeps % 2) {
		case 0:
			%this.setColor(21);
		case 1:
			%this.setColor(20);
	}
	
	%this.setColorFX(3);
	%this.schedule($Mining::ActiveLoopData.tempoMS, setColorFX, 0);

	serverPlay3D(mineBombBeep, %this.getPosition());

	%this.beeps++;
}

function Mining_doExplosionStep(%pos, %radius, %client, %bomb) {
	if(%bomb) {
		%type = $TypeMasks::All;
	} else {
		%type = $TypeMasks::FXBrickObjectType;
	}

	InitContainerRadiusSearch(%pos, %radius, %type);
	
	while((%targetObject = containerSearchNext()) != 0) {
		if(%targetObject.getClassName() $= "fxDTSBrick") {
			%targetObject.placeSurroundingBricks(%client);
			
			if(%targetObject.team < 20 && %targetObject.team >= 0) {
				$DefaultMinigame.team[%targetObject.team, score]--;
			}

			if(%targetObject) {
				if(%targetObject.team >= 0) {
					%targetObject.schedule(0, delete);
				}
			}
		}

		if(%targetObject.getClassName() $= "Player") {
			%targetObject.client.play2D(mineExplosionSound);
			%targetObject.kill();
		}
	}

	if(%radius != 0) {
		schedule(1, 0, Mining_doExplosionStep, %pos, %radius-1, %bomb);
	}
}

function fxDTSBrick::mineExplode(%this, %radius, %client) {
	if(!%radius) {
		%radius = getRandom(1, 3);
	}

	Mining_doExplosionStep(%this.getPosition(), %radius, %client, %this.bomb);
}

function withinLimits(%pos) {
	%mg = $DefaultMinigame;
	%limit = %mg.positionLimits;

	%x = mAbs(getWord(%pos, 0));
	%y = mAbs(getWord(%pos, 1));
	%z = mAbs(getWord(%pos, 2)-12000);

	if(%x < %limit) {
		if(%y < %limit) {
			if(%z < %limit) {
				return 1;
			}
		}
	}

	return 0;
}

function fxDTSBrick::withinLimits(%this) {
	return withinLimits(%this.getPosition());
}

package MiningBrickPackage {
	function onServerDestroyed() {
		deleteVariables("$Mining::*");

		return parent::onServerDestroyed();
	}
};
activatePackage(MiningBrickPackage);