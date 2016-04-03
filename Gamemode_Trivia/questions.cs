function gatherTriviaQuestions() {
	%file = new FileObject();
	%file.openForRead("Add-Ons/Gamemode_Trivia/questions.tsv");

	%i = 0;
	while(!%file.isEOF()) {
		%line = %file.readLine();

		$Trivia::Question[%i] = getField(%line, 0);
		// tfw no explode :(
		$Trivia::Choices[%i] = strReplace(getField(%line, 1), ":", "\t");		
		$Trivia::Category[%i] = getField(%line, 2);

		%i++;
	}

	talk("Gathered" SPC %i+1 SPC "questions.");
	$Trivia::QuestionCount = %i;
}

function getChoices(%question) {
	%choices = $Trivia::Choices[%question];
	%total = getFieldCount(%choices)-1;

	for(%i=0;%i<%total;%i++) {
		%choice[%i] = getField(%choices, %i);
		if(getSubStr(%choice[%i], 0, 1) $= "*") {
			%correct = %i;
			$Trivia::CorrectAnswer = %correct;
			%choice[%i] = getSubStr(%choice[%i], 1, strLen(%choice[%i]));
		}
	}

	if(%total == 1) {
		%chosen = "0 1";
	} else {
		%chosen = "";
		if(%total == 2) {
			echo("ERROR WITH QUESTION.");
			return "0 1 2";
		}
		for(%i=0;%i<4;%i++) {
			%rand = getRandom(0, %total);
			while(stripos(%chosen, " " @ %rand) != -1 || %rand == %correct) {
				echo("collision" SPC %rand);
				%rand = getRandom(0, %total);
			}
			if(!%inserted && getRandom(0, 1)) {
				%inserted = 1;
				%chosen = %chosen @ " " @ %correct;
				continue;
			}
			if(%i == 3 && !%inserted) {
				%chosen = %chosen @ " " @ %correct;
				break;
			}

			%chosen = %chosen @ " " @ %rand;
		}
	}


	return trim(%chosen);
}

function getChoiceText(%question, %choice) {
	return strReplace(getField($Trivia::Choices[%question], %choice), "*", "");
}
function getQuestion(%question) {
	return $Trivia::Question[%question];
}

function setTriviaShapeText(%which, %text) {
	switch$(%which) {
		case 0:
			%obj = Answer0Text;
		case 1:
			%obj = Answer1Text;
		case 2:
			%obj = Answer2Text;
		case 3:
			%obj = Answer3Text;
		case "question":
			%obj = QuestionText;
	}

	%obj.setShapeName(%text);
}

function setTriviaQuestion() {
	$Trivia::CurrentQuestion = getRandom(0, $Trivia::QuestionCount-1);
	%current = $Trivia::CurrentQuestion;

	messageAll('errorSound', "\c5" @ getQuestion(%current));
	setTriviaShapeText("question", getQuestion(%current));

	%all = "<font:Arial Bold:14> \c5" @ getQuestion(%current) @ "<br> \c6";

	%choices = getChoices(%current);
	$Trivia::CurrentChoices = %choices;

	%letters = "A. B. C. D.";

	if(getWordCount(%choices) == 2) {
		setTriviaShapeText(2, "");
		setTriviaShapeText(3, "");
	}

	for(%i=0;%i<getWordCount(%choices);%i++) {
		%text = getWord(%letters, %i) SPC getChoiceText(%current, getWord(%choices, %i));
		messageAll('', "\c6" @ %text);
		setTriviaShapeText(%i, %text);
		%all = %all @ %text @ "    ";
	}

	bottomPrintAll(%all);
}

//for(%question=0;%question<70;%question++) {
//	%choices = getChoices(%question);
//
//	%done = "";
//	for(%choice=0;%choice<getWordCount(%choices);%choice++) {
//		%done = trim(%done @ " " @ getChoiceText(%question, getWord(%choices, %choice));
//	}
//	echo(%done);
//}