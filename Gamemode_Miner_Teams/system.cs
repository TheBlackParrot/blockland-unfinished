function MinigameSO::setTeamData(%this) {
	%file = new FileObject();
	%file.openForRead("config/server/MinerTeams/teams.db");

	%this.teamCount = 0;
	while(!%file.isEOF()) {
		%data = %file.readLine();

		%this.team[%this.teamCount, colorID] = getField(%data, 0);
		%this.team[%this.teamCount, name] = getField(%data, 1);
		%this.team[%this.teamCount, colorHex] = getField(%data, 2);

		echo("Registered team" SPC getField(%data, 1));
		%this.teamCount++;
	}

	%file.close();
	%file.delete();

	%this.hasSetTeamData = 1;
}

function MinigameSO::setTeams(%this) {
	if(!%this.hasSetTeamData) {
		%this.setTeamData();
	}

	%teams = mClamp(%this.numMembers / 4, 2, 4);
	
	for(%i=0;%i<%teams;%i++) {
		%rand = getRandom(0, %this.teamCount-1);
		while(stripos(%teamColors, ":" @ %rand @ ":") != -1) {
			%rand = getRandom(0, %this.teamCount);
		}

		%this.team[%rand, inUse] = 1;
		%teamColors = %teamColors @ ":" @ %rand @ ":";
	}
	%teamColors = trim(strReplace(%teamColors, ":", " "));
	%teamColors = trim(strReplace(%teamColors, "  ", " "));

	%currTeam = getRandom(0, %teams-1);

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(%currTeam > %teams) {
			%currTeam = 1;
		}

		%client.setTeam(getWord(%teamColors, %i));

		%currTeam++;
	}
}

function GameConnection::setTeam(%this, %team) {
	%this.team = %team;

	if(isObject(%this.player)) {
		%player = %this.player;
		%player.setUniform();
	}
}

function GameConnection::updateBottomPrint(%this) {
	%mg = $DefaultMinigame;
	%row = $Mining::ActiveLoopData;

	%str = "";

	%count = 1;
	for(%i=0;%i<20;%i++) {
		if(%mg.team[%i, score]) {
			switch(%count % 2) {
				case 1:
					%str = %str @ "<just:left><color:" @ %mg.team[%i, colorHex] @ ">" @ strUpr(%mg.team[%i, name]) SPC "\c6" @ %mg.team[%i, score];
				case 0:
					%str = %str @ "<just:right><color:" @ %mg.team[%i, colorHex] @ ">" @ strUpr(%mg.team[%i, name]) SPC "\c6" @ %mg.team[%i, score] @ "<br>";
			}
			%count++;
		}
	}

	%str = %str @ "<br><just:left><font:Arial Bold:16>\c6" @ %row.dataBlock.uiName SPC "<just:right>\c2(" @ %row.tempoBPM @ " BPM)";

	%this.bottomPrint(%str, 5, 1);
}

function MinigameSO::checkForResetDone(%this) {
	$Mining::CanPlay = 0;

	if(BrickGroup_888888.getCount() > 0) {
		%this.schedule(500, checkForResetDone);
		messageAll('', "\c2" @ BrickGroup_888888.getCount() SPC "bricks remaining");
		return;
	}

	deleteVariables("$Mining::Brick*");

	placeMiningBrick(0, 0, 12000);
	$Mining::Brick[0, 0, 12000].mineExplode(35);

	for(%i=0;%i<%this.teamCount;%i++) {
		%this.team[%i, score] = 0;
		%this.team[%i, inUse] = 0;
	}

	cancel(%this.musicLoopSched);
	%this.musicLoop();

	%this.setTeams();

	%this.respawnAll();

	%this.startedAt = $Sim::Time;
	%this.resetSched = %this.schedule(600000, reset);
	%this.endingAt = $Sim::Time+600;

	%this.positionLimits = 70*mCeil(%this.numMembers/2);

	for(%i=1;%i<=9;%i++) {
		%this.timeLimitSched[%i-1] = %this.schedule(60000*%i, messageAll, '', "\c3" @ 10-%i SPC "minutes \c5remaining");
	}

	$Mining::CanPlay = 1;
}

package MinerTeamsMinigamePackage {
	function MinigameSO::reset(%this) {
		cancel(%this.resetSched);
		for(%i=0;%i<9;%i++) {
			cancel(%this.timeLimitSched[%i]);
		}
		cancel(%this.chainDeleteSched);
		cancel(%this.resetCheckSched);

		%highest[team] = %this.teamCount;
		%highest[score] = 0;

		for(%i=0;%i<%this.teamCount;%i++) {
			if(%this.team[%i, score] > %highest[score]) {
				%highest[score] = %this.team[%i, score];
				%highest[team] = %i;
			}
		}

		if(%highest[team] < %this.teamCount) {
			%this.team[%highest[team], wins]++;
			%this.messageAll('MsgAdminForce', "<color:" @ %this.team[%highest[team], colorHex] @ ">" @ strUpr(%this.team[%highest[team], name]) SPC "\c5wins this round with\c3" SPC %this.team[%highest[team], score] SPC "points!");
			%this.messageAll('', "<color:" @ %this.team[%highest[team], colorHex] @ ">" @ strUpr(%this.team[%highest[team], name]) SPC "\c5has won\c3" SPC %this.team[%highest[team], wins] SPC "times");
		}

		%this.chainDeleteSched = BrickGroup_888888.schedule(5000, chainDeleteAll);
		%this.resetCheckSched = %this.schedule(5000, checkForResetDone);

		return parent::reset(%this);
	}

	//function GameModeInitialResetCheck() {
	//	parent::GameModeInitialResetCheck();
	//}
};
activatePackage(MinerTeamsMinigamePackage);