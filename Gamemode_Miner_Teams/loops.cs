if(!isObject(MiningLoopSet)) {
	new ScriptGroup(MiningLoopSet);
}

function convertTempoToMS(%tempo) {
	return 60000/%tempo;
}
function normalizeString(%str) {
	return stripChars(%str, "!@#$%^&*()_+-=[]{}\\|;':\",./<>?”“–");
}

function initLoops() {
	%dir = "config/server/MinerTeams/";

	%file = new FileObject();
	
	if(isFile(%dir @ "loops/loops.db")) {
		%file.openForRead(%dir @ "loops/loops.db");
	} else {
		warn("NO LOOP DATABASE FOUND.");
		%file.delete();
		return;
	}

	while(!%file.isEOF()) {
		%data = %file.readLine();

		%loop = getField(%data, 0);
		%tempo = getField(%data, 1);

		// get your pitchforks ready
		%dbName = normalizeString(fileBase(%loop));
		eval("datablock AudioProfile(musicData_" @ %dbName @ ") {fileName = \"" @ %dir @ "loops/" @ %loop @ "\"; description = \"AudioMusicLooping3d\"; preload = 1; uiName = \"" @ strReplace(fileBase(%loop),"_"," ") @ "\";};");

		%row = new ScriptObject(MiningLoopData) {
			filename = %loop;
			tempoBPM = %tempo;
			tempoMS = convertTempoToMS(%tempo);
			dataBlock = "musicData_" @ %dbName;
		};
		MiningLoopSet.add(%row);
	}

	%file.close();
	%file.delete();

	$Mining::LoopsInit = 1;
}
if(!$Mining::LoopsInit) {
	initLoops();
	setMusic();
	$Mining::LoopsInit = 1;
}

function setMusic(%row) {
	if(isObject($AudioEmitter)) {
		$AudioEmitter.delete();
	}

	if(!isObject(%row)) {
		%prev = getWords($Mining::PreviousMusic, 0, mFloor(MiningLoopSet.getCount()/1.5));
		%which = getRandom(0, MiningLoopSet.getCount()-1);

		while(stripos(%prev, " " @ %which @ " ") != -1) {
			%which = getRandom(0, MiningLoopSet.getCount()-1);
		}

		$Mining::PreviousMusic = " " @ %which @ " " @ %prev;
		%row = MiningLoopSet.getObject(%which);
	}

	$AudioEmitter = new AudioEmitter() {
		is3D = 0;
		profile = %row.dataBlock;
		referenceDistance = 9999999;
		maxDistance = 9999999;
		volume = 1.05;
		position = "0 0 12000";
	};

	messageAll('', "\c5Music set to\c3" SPC %row.dataBlock.uiname SPC "\c2(" @ %row.tempoBPM SPC "BPM)");

	$Mining::ActiveLoopData = %row;
}

function MinigameSO::musicLoop(%this) {
	cancel(%this.musicLoopSched);
	%this.musicLoopSched = %this.schedule(90000, musicLoop);

	setMusic();
}