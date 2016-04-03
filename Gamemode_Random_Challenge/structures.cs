//create structures, return the position of where the next structure will start
//add bricks to client's brickgroup

exec("./support.cs");

function GameConnection::createStructure(%this, %which, %pos) {
	switch$(%which) {
		case "lavajump_line": return;
		case "lavajump_diamond": return;
		case "lavastairs": return;

		case "treeclimb": return;

		case "rampjump": return;
		case "stickjump": return;
		case "hoverjump": return;

		case "trampolines": return;

		case "arrowavoidance": return;

		case "spawn": return;
	}
}

function StructureContainer::generateTreeClimb(%this, %amount, %start_pos) {
	%pos = %start_pos;
	%coords = "0 0 0 0 0" SPC getWord(%start_pos, 2) @ "\t0 0 0 0\t0 0 0";

	%colors["support"] = "35 34 33 32 31 30";
	%colors["platform"] = "0 1 2 3 4 5 6 7 8 9";
	%color["support"] = getWord(%colors["support"], getRandom(0, getWordCount(%colors["support"])-1));
	%color["platform"] = getWord(%colors["platform"], getRandom(0, getWordCount(%colors["platform"])-1));

	while(%amount > 0) {
		switch(%amount % 4) {
			case 0: // south
				%rot = "1 0 0 0";
				%plate_offset = "0 -0.5 0";
			case 1: // west
				%rot = "0 0 1 90.0002";
				%plate_offset = "-0.5 0 0";
			case 2: // north
				%rot = "0 0 1 180";
				%plate_offset = "0 0.5 0";
			case 3: // east
				%rot = "0 0 -1 90.0002";
				%plate_offset = "0.5 0 0";
		}

		%pos = getWords(%start_pos, 0, 1) SPC getWord(%coords, 5) + ((brick2x2x5Data.brickSizeZ/5)/2);
		%coords = %this.plantBrick("brick2x2x5Data", %pos, %rot, %color["support"]);
		%brickSizeZ = getWord(getField(%coords, 2), 2);

		%pos = vectorAdd(getWords(%pos, 0, 1) SPC getWord(getField(%coords, 0), 2), "0 0" SPC %brickSizeZ);
		%coords = %this.plantBrick("brick2x4fData", vectorAdd(%pos, %plate_offset), %rot, %color["platform"]);
		%brickSizeZ = getWord(getField(%coords, 2), 2);

		%amount--;
		if(!%amount) {
			return %coords;
		}
	}
}

function StructureContainer::generateLavaJump_Line(%this, %amount, %start_pos, %rot) {
	talk(%start_pos);

	switch$(%rot) {
		case "1 0 0 0": // south
			%dir = "0 -4 0";
		case "0 0 1 90.0002": // west
			%dir = "-4 0 0";
		case "0 0 1 180": // north
			%dir = "0 4 0";
		case "0 0 -1 90.0002": // east
			%dir = "4 0 0";
		default: // north
			%dir = "0 4 0";
	}

	%pos = vectorAdd(vectorScale(%dir, 1.5), %start_pos);

	%colors["support"] = "35 34 33 32 31 30";
	%colors["lava"] = "0 1 4";
	%color["support"] = getWord(%colors["support"], getRandom(0, getWordCount(%colors["support"])-1));
	%color["lava"] = getWord(%colors["lava"], getRandom(0, getWordCount(%colors["lava"])-1));

	%this.plantBrick("brick16x16fData", %pos, %rot, %color["support"]);

	%pos = vectorAdd(vectorScale(%dir, 0.5), %start_pos);
	while(%amount > 0) {
		%pos = vectorAdd(vectorScale(%dir, 4), %pos);
		%coords = %this.plantBrick("brick16x32fData", %pos, %rot, %color["support"]);

		switch$(%rot) {
			case "1 0 0 0": // south
				%lava_dir = "0 0 -1 90.0002";
			case "0 0 1 90.0002": // west
				%lava_dir = "0 0 1 180";
			case "0 0 1 180": // north
				%lava_dir = "0 0 -1 90.0002";
			case "0 0 -1 90.0002": // east
				%lava_dir = "0 0 1 180";
		}

		for(%i=-1;%i<3;%i++) {
			%lava_pos = vectorAdd(%pos, getWords(vectorScale(%dir, %i), 0, 1) SPC 0.2);
			%this.plantBrick("brick1x16fData", %lava_pos, %lava_dir, %color["lava"]);
		}

		%amount--;
	}

	%pos = vectorAdd(vectorScale(%dir, 3), %pos);
	%coords = %this.plantBrick("brick16x16fData", %pos, %rot, %color["support"]);

	return %coords;
}

// testing
function GameConnection::makeTrees(%this, %left, %up) {
	for(%x=0;%x<%left;%x++) {
		for(%y=0;%y<%up;%y++) {
			%pos = %x*20 SPC %y*20 SPC 0;
			%this.structureSystem.generateTreeClimb(getRandom(15, 25), %pos);
		}
	}
}

function GameConnection::testChain(%this, %pos, %amnt) {
	%coords = %this.structureSystem.generateTreeClimb(getRandom(10, 20), %pos);
	talk(getField(%coords, 0));
	%coords = %this.structureSystem.generateLavaJump_Line(%amnt, getWords(getField(%coords, 0), 3, 5), getField(%coords, 1));
	%this.structureSystem.generateTreeClimb(getRandom(10, 20), getWords(getField(%coords, 3), 0, 1) SPC getWord(%coords, 5));
}

package RandomChallengePackage {
	function GameConnection::autoAdminCheck(%this) {
		if(!isObject(%this.structureSystem)) {
			%this.structureSystem = new ScriptGroup(StructureContainer) {
				client = %this;
				brickgroup = %this.brickgroup;
			};
		}

		return parent::autoAdminCheck(%this);
	}
};
activatePackage(RandomChallengePackage);