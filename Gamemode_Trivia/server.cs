exec("./questions.cs");
exec("./system.cs");

$Trivia::Version = "dev";

if(!$Trivia::Init) {
	datablock StaticShapeData(EmptyStaticShape) {
		shapeFile = "base/data/shapes/empty.dts";
	};

	exec("./audio.cs");
	exec("./playertype.cs");

	exec("./Event_onPlayerFirstTouch/server.cs");
	exec("./Event_onPlayerTouchStop/server.cs");

	gatherTriviaQuestions();

	$Trivia::Init = 1;
}

package TriviaPackage {
	function onServerDestroyed() {
		deleteVariables("$Trivia::*");

		return parent::onServerDestroyed();
	}

	function ServerLoadSaveFile_End() {
		parent::ServerLoadSaveFile_End();

		%obj = new staticShape(QuestionText) {
			datablock = EmptyStaticShape;
			scale = "1 1 1";
			position = "0 0 0";
			rotation = "0 0 0";
		};
		missionCleanup.add(%obj);

		%obj = new staticShape(Answer0Text) {
			datablock = EmptyStaticShape;
			scale = "1 1 1";
			position = "0 0 0";
			rotation = "0 0 0";
		};
		missionCleanup.add(%obj);

		%obj = new staticShape(Answer1Text) {
			datablock = EmptyStaticShape;
			scale = "1 1 1";
			position = "0 0 0";
			rotation = "0 0 0";
		};
		missionCleanup.add(%obj);

		%obj = new staticShape(Answer2Text) {
			datablock = EmptyStaticShape;
			scale = "1 1 1";
			position = "0 0 0";
			rotation = "0 0 0";
		};
		missionCleanup.add(%obj);

		%obj = new staticShape(Answer3Text) {
			datablock = EmptyStaticShape;
			scale = "1 1 1";
			position = "0 0 0";
			rotation = "0 0 0";
		};
		missionCleanup.add(%obj);

		QuestionText.setTransform(_question.getTransform());
		Answer0Text.setTransform(_answer0.getTransform());
		Answer1Text.setTransform(_answer1.getTransform());
		Answer2Text.setTransform(_answer2.getTransform());
		Answer3Text.setTransform(_answer3.getTransform());

		_question.disappear(-1);
		_answer0.disappear(-1);
		_answer1.disappear(-1);
		_answer2.disappear(-1);
		_answer3.disappear(-1);

		// T_T
		for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
			BrickGroup_888888.getObject(%i).enableTouch = 1;
		}
	}

	function GameConnection::spawnPlayer(%this) {
		if(%this.canPlay) {
			parent::spawnPlayer(%this);
			%this.player.setTransform(_in_spawn.getTransform());
		} else {
			%this.setCameraBrick("camera_general", "camera_general_aim");
		}

		return;
	}

	function fxDTSBrick::onPlayerTouch(%this, %player) {
		if(stripos(%this.getName(), "_floor") == -1) {
			%player.hasAnswered = 0;
		} else {
			%player.hasAnswered = 1;
		}

		return parent::onPlayerTouch(%this, %player);
	}

	function MinigameSO::checkLastManStanding(%this) {
		// manually checking
		return;
	}

	function MinigameSO::reset(%this) {
		for(%i=0;%i<%this.numMembers;%i++) {
			%client = %this.member[%i];
			%client.canPlay = 1;
			if(isObject(%client.player)) {
				%client.player.delete();
			}
			%client.spawnPlayer();
		}

		return parent::reset(%this);
	}

	function GameConnection::onDeath(%this, %obj, %killer, %type, %area) {
		%this.canPlay = 0;

		return parent::onDeath(%this, %obj, %killer, %type, %area);
	}
};
activatePackage(TriviaPackage);

talk("Trivia v" @ $Trivia::Version);