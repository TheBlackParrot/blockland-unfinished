function initBlankaBoardVars() {
	%lowest[x] = %lowest[y] = 999999;
	%highest[x] = %highest[y] = -999999;

	for(%i=1;%i<=4;%i++) {
		%brick = "_blankaBoundary" @ %i;
		%pos[%i,x] = getWord(%brick.getPosition(), 0);
		%pos[%i,y] = getWord(%brick.getPosition(), 1);

		if(%pos[%i,x] > %highest[x]) {
			%highest[x] = %pos[%i,x];
		}
		if(%pos[%i,x] < %lowest[x]) {
			%lowest[x] = %pos[%i,x];
		}

		if(%pos[%i,y] > %highest[y]) {
			%highest[y] = %pos[%i,y];
		}
		if(%pos[%i,y] < %lowest[y]) {
			%lowest[y] = %pos[%i,y];
		}
	}

	$Blanka::LowestBoundary = %lowest[x] SPC %lowest[y];
	$Blanka::HighestBoundary = %highest[x] SPC %highest[y];

	for(%i=1;%i<=4;%i++) {
		%brick = "_blankaBoundary" @ %i;
		%brick.killBrick();
	}

	if(!$DefaultMinigame.games) {
		$DefaultMinigame.games = 1;
		$DefaultMinigame.startRound();
	}
}

function generateBlankaBoard() {
	if($Blanka::BoardPieces) {
		for(%i=0;%i<$Blanka::BoardPieces;%i++) {
			%brick = $Blanka::Board[%i];
			if(isObject(%brick)) {
				%brick.delete();
			}
		}
		$Blanka::BoardPieces = 0;
	}

	if($Blanka::LowestBoundary $= "") {
		initBlankaBoardVars();
	}

	%lowest[x] = getWord($Blanka::LowestBoundary, 0);
	%lowest[y] = getWord($Blanka::LowestBoundary, 1);
	%highest[x] = getWord($Blanka::HighestBoundary, 0);
	%highest[y] = getWord($Blanka::HighestBoundary, 1);

	%base = getRandom(mFloor(%lowest[x]), mCeil(%highest[x])) SPC getRandom(mFloor(%lowest[y]), mCeil(%highest[y])) SPC 0.1;

	$Blanka::BoardPieces = 0;
	//echo("GENERATING NEW BOARD");

	for(%i=0;%i<17;%i++) {
		switch(%i) {
			case 0:
				%color = 36;
				%value = 25;

			case 1:
				%color = 37;
				%value = 20;

			case 2:
				%color = 38;
				%value = 15;

			case 3:
				%color = 39;
				%value = 12;

			case 4:
				%color = 40;
				%value = 10;

			case 5 or 6:
				%color = 41;
				%value = 7;

			case 7 or 8:
				%color = 42;
				%value = 4;

			case 9 or 10:
				%color = 43;
				%value = 3;

			case 11 or 12 or 13:
				%color = 44;
				%value = 2;

			case 14 or 15 or 16:
				%color = 45;
				%value = 1;
		}
		if(%i == 0) {
			plantBlankaBrick(%base, %color, %value);
			continue;
		}

		for(%x=%i*-1; %x<=%i; %x++) {
			for(%y=%i*-1; %y<=%i; %y++) {
				// thankfully 2x2 plates are 1x1 unit-wise
				plantBlankaBrick(vectorAdd(%base, %x SPC %y SPC 0), %color, %value);
			}
		}
	}
}

function plantBlankaBrick(%pos, %color, %value) {
	//echo(%pos);
	%brick = new fxDTSBrick(_scoreBrick) {
		angleID = 0;
		colorFxID = 0;
		colorID = %color;
		dataBlock = "brick2x2fData";
		isBasePlate = 0;
		isPlanted = 1;
		position = %pos;
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = 888888;
		value = %value;
	};
	BrickGroup_888888.add(%brick);
	%brick.setTrusted(1);
	%brick.plant();

	$Blanka::Board[$Blanka::BoardPieces] = %brick;
	$Blanka::BoardPieces++;
}

package BlankaBoardPackage {
	function ServerLoadSaveFile_End() {
		parent::ServerLoadSaveFile_End();

		initBlankaBoardVars();
	}
};
activatePackage(BlankaBoardPackage);